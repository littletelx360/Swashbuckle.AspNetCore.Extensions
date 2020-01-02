﻿using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;

namespace Microsoft.AspNetCore.Builder
{
    public static class SwaggerOptionsExtensions
    {
        public static SwaggerOptions ResolveBasePathByRequestReferer(this SwaggerOptions options)
        {
            return ResolveBasePathByRequestReferer(options, "swagger");
        }

        public static SwaggerOptions ResolveBasePathByRequestReferer(this SwaggerOptions options,
            string swaggerRoutePrefix)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            if (string.IsNullOrWhiteSpace(swaggerRoutePrefix))
                throw new ArgumentNullException(nameof(swaggerRoutePrefix));

            options.PreSerializeFilters.Add(BuildBasePathFilterByCheckRequestReferer(swaggerRoutePrefix));

            return options;
        }

        private static Action<OpenApiDocument, HttpRequest> BuildBasePathFilterByCheckRequestReferer(string swaggerRoutePrefix)
        {
            return (swaggerDoc, httpReq) =>
            {
                var refererUrl = httpReq.Headers[HeaderNames.Referer].ToString();
                if (string.IsNullOrWhiteSpace(refererUrl))
                    return;

                var referer = new Uri(refererUrl);
                var docHost = httpReq.Headers[HeaderNames.Host].ToString();
                if (string.Compare($"{referer.Host}:{referer.Port}", docHost, true) == 0)
                    return;

                var swaggerRouteIndex = referer.AbsolutePath.IndexOf($"/{swaggerRoutePrefix}");
                if (swaggerRouteIndex <= 0)
                    return;

                var basePath = referer.AbsolutePath.Substring(0, swaggerRouteIndex);
                swaggerDoc.Servers = new List<OpenApiServer> { new OpenApiServer { Url = $"{httpReq.Scheme}://{httpReq.Host.Value}{basePath}" } };                
            };
        }
    }
}

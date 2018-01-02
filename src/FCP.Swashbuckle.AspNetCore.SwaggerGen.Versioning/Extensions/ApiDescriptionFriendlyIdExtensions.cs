﻿using Microsoft.AspNetCore.Mvc.ApiExplorer;
using System.Linq;
using System.Text;

namespace Swashbuckle.AspNetCore.SwaggerGen
{
    internal static class ApiDescriptionFriendlyIdExtensions
    {
        //Ammended from https://github.com/domaindrivendev/Swashbuckle.AspNetCore/blob/master/src/Swashbuckle.AspNetCore.SwaggerGen/Generator/ApiDescriptionExtensions.cs
        internal static string FriendlyId(this ApiDescription apiDescription, string versionParamName)
        {
            var relativePathWithoutVersion = apiDescription.RelativePathSansQueryString();

            if (!string.IsNullOrWhiteSpace(versionParamName))
                relativePathWithoutVersion = relativePathWithoutVersion.Replace($"v{{{versionParamName}}}", string.Empty);
           
            var parts = (relativePathWithoutVersion + "/" + apiDescription.HttpMethod.ToLower()).Split('/');

            var builder = new StringBuilder();

            foreach (var part in parts)
            {
                var trimmed = part.Trim('{', '}');
                builder.AppendFormat("{0}{1}",
                    (part.StartsWith("{") ? "By" : string.Empty),
                    trimmed.ToTitleCase());
            }

            return builder.ToString();
        }

        internal static string RelativePathSansQueryString(this ApiDescription apiDescription)
        {
            return apiDescription.RelativePath.Split('?').First();
        }

        //https://github.com/domaindrivendev/Swashbuckle.AspNetCore/blob/master/src/Swashbuckle.AspNetCore.SwaggerGen/Generator/StringExtensions.cs
        internal static string ToTitleCase(this string value)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return char.ToUpperInvariant(value[0]) + value.Substring(1);
        }
    }
}

using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using PedroTer7.AspNetBasicAuth.Documentation;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace PedroTer7.AspNetBasicAuth.Extensions
{
    public static class SwaggerGenOptionsExtesions
    {
        public static void AddBasicAuthentication(this SwaggerGenOptions options)
        {
            AddSecurityDefinition(options);
            var filterSettings = BuildOperationFilterSettings(false);
            RegisterOperationFilter(options, filterSettings);
        }

        public static void AddBasicAuthentication(this SwaggerGenOptions options,
            bool everyAuthorizeAttribute, string[]? policies = null, string[]? roles = null)
        {
            if (everyAuthorizeAttribute)
            {
                options.AddBasicAuthentication();
            }
            else
            {
                var filterSettings = BuildOperationFilterSettings(true, policies, roles);
                RegisterOperationFilter(options, filterSettings);
            }
        }

        private static void AddSecurityDefinition(SwaggerGenOptions options)
        {
            options.AddSecurityDefinition(OpenApiDefinitions.SecuritySchemaId
                , new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Scheme = "Basic",
                    Type = SecuritySchemeType.Http,
                });
        }

        private static OperationFilterSettings BuildOperationFilterSettings(bool shouldFilter, string[]? policies = null, string[]? roles = null)
        {
            if (!shouldFilter)
                return new OperationFilterSettings(false, false, Array.Empty<string>(), Array.Empty<string>());
            if (policies is not null)
                return new OperationFilterSettings(true, false, policies, Array.Empty<string>());
            if (roles is not null)
                return new OperationFilterSettings(true, false, Array.Empty<string>(), roles);

            return new OperationFilterSettings(true, true, Array.Empty<string>(), Array.Empty<string>());
        }

        private static void RegisterOperationFilter(SwaggerGenOptions options, OperationFilterSettings settings)
        {
            options.OperationFilter<BasicAuthenticationOperationFilter>(new object[] { settings });
        }
    }
}

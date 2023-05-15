using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using PedroTer7.AspNetBasicAuth.Documentation;
using Swashbuckle.AspNetCore.SwaggerGen;
using PedroTer7.AspNetBasicAuth.Authentication;

namespace PedroTer7.AspNetBasicAuth.Extensions
{
    public static class SwaggerGenOptionsExtesions
    {
        /// <summary>
        /// Configures basic authentication schema documentation 
        /// in the generated OpenAPI document.
        /// <para>
        /// This overload applies the basic authentication schema
        /// for every action containing at least one <c>Authorize</c>
        /// attribute. See other overloads to fine grain this behaviour.
        /// </para>
        /// </summary>
        /// <param name="options"></param>
        public static void AddBasicAuthentication(this SwaggerGenOptions options)
        {
            AddSecurityDefinition(options);
            var filterSettings = BuildOperationFilterSettings(false);
            RegisterOperationFilter(options, filterSettings);
        }

        /// <summary>
        /// Configures basic authentication schema documentation 
        /// in the generated OpenAPI document.
        /// <para>
        /// If both <paramref name="policies"/> and <paramref name="roles"/> are
        /// <c>null</c> and <paramref name="everyAuthorizeAttribute"/> is
        /// <c>false</c>, then the method will apply the authentication
        /// schema to every action that contains an <c>Authorize</c> attribute
        /// that has <see cref="BasicAuthenticationDefaults.SchemaName"/>
        /// in its <c>AuthenticationSchemes</c> list.
        /// </para>
        /// </summary>
        /// <param name="options"></param>
        /// <param name="everyAuthorizeAttribute">If <c>true</c> then
        /// the basic authentication schema will be applied to every
        /// action that contains at least one <c>Authorize</c> attribute.</param>
        /// <param name="policies">A list of policies that the operation will use
        /// to decide if the authentication schema should be applied to a given action
        /// that contains <c>Authorize</c> attribute(s). If a given action contains
        /// an <c>Authorize</c> attribute with a policy that is within this list
        /// then the schema will be applied to that method. Policies take precedence
        /// over roles if both are given.</param>
        /// <param name="roles">A list of roles that the operation will use
        /// to decide if the authentication schema should be applied to a given action
        /// that contains <c>Authorize</c> attribute(s). If a given action contains
        /// an <c>Authorize</c> attribute and at least one of the roles in the attribute
        /// roles array is within this list then the schema is applied to that method.
        /// Policies take precedence over roles if both are given.</param>
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

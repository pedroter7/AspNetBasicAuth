using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using PedroTer7.AspNetBasicAuth.Authentication;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace PedroTer7.AspNetBasicAuth.Documentation
{
    internal class BasicAuthenticationOperationFilter : IOperationFilter
    {
        private readonly OperationFilterSettings _settings;

        public BasicAuthenticationOperationFilter(OperationFilterSettings settings)
        {
            _settings = settings;
        }

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var shouldApply = context.MethodInfo
                .GetCustomAttributes(true)
                .OfType<AuthorizeAttribute>()
                .Where(attr => !_settings.ShouldFilterAuthorizeAttributes || FilterAuthorizeAttribute(attr))
                .Any();

            if (shouldApply)
            {
                operation.Responses.Add("401", new OpenApiResponse { Description = "Unauthorized" });
                operation.Responses.Add("403", new OpenApiResponse { Description = "Forbidden" });

                var basicScheme = new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = OpenApiDefinitions.SecuritySchemaId }
                };

                operation.Security = new List<OpenApiSecurityRequirement>() {
                    new() {{
                        new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = OpenApiDefinitions.SecuritySchemaId }
                            },
                            new List<string>()
                        }
                    }
                };
            }
        }

        private bool FilterAuthorizeAttribute(AuthorizeAttribute attribute)
        {
            if (_settings.UseDefaultFilter)
                return AttributeSatisfiesDefaultCondition(attribute);

            if (_settings.PoliciesAuthorizeAttributeShouldContain.Length > 0)
                return AttributeContainsAtLeastOnePolicy(attribute, _settings.PoliciesAuthorizeAttributeShouldContain);

            return AttributeContainsAtLeastOneRole(attribute, _settings.RolesAuthorizeAttributeShouldContain);
        }

        private static bool AttributeContainsAtLeastOnePolicy(AuthorizeAttribute attribute,
            string[] policiesAuthorizeAttributeShouldContain)
        {
            return policiesAuthorizeAttributeShouldContain.Contains(attribute.Policy);
        }

        private static bool AttributeContainsAtLeastOneRole(AuthorizeAttribute attribute,
            string[] rolesAuthorizeAttributeShouldContain)
        {
            return attribute.Roles is not null
                && rolesAuthorizeAttributeShouldContain.Where(r => attribute.Roles.Contains(r)).Any();
        }

        private static bool AttributeSatisfiesDefaultCondition(AuthorizeAttribute attribute)
        {
            return attribute.AuthenticationSchemes is not null
                && attribute.AuthenticationSchemes
                    .Contains(BasicAuthenticationDefaults.SchemaName, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}

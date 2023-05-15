using Microsoft.AspNetCore.Authentication;
using PedroTer7.AspNetBasicAuth.Authentication;

namespace PedroTer7.AspNetBasicAuth.Extensions
{
    public static class AuthenticationBuilderExtensions
    {
        public static AuthenticationBuilder AddBasicAuthentication<T, K>(this AuthenticationBuilder builder, Type referenceType, Action<K> configureOptions)
            where T : new()
            where K : AuthenticationSchemeOptions, new()
        {
            builder.AddScheme<K, BasicAuthenticationHandler<K, T>>(BasicAuthenticationDefaults.SchemaName, configureOptions);
            builder.Services.RegisterBasicAuthenticationServices<T, K>(referenceType);
            return builder;
        }

        public static AuthenticationBuilder AddBasicAuthentication<T>(this AuthenticationBuilder builder,
            Type referenceType, Action<DefaultBasicAuthenticationSchemeOptions> configureOptions)
            where T : new()
        {
            return builder.AddBasicAuthentication<T, DefaultBasicAuthenticationSchemeOptions>(referenceType, configureOptions);
        }
    }
}

using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using PedroTer7.AspNetBasicAuth.Authentication;

namespace PedroTer7.AspNetBasicAuth.Extensions
{
    public static class IServiceCollectionExtensions
    {
        internal static void RegisterBasicAuthenticationServices<T, K>(this IServiceCollection services, Type referenceType)
            where T : new()
            where K : AuthenticationSchemeOptions, new()
        {
            RegisterIUserClaims<T, K>(services, referenceType);
            RegisterIUserCredentialsValidator(services, referenceType);
            RegisterPostConfigureOptions(services);
        }

        internal static void RegisterBasicAuthenticationServices<T>(this IServiceCollection services, Type referenceType)
            where T : new()
        {
            services.RegisterBasicAuthenticationServices<T, DefaultBasicAuthenticationSchemeOptions>(referenceType);
        }

        private static void RegisterIUserClaims<T, K>(IServiceCollection services, Type referenceType)
            where T : new()
            where K : AuthenticationSchemeOptions, new()
        {
            var implementation = referenceType.Assembly.GetTypes()
                .Where(t => t.GetInterfaces().Contains(typeof(IUserClaimsBuilder<T, K>)))
                .FirstOrDefault();

            if (implementation is null)
                throw new InvalidOperationException($"In order to use basic authentication, the client must implement the interface {typeof(IUserClaimsBuilder<T, K>).FullName}");

            services.AddTransient(typeof(IUserClaimsBuilder<T, K>), implementation);
        }

        private static void RegisterIUserCredentialsValidator(IServiceCollection services, Type referenceType)
        {
            var implementation = referenceType.Assembly.GetTypes()
                .Where(t => t.GetInterfaces().Contains(typeof(IUserCredentialsValidator)))
                .FirstOrDefault();

            if (implementation is null)
                throw new InvalidOperationException($"In order to use basic authentication, the client must implement the interface {typeof(IUserCredentialsValidator).FullName}");

            services.AddTransient(typeof(IUserCredentialsValidator), implementation);
        }

        private static void RegisterPostConfigureOptions(IServiceCollection services)
        {
            services.AddSingleton<IPostConfigureOptions<DefaultBasicAuthenticationSchemeOptions>,
                DefaultBasicAuthenticationSchemeOptionsPostConfigureOptions>();
        }
    }
}

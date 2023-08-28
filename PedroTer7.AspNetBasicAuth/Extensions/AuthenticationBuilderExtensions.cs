using Microsoft.AspNetCore.Authentication;
using PedroTer7.AspNetBasicAuth.Authentication;

namespace PedroTer7.AspNetBasicAuth.Extensions
{
    public static class AuthenticationBuilderExtensions
    {
        /// <summary>
        /// Enables basic authentication using <see cref="BasicAuthenticationDefaults"/>.
        /// <para>
        /// Basic authentication performs authentication by extracting an username and password
        /// from the Authorize header in the request.
        /// </para>
        /// This method registers the needed service implementations in the ASP.NET DI container.
        /// </summary>
        /// <typeparam name="T">Data model used by the auth handler to retrieve
        /// user data after she has been successfully authenticated. This data model
        /// must be the same as the one used when implementing 
        /// <see cref="IUserClaimsBuilder{T, K}"/>.</typeparam>
        /// <typeparam name="K">Schema options used to perform basic authentication
        /// by the <see cref="BasicAuthenticationHandler{T, K}"/>.</typeparam>
        /// <param name="builder"></param>
        /// <param name="referenceType">A type in the assembly where the interfaces needed
        /// by the basic authentication library are implemented.</param>
        /// <param name="configureOptions">A delegate to configure the schema options.</param>
        /// <returns></returns>
        public static AuthenticationBuilder AddBasicAuthentication<T, K>(this AuthenticationBuilder builder, Type referenceType, Action<K> configureOptions)
            where T : new()
            where K : AuthenticationSchemeOptions, new()
        {
            builder.AddScheme<K, BasicAuthenticationHandler<K, T>>(BasicAuthenticationDefaults.SchemaName, configureOptions);
            builder.Services.RegisterBasicAuthenticationServices<T, K>(referenceType);
            return builder;
        }

        /// <summary>
        /// Enables basic authentication using <see cref="BasicAuthenticationDefaults"/>
        /// and <see cref="DefaultBasicAuthenticationSchemeOptions"/> as scheme options.
        /// <para>
        /// Basic authentication performs authentication by extracting an username and password
        /// from the Authorize header in the request.
        /// </para>
        /// This method registers the needed service implementations in the ASP.NET DI container.
        /// </summary>
        /// <typeparam name="T">Data model used by the auth handler to retrieve
        /// user data after she has been successfully authenticated. This data model
        /// must b the same as the one used when implementing 
        /// <see cref="IUserClaimsBuilder{T, K}"/>.</typeparam>
        /// <param name="builder"></param>
        /// <param name="referenceType">A type in the assembly where the interfaces needed
        /// by the basic authentication library are implemented.</param>
        /// <param name="configureOptions">A delegate to configure <see cref="DefaultBasicAuthenticationSchemeOptions"/>.</param>
        /// <returns></returns>
        public static AuthenticationBuilder AddBasicAuthentication<T>(this AuthenticationBuilder builder,
            Type referenceType, Action<DefaultBasicAuthenticationSchemeOptions> configureOptions)
            where T : new()
        {
            return builder.AddBasicAuthentication<T, DefaultBasicAuthenticationSchemeOptions>(referenceType, configureOptions);
        }
    }
}

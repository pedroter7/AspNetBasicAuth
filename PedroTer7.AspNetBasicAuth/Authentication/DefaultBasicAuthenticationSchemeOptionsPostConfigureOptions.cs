using Microsoft.Extensions.Options;

namespace PedroTer7.AspNetBasicAuth.Authentication
{
    internal class DefaultBasicAuthenticationSchemeOptionsPostConfigureOptions
        : IPostConfigureOptions<DefaultBasicAuthenticationSchemeOptions>
    {
        public void PostConfigure(string name, DefaultBasicAuthenticationSchemeOptions options)
        {
            if (string.IsNullOrEmpty(options.Realm))
            {
                throw new InvalidOperationException($"A value for Realm must be provided in order to use {typeof(DefaultBasicAuthenticationSchemeOptions).FullName} as authentication scheme options");
            }
        }
    }
}

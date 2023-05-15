using Microsoft.AspNetCore.Authentication;

namespace PedroTer7.AspNetBasicAuth.Authentication
{
    public class DefaultBasicAuthenticationSchemeOptions : AuthenticationSchemeOptions
    {
        public string Realm { get; set; } = null!;
    }
}

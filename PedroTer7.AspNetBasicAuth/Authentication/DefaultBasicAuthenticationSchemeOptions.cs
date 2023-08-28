using Microsoft.AspNetCore.Authentication;

namespace PedroTer7.AspNetBasicAuth.Authentication
{
    /// <summary>
    /// Default options for the <see cref="BasicAuthenticationHandler{T, K}" />.
    /// Note that the <see cref="Realm"/> property is mandatory when configuring
    /// the authentication scheme options.
    /// </summary>
    public class DefaultBasicAuthenticationSchemeOptions : AuthenticationSchemeOptions
    {
        public string Realm { get; set; } = null!;
    }
}

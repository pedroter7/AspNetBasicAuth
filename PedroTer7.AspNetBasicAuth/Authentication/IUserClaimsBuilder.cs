using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace PedroTer7.AspNetBasicAuth.Authentication
{
    /// <summary>
    /// Implementations of this interface are used to build <see cref="Claim"/>
    /// objects by the <see cref="BasicAuthenticationHandler{T, K}"/> during basic authentication.
    /// </summary>
    /// <typeparam name="T">Model to hold user data retrived by
    /// <see cref="GetUserData(string)"/> implementation.</typeparam>
    /// <typeparam name="K">Authentication scheme options used during basic
    /// authentication in <see cref="BasicAuthenticationHandler{T, K}"/>.</typeparam>
    public interface IUserClaimsBuilder<T, K> where T : new() where K : AuthenticationSchemeOptions, new()
    {
        /// <summary>
        /// Retrieves data for some user.
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        Task<T> GetUserData(string username);

        /// <summary>
        /// Builds a list of <see cref="Claim"/> objects that are used to create
        /// the principal during basic authentication.
        /// </summary>
        /// <param name="userData"></param>
        /// <param name="authenticationSchemeOptions"></param>
        /// <returns></returns>
        IList<Claim> BuildClaims(T userData, K authenticationSchemeOptions);
    }
}

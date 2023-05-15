using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace PedroTer7.AspNetBasicAuth.Authentication
{
    public interface IUserClaimsBuilder<T, K> where T : new() where K : AuthenticationSchemeOptions, new()
    {
        Task<T> GetUserData(string username);
        IList<Claim> BuildClaims(T userData, K authenticationSchemeOptions);
    }
}

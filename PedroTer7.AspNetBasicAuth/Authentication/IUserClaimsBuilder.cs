using System.Security.Claims;

namespace PedroTer7.AspNetBasicAuth.Authentication
{
    public interface IUserClaimsBuilder<T> where T : new()
    {
        Task<T> GetUserData(string username);
        IList<Claim> BuildClaims(T userData);
    }
}

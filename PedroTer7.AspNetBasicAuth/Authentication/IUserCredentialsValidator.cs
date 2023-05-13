namespace PedroTer7.AspNetBasicAuth.Authentication
{
    public interface IUserCredentialsValidator
    {
        Task<bool> Validate(string username, string password);
    }
}

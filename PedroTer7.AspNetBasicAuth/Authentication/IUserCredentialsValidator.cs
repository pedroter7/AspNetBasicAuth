namespace PedroTer7.AspNetBasicAuth.Authentication
{
    /// <summary>
    /// Implementations of this interface are used by the
    /// <see cref="BasicAuthenticationHandler{T, K}"/> to validate
    /// user credentials, i.e. to authenticate the user against its
    /// credentials.
    /// </summary>
    public interface IUserCredentialsValidator
    {
        /// <summary>
        /// Validate user credentials for the basic authentication flow.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns><c>true</c> if username and password combination is valid;
        /// <c>false</c>  otherwise.</returns>
        Task<bool> Validate(string username, string password);
    }
}

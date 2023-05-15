using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;

namespace PedroTer7.AspNetBasicAuth.Authentication
{
    public class BasicAuthenticationHandler<T, K>
        : AuthenticationHandler<T>
        where T : AuthenticationSchemeOptions, new()
        where K : new()
    {
        private readonly IUserCredentialsValidator _userCredentialsValidator;
        private readonly IUserClaimsBuilder<K, T> _userClaimsBuilder;

        public BasicAuthenticationHandler(IOptionsMonitor<T> options,
            ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock,
            IUserCredentialsValidator userCredentialsValidator, IUserClaimsBuilder<K, T> userClaimsBuilder)
                : base(options, logger, encoder, clock)
        {
            _userCredentialsValidator = userCredentialsValidator;
            _userClaimsBuilder = userClaimsBuilder;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                return AuthenticateResult.NoResult();
            }

            if (!AuthenticationHeaderValue.TryParse(Request.Headers["Authorization"], out var authHeaderValue))
            {
                return AuthenticateResult.NoResult();
            }

            if (!BasicAuthenticationDefaults.AuthorizationScheme.Equals(authHeaderValue.Scheme, StringComparison.InvariantCultureIgnoreCase))
            {
                return AuthenticateResult.NoResult();
            }

            if (authHeaderValue.Parameter is null)
            {
                return AuthenticateResult.NoResult();
            }

            try
            {
                var (username, password) = ExtractUsernameAndPasswordFromBase64String(authHeaderValue.Parameter);
                var credentialsAreValid = await _userCredentialsValidator.Validate(username, password);
                if (!credentialsAreValid)
                {
                    return AuthenticateResult.Fail("Invalid username or password");
                }
                var userData = await _userClaimsBuilder.GetUserData(username);
                var claims = _userClaimsBuilder.BuildClaims(userData, Options);
                var identity = new ClaimsIdentity(claims);
                var principal = new ClaimsPrincipal(identity);
                var ticket = new AuthenticationTicket(principal, Scheme.Name);
                return AuthenticateResult.Success(ticket);
            }
            catch (Exception e)
            {
                if (e is ArgumentException || e is FormatException)
                {
                    return AuthenticateResult.NoResult();
                }

                throw;
            }
        }

        private static (string username, string password) ExtractUsernameAndPasswordFromBase64String(string base64String)
        {
            var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(base64String));
            if (string.IsNullOrEmpty(decoded))
                throw new ArgumentException("Decoded string is empty or null");

            var spplited = decoded.Split(":");
            if (spplited.Length != 2)
                throw new FormatException(decoded);

            return (spplited[0], spplited[1]);
        }
    }
}

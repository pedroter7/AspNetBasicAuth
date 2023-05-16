using Bogus;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using PedroTer7.AspNetBasicAuth.Authentication;
using System.Text;
using System.Text.Encodings.Web;


namespace PedroTer7.AspNetBasicAuth.Tests.Authentication
{
    using IUserClaimsBuilder = IUserClaimsBuilder<UserDataContainer, DefaultBasicAuthenticationSchemeOptions>;
    public class BasicAuthenticationHandlerTestsFixture
    {
        private readonly IOptionsMonitor<DefaultBasicAuthenticationSchemeOptions> _optionsMonitor;
        private readonly ILoggerFactory _loggerFactory;
        private readonly UrlEncoder _urlEncoder;
        private readonly ISystemClock _systemClock;
        private readonly Faker _faker;

        public string RandomUsername => _faker.Internet.UserName();
        public string RandomPassword => _faker.Internet.Password();

        public BasicAuthenticationHandlerTestsFixture()
        {
            _faker = new Faker();
            _optionsMonitor = Substitute.For<IOptionsMonitor<DefaultBasicAuthenticationSchemeOptions>>();
            _optionsMonitor.Get(Arg.Any<string>()).Returns(new DefaultBasicAuthenticationSchemeOptions
            {
                Realm = _faker.Internet.DomainName()
            }); ;
            _loggerFactory = Substitute.For<ILoggerFactory>();
            _urlEncoder = UrlEncoder.Default;
            _systemClock = Substitute.For<ISystemClock>();
        }

        public async Task<BasicAuthenticationHandler<DefaultBasicAuthenticationSchemeOptions, UserDataContainer>> CreateHandlerWithProperHeaders(string username, string password, IUserCredentialsValidator userCredentialsValidator, IUserClaimsBuilder userClaimsBuilder)
        {
            var handler = CreateHandler(userCredentialsValidator, userClaimsBuilder);
            var context = CreateContext(BuildBasicAuthorizationHeaderString(username, password));
            var schema = CreateScheme();
            await handler.InitializeAsync(schema, context);
            return handler;
        }

        public async Task<BasicAuthenticationHandler<DefaultBasicAuthenticationSchemeOptions, UserDataContainer>> CreateHandlerWithMalformedBasicAuthorizationHeaderValue(IUserCredentialsValidator userCredentialsValidator, IUserClaimsBuilder userClaimsBuilder)
        {
            var handler = CreateHandler(userCredentialsValidator, userClaimsBuilder);
            var context = CreateContext(BuildMalformedBasicAuthorizationHeaderString());
            var schema = CreateScheme();
            await handler.InitializeAsync(schema, context);
            return handler;
        }

        public async Task<BasicAuthenticationHandler<DefaultBasicAuthenticationSchemeOptions, UserDataContainer>> CreateHandlerWithHeaderThatIsNotBasic(IUserCredentialsValidator userCredentialsValidator, IUserClaimsBuilder userClaimsBuilder)
        {
            var handler = CreateHandler(userCredentialsValidator, userClaimsBuilder);
            var context = CreateContext(_faker.Random.Hash());
            var schema = CreateScheme();
            await handler.InitializeAsync(schema, context);
            return handler;
        }

        public async Task<BasicAuthenticationHandler<DefaultBasicAuthenticationSchemeOptions, UserDataContainer>> CreateHandlerWithEmptyAuthorizationHeader(IUserCredentialsValidator userCredentialsValidator, IUserClaimsBuilder userClaimsBuilder)
        {
            var handler = CreateHandler(userCredentialsValidator, userClaimsBuilder);
            var context = CreateContext();
            var schema = CreateScheme();
            await handler.InitializeAsync(schema, context);
            return handler;
        }

        private static AuthenticationScheme CreateScheme()
            => new(BasicAuthenticationDefaults.SchemaName, null, typeof(BasicAuthenticationHandler<DefaultBasicAuthenticationSchemeOptions, UserDataContainer>));

        private BasicAuthenticationHandler<DefaultBasicAuthenticationSchemeOptions, UserDataContainer> CreateHandler(IUserCredentialsValidator userCredentialsValidator, IUserClaimsBuilder userClaimsBuilder)
            => new(_optionsMonitor, _loggerFactory, _urlEncoder, _systemClock, userCredentialsValidator, userClaimsBuilder);

        private static byte[] GetStringBytes(string str) => Encoding.UTF8.GetBytes(str);

        private static HttpContext CreateContext(string authorizationHeaderValue)
        {
            var context = CreateContext();
            context.Request.Headers.Authorization = authorizationHeaderValue;
            return context;
        }

        private static HttpContext CreateContext() => new DefaultHttpContext();

        private static string BuildBasicAuthorizationHeaderString(string username, string password)
        {
            var bytes = Encoding.UTF8.GetBytes(username + ":" + password);
            return $"Basic {Convert.ToBase64String(bytes)}";
        }

        private string BuildMalformedBasicAuthorizationHeaderString()
        {
            var bytes = Encoding.UTF8.GetBytes(_faker.Random.AlphaNumeric(_faker.Random.Int(3, 200)));
            return $"Basic {Convert.ToBase64String(bytes)}";
        }
    }
}

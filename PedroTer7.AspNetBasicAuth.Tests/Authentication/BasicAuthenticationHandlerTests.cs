using NSubstitute;
using NSubstitute.ExceptionExtensions;
using PedroTer7.AspNetBasicAuth.Authentication;
using System.Security.Claims;

namespace PedroTer7.AspNetBasicAuth.Tests.Authentication
{
    using IUserClaimsBuilder = IUserClaimsBuilder<UserDataContainer, DefaultBasicAuthenticationSchemeOptions>;

    public class BasicAuthenticationHandlerTests : IClassFixture<BasicAuthenticationHandlerTestsFixture>
    {
        private readonly BasicAuthenticationHandlerTestsFixture _fixture;
        public readonly IUserCredentialsValidator _userCredentialsValidator;
        private readonly IUserClaimsBuilder _userClaimsBuilder;


        public BasicAuthenticationHandlerTests(BasicAuthenticationHandlerTestsFixture fixture)
        {
            _fixture = fixture;
            _userCredentialsValidator = Substitute.For<IUserCredentialsValidator>();
            _userClaimsBuilder = Substitute.For<IUserClaimsBuilder>();
        }

        private void SetupUserCredentialsValidatorMockToReturnFalseForAnyParams()
        {
            _userCredentialsValidator.Validate(Arg.Any<string>(), Arg.Any<string>())
                .Returns(Task.FromResult(false));
        }

        private void SetupUserCredentialsValidatorMockToReturnTrue(string username, string password)
        {
            _userCredentialsValidator.Validate(username, password)
                .Returns(Task.FromResult(true));
        }

        private void SetupUserCredentialsValidatorMockToThrow()
        {
            _userCredentialsValidator.Validate(Arg.Any<string>(), Arg.Any<string>())
                .Throws(new Exception());
        }

        private void SetupUserClaimsBuilderMockToReturnSuccess()
        {
            _userClaimsBuilder.GetUserData(Arg.Any<string>())
                .Returns(x => Task.FromResult(new UserDataContainer
                {
                    UserName = (string)x[0]
                }));

            _userClaimsBuilder.BuildClaims(Arg.Any<UserDataContainer>(), Arg.Any<DefaultBasicAuthenticationSchemeOptions>())
                .Returns(x => new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, ((UserDataContainer)x[0]).UserName)
                });
        }

        private void SetupUserClaimsBuilderMockToThrowDuringUserDataRetrieval()
        {
            _userClaimsBuilder.GetUserData(Arg.Any<string>())
                .Throws(new Exception());
        }

        [Fact(DisplayName = "Authentication handler should return success if username and password are valid")]
        public async Task TestAuthenticationHandler_HandleAuthenticateAsync_CredentialsValid_ShoudlReturnSuccess()
        {
            var username = _fixture.RandomUsername;
            var password = _fixture.RandomPassword;
            SetupUserCredentialsValidatorMockToReturnTrue(username, password);
            SetupUserClaimsBuilderMockToReturnSuccess();
            var handler = await _fixture.CreateHandlerWithProperHeaders(username, password, _userCredentialsValidator, _userClaimsBuilder);

            var result = await handler.AuthenticateAsync();

            Assert.NotNull(result);
            Assert.True(result.Succeeded);
            await _userCredentialsValidator
                .Received(1)
                .Validate(username, password);

            await _userCredentialsValidator
                .DidNotReceive()
                .Validate(Arg.Is<string>(x => !x.Equals(username)), Arg.Is<string>(x => !x.Equals(password)));

            await _userClaimsBuilder
                .Received(1)
                .GetUserData(username);

            await _userClaimsBuilder
                .DidNotReceive()
                .GetUserData(Arg.Is<string>(x => !x.Equals(username)));

            _userClaimsBuilder
                .Received(1)
                .BuildClaims(Arg.Any<UserDataContainer>(), Arg.Any<DefaultBasicAuthenticationSchemeOptions>());
        }

        [Fact(DisplayName = "Authentication handler should return failure when username and password are not valid")]
        public async Task TestAuthenticationHandler_HandlerAuthenticateAsync_CredentialsAreNotValid_ShouldReturnFailure()
        {
            var username = _fixture.RandomUsername;
            var password = _fixture.RandomPassword;
            SetupUserCredentialsValidatorMockToReturnFalseForAnyParams();
            var handler = await _fixture.CreateHandlerWithProperHeaders(username, password, _userCredentialsValidator, _userClaimsBuilder);

            var result = await handler.AuthenticateAsync();

            Assert.NotNull(result);
            Assert.False(result.Succeeded);
            await _userCredentialsValidator
                .Received(1)
                .Validate(username, password);

            await _userCredentialsValidator
                .DidNotReceive()
                .Validate(Arg.Is<string>(x => !x.Equals(username)), Arg.Is<string>(x => !x.Equals(password)));
        }

        [Fact(DisplayName = "Authentication handler should return failure when the authorization header value is malformed")]
        public async Task TestAuthenticationHandler_HandlerAuthenticateAsync_MalformedAuthorizationHeaderValue_ShouldReturnFailure()
        {
            SetupUserCredentialsValidatorMockToReturnFalseForAnyParams();
            var handler = await _fixture.CreateHandlerWithMalformedBasicAuthorizationHeaderValue(_userCredentialsValidator, _userClaimsBuilder);

            var result = await handler.AuthenticateAsync();

            Assert.NotNull(result);
            Assert.False(result.Succeeded);
            await _userCredentialsValidator
                .DidNotReceive()
                .Validate(Arg.Any<string>(), Arg.Any<string>());
        }

        [Fact(DisplayName = "Authentication handler should return failure when the authorization header is not basic")]
        public async Task TestAuthenticationHandler_HandlerAuthenticateAsync_AuthorizationHeaderNotBasic_ShouldReturnFailure()
        {
            SetupUserCredentialsValidatorMockToReturnFalseForAnyParams();
            var handler = await _fixture.CreateHandlerWithHeaderThatIsNotBasic(_userCredentialsValidator, _userClaimsBuilder);

            var result = await handler.AuthenticateAsync();

            Assert.NotNull(result);
            Assert.False(result.Succeeded);
            await _userCredentialsValidator
                .DidNotReceive()
                .Validate(Arg.Any<string>(), Arg.Any<string>());
        }

        [Fact(DisplayName = "Authentication handler should return failure when the authorization header is empty")]
        public async Task TestAuthenticationHandler_HandlerAuthenticateAsync_AuthorizationEmpty_ShouldReturnFailure()
        {
            SetupUserCredentialsValidatorMockToReturnFalseForAnyParams();
            var handler = await _fixture.CreateHandlerWithEmptyAuthorizationHeader(_userCredentialsValidator, _userClaimsBuilder);

            var result = await handler.AuthenticateAsync();

            Assert.NotNull(result);
            Assert.False(result.Succeeded);
            await _userCredentialsValidator
                .DidNotReceive()
                .Validate(Arg.Any<string>(), Arg.Any<string>());
        }

        [Fact(DisplayName = "Authentication handler should throw when the provided IUserCredentialsValidator throws exception during credential validation")]
        public async Task TestAuthenticationHandler_HandlerAuthenticateAsync_CredentialValidationThrows_ShouldThrow()
        {
            var username = _fixture.RandomUsername;
            var password = _fixture.RandomPassword;
            SetupUserCredentialsValidatorMockToThrow();
            var handler = await _fixture.CreateHandlerWithProperHeaders(username, password, _userCredentialsValidator, _userClaimsBuilder);

            await Assert.ThrowsAsync<Exception>(() => handler.AuthenticateAsync());
            await _userCredentialsValidator
                .Received(1)
                .Validate(username, password);

            await _userCredentialsValidator
                .DidNotReceive()
                .Validate(Arg.Is<string>(x => !x.Equals(username)), Arg.Is<string>(x => !x.Equals(password)));
        }

        [Fact(DisplayName = "Authentication handler sould throw when the provided get user data implementation throws")]
        public async Task TestAuthenticationHandler_HandlerAuthenticateAsync_GetUserDataThrows_ShouldThrow()
        {
            var username = _fixture.RandomUsername;
            var password = _fixture.RandomPassword;
            SetupUserCredentialsValidatorMockToReturnTrue(username, password);
            SetupUserClaimsBuilderMockToThrowDuringUserDataRetrieval();
            var handler = await _fixture.CreateHandlerWithProperHeaders(username, password, _userCredentialsValidator, _userClaimsBuilder);

            await Assert.ThrowsAsync<Exception>(() => handler.AuthenticateAsync());
            await _userCredentialsValidator
                .Received(1)
                .Validate(username, password);

            await _userCredentialsValidator
                .DidNotReceive()
                .Validate(Arg.Is<string>(x => !x.Equals(username)), Arg.Is<string>(x => !x.Equals(password)));

            await _userClaimsBuilder
                .Received(1)
                .GetUserData(username);

            await _userClaimsBuilder
                .DidNotReceive()
                .GetUserData(Arg.Is<string>(x => !x.Equals(username)));
        }

    }
}

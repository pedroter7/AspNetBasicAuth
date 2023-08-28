# A Basic Authentication Library for ASP.NET Core 🔐

AspNetBasicAuth is a library that implements the basic authentication schema in asp.net core applications using dependency injection to customize credential authentication, perform user data retrieval and principal claims creation. The library also provides a method to configure the SwashBuckle generated OpenAPI specification to support the basic authentication.

## Security Disclaimer ⚠️⚠️⚠️

This library is intended for non-production use. In case you want to use this library in production environments, you are responsible for hardening and testing the authentication methods that this library provides ensuring that it performs as intended. If you really wish to use it in production, do it at your own risk.

## Requirements 📋

 - Enviornment: .NET 6.0+

## Usage 👩‍💻

In order to use the library it is necessary to provide the implementation of two interfaces:

 - `IUserClaimsBuilder<T, K>`: implementations of this interface are used to retrieve user data and build the user claims ([see here](https://learn.microsoft.com/en-us/aspnet/core/security/authorization/claims?view=aspnetcore-7.0)) for that user after successfull credential validation. The type parameters are:
   - `T` is a data model to store the retrieved user data;
   - `K` is the authentication scheme options type used during the authentication;
 - `IUserCredentialsValidator`: implementations of this interface are used by the authentication handler to validate the given user credentials (username and password);

### Basic Usage Example

The following usage example does not provide its own authentication scheme options, thus the library uses by default `DefaultBasicAuthenticationSchemeOptions` as scheme options.

In `Program.cs`:

```C#
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuthentication(BasicAuthenticationDefaults.SchemaName)
    .AddBasicAuthentication<UserData>(typeof(Program), options => {
        options.Realm = "My app!";
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(BasicAuthenticationDefaults.SchemaName, authBuilder =>
    {
        authBuilder.RequireClaim(ClaimTypes.NameIdentifier);
    });
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddBasicAuthentication();
});

var app = builder.Build();
```

## API Reference 📃

### IUserClaimsBuilder<T, K> interface

#### Declaration

```C#
public interface IUserClaimsBuilder<T, K> where T : new() where K : AuthenticationSchemeOptions, new()
```

#### Summary

Implementations of this interface are used to build `Claim` objects by the `BasicAuthenticationHandler<T, K>` class during basic authentication.

#### Type Parameters

 - `T`: Model to hold user data retrived by `GetUserData(string)` implementation.
 - `K`: Authentication scheme options used during basic authentication in `BasicAuthenticationHandler<T, K>` class.

#### Methods

##### `Task<T> GetUserData(string username)`

Retrieves data for some user.

##### `IList<Claim> BuildClaims(T userData, K authenticationSchemeOptions)`

Builds a list of `Claim` objects that are used to create the principal during basic authentication.

### IUserCredentialsValidator interface

#### Declaration

```C#
public interface IUserCredentialsValidator
```

#### Summary

Implementations of this interface are used by the `BasicAuthenticationHandler<T, K>` class to validate user credentials, *i.e.* to authenticate the user against its credentials.

#### Methods

##### `Task<bool> Validate(string username, string password)`

Validate user credentials for the basic authentication flow. Returns `true` if username and password combination is valid or `false` otherwise.

### BasicAuthenticationDefaults class

#### Declaration

```C#
public class BasicAuthenticationDefaults
```

#### Summary

Default values used by basic authentication.

#### Properties

#### `const string SchemaName`

Constant value `BasicAuthentication`.

### DefaultBasicAuthenticationSchemeOptions class

#### Declaration

```C#
public class DefaultBasicAuthenticationSchemeOptions : AuthenticationSchemeOptions
```

#### Summary

Default options for the `BasicAuthenticationHandler<T, K>`. Note that the `Realm` property is mandatory when configuring the authentication scheme options.

#### Properties

#### `string Realm`

### BasicAuthenticationHandler<T, K> class

#### Declaration

```C#
public class BasicAuthenticationHandler<T, K> : AuthenticationHandler<T> where T : AuthenticationSchemeOptions, new() where K : new()
```

#### Summary

Authentication handler for basic authentication flow.

#### Type parameters

 - `T`: Type of the authentication scheme options model used for authentication.
 - `K`: Type of the model that is used during user data retrieval after a successfull authentication.

### Extension Methods for AuthenticationBuilder class

#### AddBasicAuthentication<T, K>

Declaration:

```C#
public static AuthenticationBuilder AddBasicAuthentication<T, K>(this AuthenticationBuilder builder, Type referenceType, Action<K> configureOptions)
            where T : new()
            where K : AuthenticationSchemeOptions, new()
```

This method enables basic authentication using `BasicAuthenticationDefaults` and registers the needed service implementations in the ASP.NET DI container. `T` is the type for the data model used by the auth handler to retrieve user data after she has been successfully authenticated. This data model must be the same as the one used when implementing `IUserClaimsBuilder<T, K>`.

Parameters:

 - `Type referenceType`: A type in the assembly where the interfaces needed by the basic authentication library are implemented.
 - `Action<K> configureOptions`: A delegate to configure the schema options.

#### AddBasicAuthentication<T>

Declaration:

```C#
public static AuthenticationBuilder AddBasicAuthentication<T>(this AuthenticationBuilder builder,
            Type referenceType, Action<DefaultBasicAuthenticationSchemeOptions> configureOptions)
            where T : new()
```

This method enables basic authentication using `BasicAuthenticationDefaults` and `DefaultBasicAuthenticationSchemeOptions` as scheme options, and registers the needed service implementations in the ASP.NET DI container. `T` is the type for the data model used by the auth handler to retrieve user data after she has been successfully authenticated.

Parameters:

 - `Type referenceType`: A type in the assembly where the interfaces needed by the basic authentication library are implemented.
 - `Action<DefaultBasicAuthenticationSchemeOptions> configureOptions`: A delegate to configure the schema options.

 ### Extension methods for SwaggerGenOptions class

 #### AddBasicAuthentication

 Declarations:

 ```C#
 public static void AddBasicAuthentication(this SwaggerGenOptions options);

 public static void AddBasicAuthentication(this SwaggerGenOptions options, bool everyAuthorizeAttribute, string[]? policies = null, string[]? roles = null);
 ```

Configures basic authentication schema documentation in the generated OpenAPI document.

The first overload applies the basic authentication schema for every action containing at least one `Authorize` attribute. See other overloads to fine grain this behaviour.

The second overload behaviour is: if both `policies` and `roles` parameters are `null` and `everyAuthorizeAttribute` is `false`, then the method will apply the authentication schema to every action that contains an `Authorize` attribute that has `BasicAuthenticationDefaults.SchemaName` in its `AuthenticationSchemes` list. When the list parameters are not `null`, then filters are applied to the `Authorize` attributes.

Parameters:

 - `bool everyAuthorizeAttribute`: If `true` then the basic authentication schema will be applied to every action that contains at least one `Authorize` attribute.
 - `string[]? policies`: A list of policies that the operation will use to decide if the authentication schema should be applied to a given action that contains `Authorize` attribute(s). If a given action contains an `Authorize` attribute with a policy that is within this list then the schema will be applied to that method. Policies take precedence over roles if both are given. The default value is `null`.
 - `string[]? roles`: A list of roles that the operation will use to decide if the authentication schema should be applied to a given action that contains `Authorize` attribute(s). If a given action contains an `Authorize` attribute and at least one of the roles in the attribute roles array is within this list then the schema is applied to that method. Policies take precedence over roles if both are given. The default value is `null`.

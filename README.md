# Psalms.AspNetCore.Jwt

**Psalms.AspNetCore.Jwt** is a lightweight and extensible JWT authentication library for ASP.NET Core.
It provides a clean abstraction for generating **access tokens**, managing **refresh tokens**, and integrating with ASP.NET Core authentication middleware.

The library was designed to simplify JWT usage while keeping the implementation **secure, configurable, and adaptable to different storage strategies**.

---

# ✨ Features

* JWT Access Token generation
* Secure Refresh Token generation
* Refresh Token validation and rotation
* Hashing of refresh tokens before persistence
* Support for multiple refresh token storage strategies:

  * `IDistributedCache` (Redis, SQL Server cache, etc.)
  * Entity Framework repositories
  * Custom repositories via interface
* Configuration-driven token settings
* Simple ASP.NET Core authentication integration

---

# 📦 Installation

Add the package to your project:

```
dotnet add package Psalms.AspNetCore.Jwt
```

---

# ⚙️ Configuration

Add the JWT configuration to your `appsettings.json`.

```json
{
  "JWT": {
    "Key": "your-super-secret-key",
    "Issuer": "your-app",
    "Audience": "your-app-users",
    "Expires": "1" // in hours
  }
}
```

### Configuration Fields

| Key            | Description                    |
| -------------- | ------------------------------ |
| `JWT:Key`      | Secret key used to sign tokens |
| `JWT:Issuer`   | Token issuer                   |
| `JWT:Audience` | Token audience                 |
| `JWT:Expires`  | Token expiration time in hours |

---

# 🚀 Setup

Register the authentication service in `Program.cs`:

```csharp
builder.Services.AddPsalmsJwtAuthentication(builder.Configuration);
```

This will configure the ASP.NET Core authentication middleware using `JwtBearer`.

---

# 🔑 Generating Tokens

Inject `IPsalmsJwtTokenService` and generate tokens.

```csharp
public class AuthService
{
    private readonly IPsalmsJwtTokenService _jwt;

    public AuthService(IPsalmsJwtTokenService jwt)
    {
        _jwt = jwt;
    }

    public async Task<AuthResponse> Authenticate(IEnumerable<Claim> claims)
    {
        return await _jwt.GetAuthResponseAsync(claims);
    }
}
```

The response contains:

* Access Token
* Refresh Token

---

# 🔄 Refresh Token Flow

The refresh flow works as follows:

1. Client sends an expired access token and a refresh token.
2. The service validates the refresh token.
3. The old refresh token is removed.
4. A new access token and refresh token are issued.

```csharp
var newAuth = await jwtService.RefreshTokenAsync(authResponse);
```

---

# 🔐 Refresh Token Security

Refresh tokens are generated using a **cryptographically secure random generator**.

Before being stored, refresh tokens are hashed using the password hashing mechanism from ASP.NET Core Identity.

This ensures that:

* Tokens are **never stored in plaintext**
* Compromised storage does **not expose usable tokens**

---

# 🗄 Refresh Token Storage

The library supports multiple storage strategies.

## 1️⃣ Distributed Cache (Recommended for scalable systems)

You can use `IDistributedCache` to store refresh tokens in Redis or another distributed cache.

```csharp
var service = new PsalmsJwtTokenService(configuration, distributedCache);
```

This enables:

* horizontal scaling
* fast token lookup
* automatic expiration via cache policies

---

## 2️⃣ Entity Framework

If you prefer database persistence, use an EF repository.

```csharp
var service = new PsalmsJwtTokenService(configuration, refreshTokenContext);
```

---

## 3️⃣ Custom Repository

You can implement your own repository.

```csharp
public class CustomRepository : IPsalmsRefreshTokenRepository
{
    // custom implementation
}
```

Then inject it:

```csharp
var service = new PsalmsJwtTokenService(configuration, repository);
```

---

# 🧠 Architecture

The library separates responsibilities into clear components:

| Component                       | Responsibility                        |
| ------------------------------- | ------------------------------------- |
| `PsalmsJwtTokenService`         | Token generation and validation       |
| `IPsalmsRefreshTokenRepository` | Refresh token persistence abstraction |
| `DistributedCacheRepository`    | Distributed cache implementation      |
| `EFRefreshTokenRepository`      | Entity Framework implementation       |

This architecture allows the authentication system to adapt to different application requirements.

---

# 🔒 Token Validation

The library internally configures `TokenValidationParameters` using the configured JWT settings.

Validation includes:

* issuer
* audience
* signing key
* token lifetime

---

# 📚 Example Auth Response

```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI...",
  "refreshTokenModel": {
    "id": "8cdb92c2-5e10-4b8f-9d0e-bcd79f9c3a3c",
    "refreshToken": "generated-token",
    "expires": "2026-03-16T20:00:00Z"
  }
}
```

---

# 🛠 Extensibility

The library was designed to be extensible.

You can customize:

* refresh token storage
* authentication flows
* claim generation
* token validation

without modifying the core library.

---

# 📜 License

This project is part of the **Psalms** ecosystem of libraries.

Feel free to contribute and improve the project.
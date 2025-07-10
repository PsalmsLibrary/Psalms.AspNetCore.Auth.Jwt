## 📖 PsalmsJwtTokenService

A simple, testable, and configuration-driven **JWT token generator** for .NET applications.

Part of the **Psalms** library family — clean code, clear purpose.

---

# Hello?🤔

Have you ever stopped to think about how much time and boilerplate code you waste just to implement JWT authentication in your app?

And when you're dealing with **microservices**, that number only multiplies over and over again.

**Welcome to Psalms**, friend. You're not alone anymore. We've built this so you don't have to repeat yourself ever again.

---

### ✨ Features

- ✅ Minimal and dependency-light
- ✅ Configurable via `IConfiguration`
- ✅ Uses `JwtSecurityTokenHandler` from `System.IdentityModel.Tokens.Jwt`
- ✅ Supports custom claims
- ✅ Optional token expiration
- ✅ Fully unit tested with `xUnit`

---

### 📦 Installation

### NuGet

```bash

dotnet add package Psalms.AspNetCore.Auth.Jwt --version 1.0.0
```

Or via the NuGet Gallery (replace with actual link).

---

### ⚙️ Configuration

Add to your `appsettings.json`:

```json
{
  "JWT": {
    "Key": "your-secret-key",
    "Issuer": "your-api",
    "Audience": "your-client",
    "Expires": "2"
  }
}
```

| Key | Description |
| --- | --- |
| `Key` | Secret string used to sign the token (HMAC-SHA256) (Required) |
| `Issuer` | Identifier for your API or auth server (Optional) |
| `Audience` | Intended recipient of the token (Optional) |
| `Expires` | Token lifetime in hours (Optional) |

---

### 🛠 Example Usage

In your Program.cs:

```csharp
Services.AddPsalmsJwtAuthentication(Services.Configuration);
```

In your Controller or Service:

```csharp
var claims = new[]
{
    new Claim(ClaimTypes.NameIdentifier, "123"),
    new Claim(ClaimTypes.Name, "John")
};

var token = jwtService.GenerateToken(claims)
```

You can inject `PsalmsJwtTokenService` anywhere using the default .NET DI system:

```csharp
services.AddSingleton<PsalmsJwtTokenService>();
```

---

### ✅ Testing

This library is fully covered with unit tests using `xUnit`, testing:

- Valid token generation
- Expiration handling
- Missing/malformed configuration
- Custom and empty claims

---

### 🔮 Roadmap

Planned improvements for future versions:

- [ ]  Token validation
- [ ]  Refresh token support
- [ ]  Public interface for easier testing and DI
- [ ]  Support for other signing algorithms (e.g. RS256)
- [ ]  Automatic claims like `iat`, `jti`, etc.
- [ ]  Fluent token builder API

---

### 📫 Contributing

Contributions are welcome! Feel free to open issues, suggest features or improvements, or send pull requests.

---

### 📄 License

MIT License © 2025 [PsalmsLibrary]

---

*The eyes of the blind and the blindness of vision.*

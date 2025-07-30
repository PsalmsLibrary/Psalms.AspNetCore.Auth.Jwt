# 📖 PsalmsJwtTokenService

A simple, testable, and configuration-driven JWT token generator and refresh token manager for .NET applications.

Part of the **Psalms** library family — clean code, clear purpose.

---

## 👋 Hello? 🤔

Have you ever stopped to think about how much time and boilerplate code you waste just to implement JWT authentication in your app?

And when you're dealing with microservices... that number only multiplies.

Welcome to Psalms, friend. You're not alone anymore. We've built this so you don't have to repeat yourself ever again.

---

## ✨ Features

- ✅ Generate **JWT access tokens** based on claims
- 🔁 Generate and persist secure **refresh tokens**
- 🔄 Support for **token renewal** using a validated refresh token
- 🧩 Pluggable refresh token repositories:
    - EF Core (`IPsalmsRefreshTokenEFContext`)
    - In-Memory (`IMemoryCache`)
    - Custom (`IPsalmsRefreshTokenRepository`)
- 🔍 Extract `ClaimsPrincipal` from expired access tokens
- ⚙️ Configuration via `IConfiguration` (`appsettings.json`)
- ⚡ Minimal dependencies, uses `JwtSecurityTokenHandler` from `System.IdentityModel.Tokens.Jwt`
- 🔧 Optional token expiration
- 🧪 Fully unit tested with xUnit

---

## 📦 Installation

Via NuGet:

```bash
dotnet add package Psalms.AspNetCore.Auth.Jwt --version 2.0.0
```

## ⚙️ Configuration (`appsettings.json`)

```json

"JWT": {
  "Key": "your-secret-key",
  "Issuer": "your-api",
  "Audience": "your-client",
  "Expires": "8" // in hours (optional)
}
```

| Key | Description |
| --- | --- |
| `Key` | Secret string used to sign the token (Required) |
| `Issuer` | Identifier for your API or auth server (Optional) |
| `Audience` | Intended recipient of the token (Optional) |
| `Expires` | Token lifetime in hours (Optional) |

> ⚠️ Use a strong key (at least 32 characters) and never commit secrets to your repository.
> 

---

## 🛠 Example Usage with RefreshToken

### 🧱 1. Setup in `Program.cs` (In-Memory version)

```csharp
builder.Services.AddPsalmsJwtAuthentication(builder.Configuration);
builder.Services.AddMemoryCache();
builder.Services.AddScoped<PsalmsJwtTokenService>();
```

### 📲 2. Inject and use in your Controller

```csharp
[Route("api/[controller]")]
[ApiController]
public class AuthController(PsalmsJwtTokenService jwtService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Login()
    {
        // TODO: validate login credentials here

        var userClaims = new List<Claim>()
        {
            new(ClaimTypes.Name, "ExampleName")
        };

        return Ok(await jwtService.GetAuthResponseAsync(userClaims));
    }

    [HttpPost("Refresh")]
    public async Task<IActionResult> RefreshToken([FromBody] AuthResponse authResponse)
        => Ok(await jwtService.RefreshTokenAsync(authResponse));
}
```

---

### 🧩 3. Return tokens separately (optional)

```csharp
[HttpPost]
public async Task<IActionResult> Login()
{
    var claims = new List<Claim>()
    {
        new(ClaimTypes.Name, "ExampleName")
    };

    var accessToken = await jwtService.GenerateAccessTokenAsync(claims);
    var refreshToken = await jwtService.GenerateRefreshTokenAsync();

    return Ok(new AuthResponse
    {
        AccessToken = accessToken,
        RefreshToken = refreshToken
    })
```

> ⚠️ To refresh the token, you'll need to pass an AuthResponse object (which includes both the access and refresh tokens).
> 
> 
> ✅ Nothing stops you from generating access and refresh tokens independently if needed!
> 

---

## 🗃️ EF Core Integration

### 1. Register in `Program.cs`

```csharp
builder.Services.AddPsalmsJwtAuthentication(builder.Configuration);
builder.Services.AddDbContext<AppDbContext>(options => { /* your config */ });
builder.Services.AddScoped<IPsalmsRefreshTokenEFContext, AppDbContext>();
builder.Services.AddScoped<PsalmsJwtTokenService>();
```

### 2. Implement the interface in your DbContext

```csharp
public class AppDbContext : DbContext, IPsalmsRefreshTokenEFContext
{
    public DbSet<RefreshTokenModel> Refreshes { get; set; }

    public Task ConfirmChangesAsync()
        => SaveChangesAsync();
}
```

---

## 🧱 Custom Repository Integration

You can also implement your own refresh token storage:

```csharp
public class MyRepository : IPsalmsRefreshTokenRepository
{
    public Task DeleteRefreshTokenAsync(string refreshToken) 
    => throw new NotImplementedException();
    public Task<bool> RefreshTokenExistAsync(string refreshToken) 
    => throw new NotImplementedException();
    public Task SaveRefreshTokenAsync(RefreshTokenModel model) 
    => throw new NotImplementedException();
}
```

And register it in `Program.cs`:

```csharp
builder.Services.AddPsalmsJwtAuthentication(builder.Configuration);
builder.Services.AddScoped<IPsalmsRefreshTokenRepository, MyRepository>();
builder.Services.AddScoped<PsalmsJwtTokenService>();
```

---

## 📥 AuthResponse Example

```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "e10399da-3023-4e1f-bf2f-44f10d5b8bb9"
}
```

---

## 📫 Contributing

We welcome your contributions! Feel free to open an issue, suggest an improvement, or send a pull request.

---

## 📄 License

MIT License © 2025 [PsalmsLibrary]

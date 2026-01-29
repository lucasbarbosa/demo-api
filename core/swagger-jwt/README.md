# Demo API - JWT Authentication Version

**Secure REST API with JWT Bearer authentication and authorization policies**

## 📋 Quick Overview

This version enhances the base Swagger implementation with **enterprise-grade security** through JWT (JSON Web Token) authentication and authorization. It demonstrates production-ready authentication patterns including token validation, secure configuration management, and authorization fallback policies.

This version demonstrates all features from the Swagger version, plus:
- ✅ **JWT Bearer Authentication** - Stateless token-based authentication
- ✅ **Authorization Fallback Policy** - Secure by default (requires authentication)
- ✅ **Token Validation** - Issuer, audience, lifetime, and signature validation
- ✅ **Security Hardening** - Enhanced configuration validation
- ✅ **Swagger JWT Integration** - Interactive authentication in API explorer

## 🎯 Key Differences from Other Versions

### What This Version Adds (vs Swagger)

**New Configuration** (`Configuration/JwtConfig.cs`)
- JWT Bearer authentication setup
- Token validation parameters
- Authorization fallback policy

**Enhanced Security:**
- All endpoints require authentication by default
- Configurable token expiration
- Issuer and audience validation
- Minimum SecurityKey length enforcement (32 characters)
- Clock skew set to zero for strict expiration
- HTTPS metadata requirement

**Swagger Enhancements:**
- JWT security scheme in OpenAPI spec
- "Authorize" button in Swagger UI
- Bearer token input field
- Automatic token inclusion in requests

### Version Comparison

| Feature | Swagger | **This Version (JWT)** | Docker |
|---------|---------|------------------------|--------|
| Authentication | ❌ | ✅ JWT Bearer | ✅ JWT Bearer |
| Authorization Policy | ❌ | ✅ Fallback Policy | ✅ Fallback Policy |
| Token Validation | ❌ | ✅ Full validation | ✅ Full validation |
| Swagger Auth UI | ❌ | ✅ Interactive | ✅ Interactive |
| Containerization | ❌ | ❌ | ✅ Docker |

## 🚀 Running This Version

### Prerequisites
- .NET 10.0 SDK or later
- Visual Studio 2026 (v19.0+) or VS Code with C# extension
- A JWT secret key (32+ characters)

### Configuration Setup

Before running, configure JWT settings in `src/DemoApi.Api/appsettings.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "Authorization": {
    "SecurityKey": "your-super-secret-key-min-32-characters-long",
    "Sender": "DemoApi",
    "ValidOn": "https://localhost:5001",
    "ExpirationMinutes": 60
  }
}
```

**Configuration Parameters:**

| Parameter | Description | Example | Required |
|-----------|-------------|---------|----------|
| `SecurityKey` | JWT signing key (min 32 chars) | `"my-secret-key-..."` | ✅ Yes |
| `Sender` | Token issuer (iss claim) | `"DemoApi"` | ✅ Yes |
| `ValidOn` | Token audience (aud claim) | `"https://localhost:5001"` | ✅ Yes |
| `ExpirationMinutes` | Token lifetime | `60` | ✅ Yes |

**Security Validations:**
- ✅ Configuration section must exist
- ✅ SecurityKey cannot be null or empty
- ✅ SecurityKey minimum length: 32 characters
- ✅ All required fields validated at startup

### Quick Start

```powershell
# Navigate to this version's directory
cd d:\Projects\Git\lucasbarbosa\demo-api\core\swagger-jwt

# Restore dependencies
dotnet restore

# Build the solution
dotnet build

# Run tests
dotnet test --no-build

# Run the API
cd src\DemoApi.Api
dotnet run
```

### Access the API

- **HTTPS**: `https://localhost:5001`
- **HTTP**: `http://localhost:5000`
- **Swagger UI**: `https://localhost:5001/swagger`

## 🔐 Authentication Flow

### 1. Generate JWT Token

**Note**: This demo does not include a `/auth/login` endpoint. In production, you would:
1. Implement authentication endpoint
2. Validate user credentials
3. Generate token with claims
4. Return token to client

**Sample Token Generation (for testing):**

You can use online JWT tools like [jwt.io](https://jwt.io/) or create a token programmatically:

```csharp
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

var securityKey = new SymmetricSecurityKey(
    Encoding.UTF8.GetBytes("your-super-secret-key-min-32-characters-long"));

var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

var token = new JwtSecurityToken(
    issuer: "DemoApi",
    audience: "https://localhost:5001",
    claims: new[]
    {
        new Claim(ClaimTypes.Name, "TestUser"),
        new Claim(ClaimTypes.Role, "Admin")
    },
    expires: DateTime.UtcNow.AddMinutes(60),
    signingCredentials: credentials
);

string tokenString = new JwtSecurityTokenHandler().WriteToken(token);
Console.WriteLine(tokenString);
```

### 2. Using the Token with Swagger UI

1. Navigate to `https://localhost:5001/swagger`
2. Click the **"Authorize"** button (top right)
3. Enter: `Bearer {your-token-here}` (including the word "Bearer")
4. Click **"Authorize"**
5. All subsequent requests will include the token

### 3. Using the Token with cURL

```powershell
# Get all products (with authentication)
curl -H "Authorization: Bearer {your-token}" https://localhost:5001/api/v1/products

# Create a product (with authentication)
curl -X POST https://localhost:5001/api/v1/products `
  -H "Content-Type: application/json" `
  -H "Authorization: Bearer {your-token}" `
  -d '{"name": "Authenticated Product", "weight": 5.5}'
```

### 4. Response Without Token

If you attempt to access an endpoint without a token:

```json
{
  "type": "https://tools.ietf.org/html/rfc7235#section-3.1",
  "title": "Unauthorized",
  "status": 401,
  "traceId": "00-..."
}
```

### 5. Response With Invalid Token

If the token is expired, malformed, or has invalid signature:

```json
{
  "type": "https://tools.ietf.org/html/rfc7235#section-3.1",
  "title": "Unauthorized",
  "status": 401,
  "traceId": "00-..."
}
```

## 🛡️ Security Features

### JWT Configuration (`JwtConfig.cs`)

**Authentication Setup:**
```csharp
services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = true;  // Enforce HTTPS in production
    options.SaveToken = true;             // Save token in AuthenticationProperties
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,           // Validate 'iss' claim
        ValidateAudience = true,         // Validate 'aud' claim
        ValidateIssuerSigningKey = true, // Validate signature
        ValidateLifetime = true,         // Validate expiration
        ClockSkew = TimeSpan.Zero,       // No grace period for expiration
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidAudience = authorization.ValidOn,
        ValidIssuer = authorization.Sender
    };
});
```

### Authorization Fallback Policy

**Secure by Default:**
```csharp
services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});
```

This ensures:
- ✅ All endpoints require authentication unless explicitly marked `[AllowAnonymous]`
- ✅ Prevents accidental exposure of sensitive endpoints
- ✅ Follows principle of "secure by default"

### Configuration Validation

`JwtConfig.cs` validates configuration at startup:

```csharp
// 1. Configuration section exists
if (authorization is null)
    throw new InvalidOperationException("JWT Authorization settings are missing...");

// 2. SecurityKey is not empty
if (string.IsNullOrWhiteSpace(authorization.SecurityKey))
    throw new InvalidOperationException("JWT SecurityKey is required...");

// 3. SecurityKey minimum length
if (authorization.SecurityKey.Length < 32)
    throw new InvalidOperationException("JWT SecurityKey must be at least 32 characters...");
```

**Benefits:**
- Fails fast at startup (not at runtime)
- Clear error messages for missing configuration
- Enforces security best practices

## 🧪 Testing

### Run All Tests
```powershell
dotnet test
```

**Note**: The test suite uses `WebApplicationFactory` which may require modifications for JWT testing. Consider:
- Adding a test authentication handler
- Generating test tokens in integration tests
- Mocking JWT validation for unit tests

### Testing with Authentication

**Integration Test Example:**
```csharp
// Generate test token
var testToken = GenerateTestToken();

// Add to HTTP client
_client.DefaultRequestHeaders.Authorization = 
    new AuthenticationHeaderValue("Bearer", testToken);

// Make authenticated request
var response = await _client.GetAsync("/api/v1/products");
```

## 📚 API Endpoints

All endpoints from the [Swagger version](../swagger/README.md#api-endpoints) remain the same, but now **require authentication**.

### Authorization Headers

Every request must include:
```
Authorization: Bearer {jwt-token}
```

### Swagger UI Integration

The Swagger documentation now includes:
- 🔒 Padlock icon on secured endpoints (all endpoints)
- 🔑 "Authorize" button for token input
- Automatic authorization header injection
- Security scheme definition in OpenAPI spec

## ⚙️ Configuration Files

### appsettings.json (with JWT)

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "Authorization": {
    "SecurityKey": "production-key-should-be-from-env-variables",
    "Sender": "DemoApi",
    "ValidOn": "https://yourdomain.com",
    "ExpirationMinutes": 60
  }
}
```

### Environment Variables (Production)

**Never commit secrets to source control!** Use environment variables:

```powershell
$env:Authorization__SecurityKey = "your-production-secret-key"
$env:Authorization__Sender = "ProductionApi"
$env:Authorization__ValidOn = "https://api.production.com"
```

### Program.cs Changes

```csharp
builder.Services.AddJwtConfig(builder.Configuration);  // NEW

builder.Services.AddDependencyInjectionConfig();
builder.Services.AddApiConfig();

WebApplication app = builder.Build();

app.UseApiConfig(app.Environment);
app.UseJwtConfig();  // NEW - Adds UseAuthentication() + UseAuthorization()
app.MapControllers();
```

## 🏗️ Architecture Changes

### New Files

```
src/DemoApi.Api/
├── Configuration/
│   └── JwtConfig.cs          # NEW: JWT authentication configuration
└── Extensions/
    └── AuthorizationSettings.cs  # NEW: Configuration model (if separate)
```

### Modified Files

- `Program.cs` - Added JWT configuration
- `SwaggerConfig.cs` - Added JWT security definition (if modified)

### Middleware Pipeline Order

Critical: Authentication must come **before** authorization:

1. Exception Middleware
2. HTTPS Redirection
3. **Authentication** (`app.UseAuthentication()`)  ⬅️ NEW
4. **Authorization** (`app.UseAuthorization()`)    ⬅️ Changed order
5. Controllers
6. Swagger UI

## 📈 What's Next?

To see this implementation containerized for production deployment:

👉 **[Swagger + JWT + Docker Version](../swagger-jwt-docker/README.md)**

This adds:
- Multi-stage Docker builds
- Docker Compose orchestration
- Production container optimization
- Security hardening (non-root user)

---

**For comprehensive architectural documentation, see:**  
📘 [Root README](../../README.md)

**For the base implementation without authentication:**  
📘 [Swagger Version README](../swagger/README.md)
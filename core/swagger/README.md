# Demo API - Swagger Version

**Foundation implementation demonstrating Clean Architecture, comprehensive validation, and API documentation**

## 📋 Quick Overview

This is the **base implementation** of the Demo API project, showcasing enterprise-grade .NET development fundamentals without the complexity of authentication or containerization. It serves as the architectural foundation upon which the JWT and Docker versions build.

This version demonstrates:
- ✅ Clean Architecture with four distinct layers
- ✅ SOLID principles and design patterns (Repository, Notification, Builder)
- ✅ Comprehensive validation strategy (FluentValidation + custom filters)
- ✅ API versioning and OpenAPI documentation
- ✅ Structured logging with Serilog
- ✅ Complete testing suite (unit + integration tests)
- ✅ Test data builders with Bogus library
- ✅ Central package management

## 🎯 Key Differences from Other Versions

| Feature | This Version (Swagger) | JWT Version | Docker Version |
|---------|------------------------|-------------|----------------|
| **Authentication** | ❌ Public API | ✅ JWT Bearer | ✅ JWT Bearer |
| **Authorization** | ❌ None | ✅ Fallback Policy | ✅ Fallback Policy |
| **Deployment** | 🖥️ Local/IIS | 🖥️ Local/IIS | 🐳 Docker Container |
| **Security Hardening** | ⚠️ Basic (server header) | ✅ JWT + hardening | ✅ JWT + Docker + hardening |

**What this version adds:**
- Complete Clean Architecture implementation
- API versioning (URL, header, query string)
- Swagger/OpenAPI documentation
- Custom exception middleware
- FluentValidation integration
- Notification pattern for business errors
- Server header removal for security
- Serilog structured logging
- Unit tests with Moq and FluentAssertions
- Integration tests with WebApplicationFactory
- Test builders using Bogus

## 🚀 Running This Version

### Prerequisites
- .NET 10.0 SDK or later
- Visual Studio 2026 (v19.0+) or VS Code with C# extension

### Quick Start

```powershell
# Navigate to this version's directory
cd d:\Projects\Git\lucasbarbosa\demo-api\core\swagger

# Restore dependencies (Central Package Management)
dotnet restore

# Build the solution
dotnet build

# Run all tests
dotnet test --no-build

# Run the API
cd src\DemoApi.Api
dotnet run
```

### Access the API

Once running, the API will be available at:

- **HTTPS**: `https://localhost:5001`
- **HTTP**: `http://localhost:5000`
- **Swagger UI**: `https://localhost:5001/swagger`

### Example Requests

**Get all products:**
```powershell
curl https://localhost:5001/api/v1/products
```

**Create a product:**
```powershell
curl -X POST https://localhost:5001/api/v1/products `
  -H "Content-Type: application/json" `
  -d '{"name": "Test Product", "weight": 5.5}'
```

**Get product by ID:**
```powershell
curl https://localhost:5001/api/v1/products/1
```

**Update a product:**
```powershell
curl -X PUT https://localhost:5001/api/v1/products `
  -H "Content-Type: application/json" `
  -d '{"id": 1, "name": "Updated Product", "weight": 7.2}'
```

**Delete a product:**
```powershell
curl -X DELETE https://localhost:5001/api/v1/products/1
```

## 🧪 Testing

### Run All Tests
```powershell
dotnet test
```

### Run Unit Tests Only
```powershell
dotnet test --filter FullyQualifiedName~DemoApi.Application.Tests
```

### Run Integration Tests Only
```powershell
dotnet test --filter FullyQualifiedName~DemoApi.Api.Tests
```

### Test Coverage
```powershell
dotnet test --collect:"XPlat Code Coverage"
```

### Test Structure

**Unit Tests** (`tests/DemoApi.Application.Tests/`)
- Tests application service logic (`ProductAppService`)
- Uses Moq for repository mocking
- FluentAssertions for readable assertions
- Bogus for realistic test data

**Integration Tests** (`tests/DemoApi.Api.Tests/`)
- Tests complete HTTP request/response cycle
- WebApplicationFactory for in-memory test server
- Tests middleware pipeline and validation

**Test Builders** (`tests/DemoApi.Tests.Builders/`)
- `ProductBuilder` - Creates domain entities
- `ProductViewModelBuilder` - Creates DTOs
- Fluent API for flexible test setup

## 📚 API Endpoints

### Product Management

| Method | Endpoint | Description | Request | Response Codes |
|--------|----------|-------------|---------|----------------|
| GET | `/api/v1/products` | Get all products | - | 200, 400 |
| GET | `/api/v1/products/{id}` | Get product by ID | - | 200, 400, 404, 412 |
| POST | `/api/v1/products` | Create product | `ProductViewModel` | 201, 400, 412 |
| PUT | `/api/v1/products` | Update product | `ProductViewModel` | 204, 400, 404, 412 |
| DELETE | `/api/v1/products/{id}` | Delete product | - | 204, 400, 404, 412 |

### Request Model (`ProductViewModel`)

```json
{
  "id": 1,
  "name": "Product Name",
  "weight": 5.5
}
```

**Validation Rules:**
- `name`: Required, not empty (FluentValidation)
- `weight`: Must be greater than 0 (FluentValidation)

### Response Models

**Success Response:**
```json
{
  "success": true,
  "data": {
    "id": 1,
    "name": "Product Name",
    "weight": 5.5
  },
  "errors": []
}
```

**Error Response:**
```json
{
  "success": false,
  "data": null,
  "errors": [
    "Name is required",
    "Weight must be greater than 0"
  ]
}
```

## ⚙️ Configuration

### appsettings.json

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

### Serilog Configuration

Logging is configured in `src/DemoApi.Api/appsettings.json`:

- **Console Sink**: Colored output for development
- **File Sink**: `logs/log-{date}.txt`
- **Log Levels**: Info, Warning, Error
- **Enrichers**: LogContext, MachineName, ThreadId

### Kestrel Configuration

Server hardening in `HostConfig.cs`:
```csharp
builder.WebHost.ConfigureKestrel(options => options.AddServerHeader = false);
```
Removes the `Server` header to prevent information disclosure.

## 🏗️ Architecture Highlights

### Dependency Flow

```
DemoApi.Api
  └─> DemoApi.Application
        └─> DemoApi.Domain (interfaces)
  └─> DemoApi.Infra.Data
        └─> DemoApi.Domain (implements interfaces)
  └─> DemoApi.Infra.CrossCutting
```

### Key Classes

**Controllers** (`V1/Controllers/ProductController.cs`)
- Inherits from `MainApiController`
- Depends on `IProductAppService` and `INotificatorHandler`
- Returns standardized responses via `CustomResponse()`

**Services** (`Application/Services/ProductAppService.cs`)
- Orchestrates business operations
- Uses `INotificatorHandler` for error collection
- Depends on `IProductRepository` abstraction

**Repositories** (`Infra/Repositories/ProductRepository.cs`)
- Implements `IProductRepository` from Domain
- Current implementation: In-memory storage
- Easily swappable for EF Core, Dapper, etc.

**Notification Handler** (`Domain/Handlers/NotificatorHandler.cs`)
- Collects business validation errors
- Avoids exception-based flow control
- Queried by controllers for error reporting

### Middleware Pipeline

Order of execution in `Program.cs`:

1. **Exception Middleware** (`ExceptionMiddleware`) - Global exception handling
2. **HTTPS Redirection** - Force HTTPS in production
3. **Authorization** - (No-op in this version)
4. **Controllers** - Route to controller actions
5. **Swagger UI** - Interactive API documentation

### Validation Pipeline

1. **FluentValidation** (`FluentValidationFilter`)
   - Runs first, uses `ProductValidator`
   - Returns 400 with validation errors

2. **ModelState Validation** (`ModelValidationFilter`)
   - Fallback for framework validation
   - Suppressed default behavior for custom responses

3. **Business Validation** (In `ProductAppService`)
   - Domain-specific rules
   - Uses `INotificatorHandler`
   - Returns 412 (Precondition Failed) for business errors

## 📦 Dependencies

This version uses the following key packages (managed in `Directory.Packages.props`):

**Production:**
- `Asp.Versioning.Mvc` (8.1.1) - API versioning
- `Swashbuckle.AspNetCore` (6.6.2) - OpenAPI/Swagger
- `FluentValidation` (12.1.1) - Input validation
- `AutoMapper` (16.0.0) - Object mapping
- `Serilog.AspNetCore` (10.0.0) - Structured logging

**Testing:**
- `xUnit` (2.9.3) - Test framework
- `Moq` (4.20.72) - Mocking library
- `FluentAssertions` (8.8.0) - Assertion library
- `Bogus` (35.6.5) - Fake data generation
- `Microsoft.AspNetCore.Mvc.Testing` (10.0.2) - Integration testing

## 📈 What's Next?

To see this implementation enhanced with authentication and authorization:

👉 **[Swagger + JWT Version](../swagger-jwt/README.md)**

This adds:
- JWT Bearer authentication
- Authorization with FallbackPolicy (secure by default)
- Swagger JWT security scheme
- Token generation and validation

---

**For comprehensive architectural documentation, see:**  
📘 [Root README](../../README.md)
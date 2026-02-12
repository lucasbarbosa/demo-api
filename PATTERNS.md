# 🏗️ Architecture Patterns & Best Practices

> **Comprehensive catalog of design patterns, architectural decisions, and engineering best practices implemented in Demo API**

This document serves as a technical deep-dive into the patterns and practices applied throughout the project. Each pattern is documented with its location in the codebase, implementation details, and architectural rationale.

---

## 📑 Table of Contents

1. [Architectural Patterns](#architectural-patterns)
2. [Design Patterns](#design-patterns)
3. [API Design Patterns](#api-design-patterns)
4. [Validation Patterns](#validation-patterns)
5. [Testing Patterns](#testing-patterns)
6. [Security Patterns](#security-patterns)
7. [Configuration Patterns](#configuration-patterns)
8. [Modern C# Patterns](#modern-c-patterns)
9. [Best Practices](#best-practices)

---

## 🏛️ Architectural Patterns

### 1. Clean Architecture (Onion Architecture)

**Location:** Entire solution structure

**Description:** Implementation of Clean Architecture with strict dependency rules flowing inward toward the Domain layer.

**Layer Structure:**
```
┌─────────────────────────────────────┐
│  API Layer (DemoApi.Api)            │  ← Frameworks, Controllers, Middleware
│  ┌───────────────────────────────┐  │
│  │  Application (DemoApi.App)    │  │  ← Use Cases, Services, DTOs
│  │  ┌─────────────────────────┐  │  │
│  │  │  Domain (DemoApi.Domain)│  │  │  ← Entities, Interfaces, Business Rules
│  │  └─────────────────────────┘  │  │
│  └───────────────────────────────┘  │
│  Infrastructure (DemoApi.Infra.*)   │  ← Data Access, External Services
└─────────────────────────────────────┘
```

**Dependency Rules:**
- ✅ API → Application → Domain
- ✅ Infrastructure → Domain (through interfaces)
- ❌ Domain → Never depends on outer layers
- ❌ Application → Never depends on Infrastructure

**Evidence in Code:**
- `DemoApi.Domain` has zero external dependencies
- `IProductRepository` defined in Domain, implemented in Infrastructure
- `Program.cs` serves as the Composition Root

**Benefits:**
- **Testability** - Domain logic testable without infrastructure
- **Maintainability** - Clear separation of concerns
- **Flexibility** - Easy to swap infrastructure implementations
- **Independence** - Business rules independent of frameworks

---

### 2. Repository Pattern

**Location:** `DemoApi.Domain/Interfaces/IProductRepository.cs`, `DemoApi.Infra/Repositories/ProductRepository.cs`

**Description:** Abstraction layer between domain logic and data access, providing collection-like interface for domain entities.

**Interface Contract:**
```csharp
public interface IProductRepository
{
    Task<IList<Product>> GetAll();
    Task<Product?> GetById(uint id);
    Task<Product?> GetByName(string name);
    Task<Product> Create(Product product);
    Task<bool> Update(Product product);
    Task<bool> DeleteById(uint id);
}
```

**Implementation Strategy:**
- Current: In-memory storage (demo purposes)
- Future-ready: Swap with EF Core, Dapper, or any ORM without changing Application layer

**Key Principle:** Dependency Inversion - high-level modules (Application) depend on abstractions (IProductRepository), not concrete implementations.

---

### 3. Notification Pattern (Domain Notifications)

**Location:** `DemoApi.Domain/Handlers/NotificatorHandler.cs`, `DemoApi.Domain/Interfaces/INotificatorHandler.cs`

**Description:** Collects business validation errors without throwing exceptions, enabling graceful error aggregation and multiple error messages in a single response.

**Implementation:**
```csharp
public interface INotificatorHandler
{
    bool HasErrors();
    void AddError(string error);
    void AddErrors(params IEnumerable<string> errors);
    List<Notification> GetErrors();
}
```

**Usage Pattern:**
```csharp
// In Application Service
if (await _productRepository.GetByName(product.Name) is not null)
{
    _notificator.AddError($"Product ({product.Name}) is already registered");
    return null;
}
```

**Why Not Exceptions:**
- ✅ **Performance** - No stack unwinding overhead
- ✅ **Expected Flow** - Business validations are not exceptional cases
- ✅ **Multiple Errors** - Collect all validation errors before returning
- ✅ **Cleaner Code** - No try-catch blocks for business logic

**Lifetime:** Scoped - errors are request-specific

**Related Patterns:** Railway-Oriented Programming, Result Pattern

---

### 4. Dependency Injection (Native .NET Container)

**Location:** `DemoApi.Api/Configuration/DependencyInjectionConfig.cs`

**Description:** Constructor-based dependency injection using .NET's built-in DI container, avoiding Service Locator anti-pattern.

**Registration Strategy:**
```csharp
// Applications
services.AddScoped<IProductAppService, ProductAppService>();

// Repositories
services.AddScoped<IProductRepository, ProductRepository>();

// Domain Handlers
services.AddScoped<INotificatorHandler, NotificatorHandler>();

// Validators (auto-discovery)
services.AddValidatorsFromAssemblyContaining<ProductValidator>();
```

**Validation Configuration:**
- `ValidateScopes` - Ensures scoped services not resolved from singleton
- `ValidateOnBuild` - Catches DI configuration errors at startup

**Anti-Pattern Avoided:** No `IServiceProvider.GetService()` in production code (only in `FluentValidationFilter` where dynamic resolution is required)

---

## 🎨 Design Patterns

### 5. Builder Pattern (Test Data Builders)

**Location:** `DemoApi.Tests.Builders/Products/ProductBuilder.cs`, `ProductViewModelBuilder.cs`

**Description:** Fluent interface for constructing test objects with realistic default values.

**Implementation:**
```csharp
public class ProductBuilder
{
    private uint _id = 0;
    private string _name;
    private double _weight;
    private static readonly Faker _faker = new();

    public ProductBuilder()
    {
        _name = _faker.Commerce.ProductName();
        _weight = Math.Round(_faker.Random.Double(0.1, 10.0), 2);
    }

    public ProductBuilder WithName(string name) { _name = name; return this; }
    public ProductBuilder WithWeight(double weight) { _weight = weight; return this; }
    public Product Build() => new Product { Id = _id, Name = _name, Weight = _weight };
    public static ProductBuilder New() => new();
}
```

**Usage:**
```csharp
Product product = ProductBuilder.New()
    .WithName("Custom Product")
    .WithWeight(5.5)
    .Build();
```

**Advanced Features:**
- **Object Mother Pattern** - Realistic defaults via Bogus library
- **Static Factory Method** - `New()` for fluent instantiation
- **Immutable Build** - Each `With*` returns new instance (optional)

**Benefits:**
- Readable test setup
- Realistic test data reveals edge cases
- Easy to create variations of test objects

---

### 6. Template Method Pattern (Base Controller)

**Location:** `DemoApi.Api/Controllers/MainApiController.cs`

**Description:** Defines skeleton of API response handling in base controller, allowing derived controllers to reuse response logic.

**Template Methods:**
```csharp
protected ActionResult CustomResponse(object? result = null)
protected ActionResult CustomResponse(HttpStatusCode statusCode, object? result)
protected ActionResult CustomResponse(ModelStateDictionary modelState)
protected ActionResult CustomResponseCreate(object? result)
```

**Derived Controller Usage:**
```csharp
public class ProductController : MainApiController
{
    public async Task<IActionResult> GetById(uint id)
    {
        ProductViewModel? product = await _productApplication.GetById(id);
        return CustomResponse(product); // Template method handles all response logic
    }
}
```

**Encapsulated Logic:**
- Notification error checking
- HTTP status code selection
- Response envelope creation
- Consistent error formatting

**Benefits:**
- **DRY** - No repeated response logic
- **Consistency** - All endpoints return same structure
- **Maintainability** - Change response format in one place

---

### 7. Chain of Responsibility (Validation Pipeline)

**Location:** `DemoApi.Api/Configuration/ApiConfig.cs`, `Extensions/FluentValidationFilter.cs`, `Extensions/ModelValidationFilter.cs`

**Description:** Multiple validation filters executed in sequence before reaching controller action.

**Pipeline Order:**
```
HTTP Request
    ↓
1. ModelValidationFilter (Data Annotations - fallback)
    ↓
2. FluentValidationFilter (FluentValidation rules)
    ↓
3. Controller Action (ModelState check)
    ↓
4. Application Service (Business validation via NotificatorHandler)
    ↓
Response
```

**Filter Registration:**
```csharp
services.AddControllers(options =>
{
    options.Filters.Add<ModelValidationFilter>();
    options.Filters.Add<FluentValidationFilter>();
});
```

**Separation of Concerns:**
- **Framework Validation** → Filters (ModelState)
- **Business Validation** → Application Service (NotificatorHandler)
- **Data Validation** → Repository (implicit, e.g., unique constraints)

**Related Patterns:** Pipeline Pattern, Decorator Pattern

---

### 8. Middleware Pattern (Global Exception Handler)

**Location:** `DemoApi.Api/Extensions/ExceptionMiddleware.cs`

**Description:** ASP.NET Core middleware that catches unhandled exceptions and converts them to standardized API responses.

**Implementation:**
```csharp
public class ExceptionMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext httpContext, ILogger logger, INotificatorHandler notificator)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(httpContext, ex, logger, notificator);
        }
    }
}
```

**Exception Handling Strategy:**
- ✅ **Log exception** via custom ILogger
- ✅ **Combine** NotificatorHandler errors with exception message
- ✅ **Return 500** with ResponseViewModel structure
- ✅ **Prevent information leakage** (no stack traces in production)

**Middleware Registration:**
```csharp
app.UseMiddleware<ExceptionMiddleware>();
```

**Benefits:**
- Centralized error handling
- Consistent error responses
- Automatic logging
- Clean controller code (no try-catch blocks)

---

### 9. Static Factory Method Pattern

**Location:** `ProductBuilder.cs` (`New()` method), `ProductRepository.cs` (`NewId()` method)

**Description:** Static methods that encapsulate object creation logic.

**Examples:**

**Test Builder Factory:**
```csharp
public static ProductBuilder New() => new();

// Usage
var product = ProductBuilder.New().WithName("Test").Build();
```

**ID Generation Factory:**
```csharp
protected static uint NewId()
{
    if (_memoryProducts.Count == 0) return 1;
    var maxId = _memoryProducts.Max(u => u.Id);
    return maxId + 1;
}
```

**Benefits:**
- Encapsulates creation complexity
- Enables fluent API design
- Allows evolution without breaking public API

---

## 🌐 API Design Patterns

### 10. API Response Envelope Pattern (Response Wrapper)

**Location:** `DemoApi.Application/Models/ResponseViewModel.cs`, `MainApiController.cs`

**Description:** Wraps all API responses in a consistent envelope structure, regardless of HTTP status code.

**Envelope Structure:**
```json
{
    "success": true,
    "data": { /* payload */ },
    "errors": []
}
```

**Implementation:**
```csharp
public class ResponseViewModel : BaseViewModel
{
    public bool Success { get; set; }
    public object? Data { get; set; }
    public IList<string> Errors { get; set; } = [];
}
```

**Benefits:**
- ✅ **Predictable Structure** - Clients always receive same format
- ✅ **Multiple Errors** - Can return array of validation messages
- ✅ **Business vs Technical Success** - `success: false` with `200 OK` for business failures
- ✅ **Frontend Simplification** - Single parsing logic for all responses

**Specialized Responses:**
```csharp
public class ProductResponse : ResponseViewModel
{
    public new ProductViewModel? Data { get; set; } // Type-safe data property
}
```

**Related Patterns:** Envelope Pattern, Result Pattern, DTO Pattern

---

### 11. Semantic HTTP Status Code Pattern

**Location:** `MainApiController.cs`, `ProductController.cs`

**Description:** Precise, semantic use of HTTP status codes to communicate operation results.

**Status Code Strategy:**

| Code | Meaning | Usage in Project |
|------|---------|------------------|
| **200 OK** | Success | GET requests, successful queries |
| **201 Created** | Resource created | POST requests with new resource |
| **204 No Content** | Success, no body | PUT/DELETE successful operations |
| **400 Bad Request** | Invalid input | FluentValidation failures, malformed requests |
| **404 Not Found** | Resource missing | GET/PUT/DELETE when resource doesn't exist |
| **412 Precondition Failed** | **Business rule violation** | Duplicate product, business logic failures |
| **500 Internal Server Error** | Unexpected error | Unhandled exceptions |

**Key Distinction - 412 Precondition Failed:**
```csharp
// 400 Bad Request - Input validation
if (ModelState.IsValid is false) return CustomResponse(ModelState);

// 412 Precondition Failed - Business rule
if (await _productRepository.GetByName(product.Name) is not null)
{
    _notificator.AddError($"Product ({product.Name}) is already registered");
    return CustomResponse(HttpStatusCode.PreconditionFailed, ...);
}
```

**Why 412 for Business Validation:**
- Differentiates from malformed input (400)
- Indicates request was well-formed but violated business rules
- Semantically correct per RFC 7232

---

### 12. API Versioning Multi-Reader Pattern

**Location:** `DemoApi.Api/Configuration/ApiConfig.cs`

**Description:** Supports multiple methods for clients to specify API version, enabling gradual migration.

**Supported Version Readers:**
```csharp
options.ApiVersionReader = ApiVersionReader.Combine(
    new UrlSegmentApiVersionReader(),        // /api/v1/products
    new HeaderApiVersionReader("X-Api-Version"),  // Header: X-Api-Version: 1.0
    new QueryStringApiVersionReader("api-version") // ?api-version=1.0
);
```

**Configuration:**
```csharp
services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ReportApiVersions = true; // Returns supported versions in response headers
});
```

**Benefits:**
- Backward compatibility
- Client flexibility
- Gradual migration path
- Version discovery via headers

---

### 13. RESTful Resource Naming Convention

**Location:** All controllers

**Description:** Follows REST best practices for resource naming and HTTP verb mapping.

**Resource Structure:**
```
GET    /api/v1/products      → List all products
GET    /api/v1/products/{id} → Get single product
POST   /api/v1/products      → Create product
PUT    /api/v1/products      → Update product (full replacement)
DELETE /api/v1/products/{id} → Delete product
```

**Conventions Applied:**
- ✅ Plural nouns for collections (`products`, not `product`)
- ✅ HTTP verbs indicate action (no verbs in URLs)
- ✅ Hierarchical structure (`/api/v{version}/resource`)
- ✅ ID in URL for single resource operations

---

## ✅ Validation Patterns

### 14. Multi-Layer Validation Strategy

**Location:** Entire application stack

**Description:** Validation implemented at multiple layers with different responsibilities.

**Validation Layers:**

**Layer 1 - Framework Validation (ModelState)**
- Location: `ModelValidationFilter.cs`
- Scope: Data Annotations (fallback)
- Example: `[Required]`, `[Range]`

**Layer 2 - Input Validation (FluentValidation)**
- Location: `ProductValidator.cs`, `FluentValidationFilter.cs`
- Scope: Complex input rules
- Example:
  ```csharp
  RuleFor(p => p.Name).NotEmpty().WithMessage("Name is required");
  RuleFor(p => p.Weight).GreaterThan(0).WithMessage("Weight must be greater than 0");
  ```

**Layer 3 - Business Validation (Domain Logic)**
- Location: `ProductAppService.cs`
- Scope: Business rules requiring data access
- Example:
  ```csharp
  if (await _productRepository.GetByName(product.Name) is not null)
      _notificator.AddError($"Product ({product.Name}) is already registered");
  ```

**Fail-Fast Strategy:**
```csharp
// Controller checks ModelState first
if (ModelState.IsValid is false) return CustomResponse(ModelState);

// Service performs business validation
if (await _productRepository.GetByName(product.Name) is not null)
{
    _notificator.AddError("Duplicate product");
    return null; // Early return
}
```

**Benefits:**
- Clear separation of concerns
- Early validation failures (performance)
- Comprehensive error collection
- Testable at each layer

---

### 15. Validator Auto-Discovery Pattern

**Location:** `DependencyInjectionConfig.cs`

**Description:** Automatic registration of all FluentValidation validators from assembly.

**Implementation:**
```csharp
services.AddValidatorsFromAssemblyContaining<ProductValidator>();
```

**How It Works:**
- Scans assembly for classes implementing `IValidator<T>`
- Registers each validator with DI container
- `FluentValidationFilter` resolves validators dynamically at runtime

**Benefits:**
- No manual registration per validator
- Scales automatically as validators are added
- Convention over configuration

---

## 🧪 Testing Patterns

### 16. WebApplicationFactory Pattern (Integration Testing)

**Location:** `DemoApi.Api.Tests/Common/Factories/CustomWebApplicationFactory.cs`

**Description:** In-memory HTTP server for end-to-end integration testing without external dependencies.

**Implementation:**
```csharp
public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    // Minimal setup - inherits full application configuration
}
```

**Test Example:**
```csharp
public class ProductApiTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ProductApiTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAll_ShouldReturn200_WithProductList()
    {
        var response = await _client.GetAsync("/api/v1/products");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
```

**What's Tested:**
- ✅ Full HTTP request/response cycle
- ✅ Middleware pipeline
- ✅ Routing
- ✅ Model binding
- ✅ Validation filters
- ✅ Controller logic
- ✅ Serialization

**Key Enabler:**
```csharp
// Program.cs
public partial class Program { } // Allows test project to reference Program
```

**Benefits:**
- Tests actual HTTP behavior
- No mocking of framework components
- Fast (in-memory, no network)
- Isolated per test class

---

### 17. Arrange-Act-Assert Pattern (AAA)

**Location:** All test files

**Description:** Standard test structure for clarity and consistency.

**Example:**
```csharp
[Fact]
public async Task Create_ShouldReturnProduct_WhenRepositoryCreatesSuccessfully()
{
    // Arrange - Setup mocks and test data
    var product = ProductBuilder.New().Build();
    _mockRepository.Setup(x => x.Create(It.IsAny<Product>()))
        .ReturnsAsync(product);

    // Act - Execute the method under test
    var result = await _service.Create(_mapper.Map<ProductViewModel>(product));

    // Assert - Verify results
    result.Should().NotBeNull();
    result!.Name.Should().Be(product.Name);
    _mockRepository.Verify(x => x.Create(It.IsAny<Product>()), Times.Once);
}
```

**Benefits:**
- Clear test structure
- Easy to understand intent
- Consistent across all tests

---

### 18. Realistic Test Data Pattern (Bogus Integration)

**Location:** `ProductBuilder.cs`

**Description:** Generates realistic fake data for tests instead of hardcoded values.

**Implementation:**
```csharp
private static readonly Faker _faker = new();

public ProductBuilder()
{
    _name = _faker.Commerce.ProductName();     // "Gorgeous Granite Gloves"
    _weight = Math.Round(_faker.Random.Double(0.1, 10.0), 2); // 7.42
}
```

**Why Realistic Data:**
- ❌ Avoid: `"Test1"`, `"Test2"`, `123`
- ✅ Use: `"Ergonomic Steel Keyboard"`, `3.47`

**Benefits:**
- Reveals bugs with special characters
- Tests string length edge cases
- Simulates production-like data
- More confidence in test coverage

---

## 🔒 Security Patterns

### 19. Security Header Suppression Pattern

**Location:** `DemoApi.Api/Configuration/HostConfig.cs`

**Description:** Removes server identification headers to prevent information disclosure.

**Implementation:**
```csharp
public static WebApplicationBuilder AddHostConfig(this WebApplicationBuilder builder)
{
    builder.WebHost.ConfigureKestrel(options => options.AddServerHeader = false);
    return builder;
}
```

**What It Prevents:**
```
// Before
Server: Kestrel

// After
(header removed)
```

**Security Principle:** Defense in Depth - reduces attack surface by hiding server technology.

---

### 20. JWT Authentication Pattern (Versions 2 & 3)

**Location:** `DemoApi.Api/Configuration/JwtConfig.cs` (swagger-jwt versions)

**Description:** Bearer token authentication with comprehensive validation.

**Configuration Validations:**
```csharp
// Security key minimum length
if (securityKey.Length < 32)
    throw new InvalidOperationException("SecurityKey must be at least 32 characters");

// Issuer and audience validation
ValidateIssuer = true,
ValidateAudience = true,
ValidateLifetime = true,
ValidateIssuerSigningKey = true,

// Strict expiration (no clock skew)
ClockSkew = TimeSpan.Zero
```

**Fallback Authorization Policy:**
```csharp
services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});
```

**Effect:** All endpoints require authentication by default unless explicitly marked with `[AllowAnonymous]`.

---

### 21. Docker Security Hardening (Version 3)

**Location:** `docker/Dockerfile`

**Description:** Multi-stage build with security best practices.

**Security Measures:**
- ✅ **Multi-stage build** - Minimal final image (no SDK tools)
- ✅ **Non-root user** - `USER app` (default in .NET 10)
- ✅ **Minimal base image** - `mcr.microsoft.com/dotnet/aspnet` (runtime only)
- ✅ **Explicit port exposure** - Only necessary ports (8080, 8081)
- ✅ **No secrets in image** - Configuration via environment variables

**Dockerfile Structure:**
```dockerfile
# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

# Stage 2: Publish
FROM build AS publish

# Stage 3: Final (minimal)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
USER app  # Non-root user
```

---

## ⚙️ Configuration Patterns

### 22. Extension Methods Pattern (Modular Configuration)

**Location:** All `*Config.cs` files

**Description:** Encapsulates configuration logic in extension methods for modularity and composability.

**Configuration Modules:**
```csharp
// Program.cs - Composition
builder.AddHostConfig();
builder.AddSerilogConfiguration();
builder.Services.AddDependencyInjectionConfig();
builder.Services.AddApiConfig();

app.UseApiConfig(app.Environment);
app.UseSwaggerConfig();
```

**Example Module:**
```csharp
public static class ApiConfig
{
    public static IServiceCollection AddApiConfig(this IServiceCollection services)
    {
        services.AddControllers(/* ... */);
        services.AddApiVersioning(/* ... */);
        return services;
    }

    public static IApplicationBuilder UseApiConfig(this IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseMiddleware<ExceptionMiddleware>();
        app.UseHttpsRedirection();
        return app;
    }
}
```

**Benefits:**
- **High Cohesion** - Related configuration grouped together
- **Low Coupling** - Modules independent of each other
- **Testability** - Easy to test configuration in isolation
- **Readability** - `Program.cs` reads like a DSL

**Related Patterns:** Extension Object Pattern, Fluent Interface

---

### 23. Configuration Suppression Pattern

**Location:** `ApiConfig.cs`

**Description:** Disables default ASP.NET Core behaviors to implement custom logic.

**Example:**
```csharp
services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});
```

**Why Suppress:**
- Default behavior returns 400 immediately on ModelState errors
- Custom behavior integrates with NotificatorHandler for consistent error responses
- Allows collecting multiple error sources before responding

---

### 24. Two-Stage Serilog Initialization Pattern

**Location:** `SerilogConfig.cs`

**Description:** Initializes Serilog in two stages to capture startup errors.

**Stage 1 - Bootstrap Logger:**
```csharp
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateBootstrapLogger();
```

**Stage 2 - Full Configuration:**
```csharp
builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext());
```

**Benefits:**
- Captures errors during application startup
- Full configuration from `appsettings.json`
- No lost log entries

---

## 💻 Modern C# Patterns

### 25. Primary Constructor Pattern (C# 12)

**Location:** Controllers, Middleware, Filters

**Description:** Simplified constructor syntax with inline field initialization.

**Traditional Approach:**
```csharp
public class ProductController : MainApiController
{
    private readonly IProductAppService _productApplication;

    public ProductController(INotificatorHandler notificator, IProductAppService productApplication)
        : base(notificator)
    {
        _productApplication = productApplication;
    }
}
```

**Primary Constructor:**
```csharp
public class ProductController(
    INotificatorHandler notificator,
    IProductAppService productApplication) : MainApiController(notificator)
{
    private readonly IProductAppService _productApplication = productApplication;
}
```

**Benefits:**
- Less boilerplate
- Clear parameter-to-field mapping
- Maintains explicit field declaration for clarity

---

### 26. Collection Expressions (C# 12)

**Location:** Throughout codebase

**Description:** Modern syntax for collection initialization.

**Examples:**
```csharp
// Old
public IList<string> Errors { get; set; } = new List<string>();
private static readonly List<Product> _memoryProducts = new List<Product>();

// New (C# 12)
public IList<string> Errors { get; set; } = [];
private static readonly List<Product> _memoryProducts = [];
```

**Benefits:**
- Cleaner syntax
- Consistent with modern C# style
- Better performance (compiler optimizations)

---

### 27. Nullable Reference Types (C# 8+)

**Location:** Entire codebase

**Description:** Explicit null handling to prevent `NullReferenceException`.

**Configuration:**
```xml
<Nullable>enable</Nullable>
```

**Examples:**
```csharp
// Required properties
public required string Name { get; set; }

// Nullable return types
public async Task<Product?> GetById(uint id)

// Null checks with pattern matching
if (product is null)
{
    _notificator.AddError("Product was not found");
    return null;
}

// Non-null assertion (when guaranteed)
var name = product!.Name;
```

**Benefits:**
- Compile-time null safety
- Explicit intent (nullable vs non-nullable)
- Fewer runtime null exceptions

---

### 28. Pattern Matching (C# 9+)

**Location:** Controllers, Services

**Description:** Modern pattern matching for cleaner conditional logic.

**Examples:**
```csharp
// Null check with negation
if (result is null or false)
{
    return NotFound(/* ... */);
}

// Type pattern with null check
if (await _productRepository.GetByName(product.Name) is not null)
{
    _notificator.AddError("Product already exists");
}
```

**Benefits:**
- More expressive than traditional `==` checks
- Combines multiple conditions elegantly
- Better readability

---

### 29. Record Types (C# 9+)

**Location:** Domain entities, DTOs (where applicable)

**Description:** Immutable data structures with value-based equality.

**Potential Usage:**
```csharp
// For DTOs/ViewModels
public record ProductViewModel(uint Id, string Name, double Weight);

// For Domain Events
public record ProductCreatedEvent(uint ProductId, string Name, DateTime CreatedAt);
```

**Note:** Current implementation uses classes for flexibility, but records are suitable for immutable DTOs.

---

## 🎯 Best Practices

### 30. Async All the Way Down

**Location:** Entire codebase

**Description:** Consistent use of async/await throughout the stack, even for in-memory operations.

**Examples:**
```csharp
// Repository (even in-memory)
public async Task<Product?> GetById(uint id)
{
    return await Task.FromResult(_memoryProducts.FirstOrDefault(p => p.Id == id));
}

// Service
public async Task<ProductViewModel?> GetById(uint id)
{
    return _mapper.Map<ProductViewModel>(await _productRepository.GetById(id));
}

// Controller
public async Task<IActionResult> GetById(uint id)
{
    ProductViewModel? product = await _productApplication.GetById(id);
    return CustomResponse(product);
}
```

**Why Async for In-Memory:**
- ✅ **Future-proof** - Ready for real I/O operations
- ✅ **Consistent API** - Same signatures when swapping implementations
- ✅ **No blocking** - No `.Result` or `.Wait()` calls

**Anti-Pattern Avoided:**
```csharp
// ❌ Never do this
var result = asyncMethod().Result; // Deadlock risk
var result = asyncMethod().GetAwaiter().GetResult(); // Blocks thread
```

---

### 31. Zero Warnings Policy

**Location:** All `.csproj` files

**Description:** Treats all compiler warnings as errors to enforce code quality.

**Configuration:**
```xml
<TreatWarningsAsErrors>true</TreatWarningsAsErrors>
```

**Impact:**
- ❌ Build fails on any warning
- ✅ Forces resolution of potential issues
- ✅ Prevents technical debt accumulation
- ✅ Enforces best practices (e.g., unused variables, missing XML docs)

---

### 32. Central Package Management

**Location:** `Directory.Packages.props`

**Description:** Centralized NuGet package version management across all projects.

**Structure:**
```xml
<Project>
  <PropertyGroup>
    <ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>
  </PropertyGroup>
  <ItemGroup>
    <PackageVersion Include="AutoMapper" Version="16.0.0" />
    <PackageVersion Include="FluentValidation" Version="12.1.1" />
    <!-- ... -->
  </ItemGroup>
</Project>
```

**Benefits:**
- ✅ **Consistency** - All projects use same package versions
- ✅ **Maintainability** - Update version in one place
- ✅ **Conflict Prevention** - No version mismatches
- ✅ **Easier Upgrades** - Clear view of all dependencies

---

### 33. Explicit String Comparison

**Location:** Repository, Services

**Description:** Uses `StringComparison` enum for culture-aware string operations.

**Example:**
```csharp
public async Task<Product?> GetByName(string name)
{
    return await Task.FromResult(_memoryProducts
        .FirstOrDefault(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase)));
}
```

**Why Explicit:**
- ✅ **Performance** - Ordinal comparison is faster than culture-aware
- ✅ **Predictability** - Same behavior across cultures
- ✅ **Security** - Prevents culture-based attacks
- ❌ Avoid: `name.ToLower() == other.ToLower()` (allocates strings)

---

### 34. ProducesResponseType Attributes

**Location:** All controller actions

**Description:** Documents expected response types for OpenAPI/Swagger schema generation.

**Example:**
```csharp
[HttpGet("{id}")]
[ProducesResponseType(typeof(ProductResponse), StatusCodes.Status200OK)]
[ProducesResponseType(typeof(ResponseViewModel), StatusCodes.Status400BadRequest)]
[ProducesResponseType(typeof(ResponseViewModel), StatusCodes.Status404NotFound)]
[ProducesResponseType(typeof(ResponseViewModel), StatusCodes.Status412PreconditionFailed)]
public async Task<IActionResult> GetById(uint id)
{
    // ...
}
```

**Benefits:**
- ✅ **Accurate Swagger docs** - Shows exact response types
- ✅ **Client code generation** - Strongly-typed client SDKs
- ✅ **API contract** - Documents expected behavior
- ✅ **IntelliSense** - Better IDE support

---

### 35. DTO Anti-Corruption Layer

**Location:** Application layer (ViewModels), AutoMapper configuration

**Description:** Separates internal domain models from external API contracts using DTOs.

**Pattern:**
```
Client Request (JSON)
    ↓
ProductViewModel (DTO)
    ↓ (AutoMapper)
Product (Domain Entity)
    ↓
Repository
```

**Benefits:**
- ✅ **Versioning** - API v1 and v2 can use different DTOs for same entity
- ✅ **Security** - Don't expose internal entity structure
- ✅ **Flexibility** - Change domain model without breaking API
- ✅ **Validation** - Different validation rules for API vs domain

**AutoMapper Configuration:**
```csharp
public class AutomapperConfig : Profile
{
    public AutomapperConfig()
    {
        CreateMap<Product, ProductViewModel>().ReverseMap();
    }
}
```

---

### 36. Scoped Lifetime for Stateful Services

**Location:** `DependencyInjectionConfig.cs`

**Description:** Uses `Scoped` lifetime for services that maintain request-specific state.

**Registration:**
```csharp
services.AddScoped<INotificatorHandler, NotificatorHandler>(); // Stateful (errors list)
services.AddScoped<IProductAppService, ProductAppService>();   // Stateless but uses scoped dependencies
services.AddScoped<IProductRepository, ProductRepository>();   // Stateful (in-memory data)
```

**Lifetime Strategy:**
- **Scoped** - Created once per HTTP request
  - `NotificatorHandler` (accumulates errors per request)
  - Application services (use scoped dependencies)
  - Repositories (may have request-specific context)

- **Transient** - Created every time requested
  - Validators (stateless)

- **Singleton** - Created once for application lifetime
  - Configuration objects
  - Loggers (Serilog)

---

### 37. Explicit Error Messages

**Location:** Validators, Services

**Description:** Clear, actionable error messages for validation failures.

**Examples:**
```csharp
// FluentValidation
RuleFor(p => p.Name)
    .NotEmpty()
    .WithMessage("Name is required"); // Not: "Name cannot be empty"

RuleFor(p => p.Weight)
    .GreaterThan(0)
    .WithMessage("Weight must be greater than 0"); // Not: "Invalid weight"

// Business validation
_notificator.AddError($"Product ({product.Name}) is already registered");
// Not: "Duplicate product"
```

**Principles:**
- ✅ **Specific** - Tell user exactly what's wrong
- ✅ **Actionable** - User knows how to fix it
- ✅ **Contextual** - Include relevant data (e.g., product name)
- ❌ Avoid generic messages like "Invalid input"

---

### 38. Guard Clauses (Fail-Fast)

**Location:** Services, Controllers

**Description:** Early validation and return to avoid nested conditionals.

**Example:**
```csharp
public async Task<ProductViewModel?> Create(ProductViewModel product)
{
    // Guard clause 1
    if (product is null)
    {
        _notificator.AddError("Product could not be created");
        return null;
    }

    // Guard clause 2
    if (await _productRepository.GetByName(product.Name!) is not null)
    {
        _notificator.AddError($"Product ({product.Name!}) is already registered");
        return null;
    }

    // Happy path (not nested)
    var response = _mapper.Map<ProductViewModel>(
        await _productRepository.Create(_mapper.Map<Product>(product)));

    if (response is null)
        _notificator.AddError("Product could not be created");

    return response;
}
```

**Benefits:**
- ✅ **Readability** - Happy path not buried in nested ifs
- ✅ **Performance** - Exits early on validation failure
- ✅ **Maintainability** - Easy to add new validations

---

### 39. Separation of Read and Write Models (Implicit CQRS)

**Location:** Application services, Controllers

**Description:** Different response models for queries vs commands.

**Pattern:**
```csharp
// Query - Returns data
[HttpGet]
public async Task<IActionResult> GetAll()
{
    IList<ProductViewModel> products = await _productApplication.GetAll();
    return CustomResponse(products); // ProductListResponse
}

// Command - Returns success/failure
[HttpPut]
public async Task<IActionResult> Update([FromBody] ProductViewModel product)
{
    return await _productApplication.Update(product)
        ? CustomResponse(HttpStatusCode.NoContent, true)
        : CustomResponse(); // No data, just status
}
```

**Response Types:**
- **Queries** (GET) → Return data (`ProductResponse`, `ProductListResponse`)
- **Commands** (POST/PUT/DELETE) → Return status (201/204) or errors

**Note:** This is **implicit CQRS** - not full CQRS with separate read/write databases, but follows the principle of separating query and command concerns.

---

### 40. Immutable Configuration Objects

**Location:** Configuration classes

**Description:** Configuration read from `appsettings.json` treated as immutable after startup.

**Pattern:**
```csharp
// Read configuration
var jwtSettings = builder.Configuration.GetSection("Authorization");

// Use throughout application (never modified)
services.AddAuthentication(/* ... uses jwtSettings */);
```

**Benefits:**
- ✅ **Thread-safe** - No concurrent modification issues
- ✅ **Predictable** - Configuration doesn't change during runtime
- ✅ **Testable** - Easy to provide test configuration

---

## 📊 Pattern Summary

### By Category

| Category | Pattern Count |
|----------|---------------|
| **Architectural Patterns** | 4 |
| **Design Patterns** | 5 |
| **API Design Patterns** | 4 |
| **Validation Patterns** | 2 |
| **Testing Patterns** | 3 |
| **Security Patterns** | 3 |
| **Configuration Patterns** | 3 |
| **Modern C# Patterns** | 5 |
| **Best Practices** | 11 |
| **Total** | **40** |

### By Complexity

| Level | Patterns |
|-------|----------|
| **Fundamental** | Repository, DI, Builder, DTO |
| **Intermediate** | Notification, Template Method, Chain of Responsibility, Middleware |
| **Advanced** | Clean Architecture, Multi-Layer Validation, Response Envelope, WebApplicationFactory |
| **Expert** | Async All the Way, Zero Warnings Policy, Security Hardening |

---

## 🎯 Key Points

### Architecture
- ✅ **Clean Architecture** with verifiable dependency rules
- ✅ **Separation of Concerns** across 4 distinct layers
- ✅ **Dependency Inversion** - high-level modules depend on abstractions

### Design Patterns
- ✅ **Notification Pattern** for exception-free error handling
- ✅ **Repository Pattern** for data access abstraction
- ✅ **Template Method** for consistent API responses
- ✅ **Chain of Responsibility** in validation pipeline

### API Design
- ✅ **Response Envelope** for consistent client experience
- ✅ **Semantic HTTP status codes** (especially 412 for business rules)
- ✅ **Multi-layer validation** (framework → input → business)
- ✅ **API versioning** with multiple reader strategies

### Testing
- ✅ **Three-tier testing** (Unit → Integration → Builders)
- ✅ **WebApplicationFactory** for true end-to-end tests
- ✅ **Realistic test data** via Bogus library
- ✅ **AAA pattern** for test clarity

### Modern .NET
- ✅ **.NET 10** with latest C# 14 features
- ✅ **Nullable reference types** for compile-time safety
- ✅ **Primary constructors** for cleaner code
- ✅ **Async all the way** for scalability

### Production Readiness
- ✅ **Zero warnings policy** enforced at build time
- ✅ **Structured logging** with Serilog
- ✅ **Docker multi-stage builds** with security hardening
- ✅ **JWT authentication** with comprehensive validation
- ✅ **Central package management** for consistency

---

## 📚 References

- **Clean Architecture** - Robert C. Martin (Uncle Bob)
- **Domain-Driven Design** - Eric Evans
- **Enterprise Application Architecture Patterns** - Martin Fowler
- **RESTful Web APIs** - Leonard Richardson, Mike Amundsen
- **C# 12 and .NET 10** - Microsoft Documentation
- **ASP.NET Core Best Practices** - Microsoft Docs

---

**For implementation details, see the codebase. For architectural overview, see [README.md](README.md).**

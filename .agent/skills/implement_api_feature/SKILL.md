---
name: implement_api_feature
description: Guide to implementing a full vertical feature in the Demo API following the Clean Architecture pattern used in swagger-jwt-docker.
version: 1.0.0
---

# 🚀 Implement API Feature (Vertical Slice)

**Implements a complete feature across Domain, Application, and API layers following the strict `swagger-jwt-docker` project standards.**

## 📋 Overview

This skill guides the creation of a new feature (e.g., "Customer Management") by implementing the specific Layered Architecture pattern used in this project. It enforces the use of **Primary Constructors**, **BaseServices**, **Automapper**, and **FluentValidation**.

## 🏗️ Architecture Flow

The implementation must follow this order to ensure dependency resolution:
1.  **Domain**: Entities & Repository Interfaces
2.  **Infra**: Repository Implementation & Mapping
3.  **Application**: ViewModels, Validators, Service Interface & Implementation
4.  **API**: Controller (Versioned & Authorized)

## ⚙️ Execution Steps

### Phase 1: Domain Layer (`src/DemoApi.Domain`)

#### 1. Create Entity
*   **Location**: `Entities/[Entity].cs`
*   **Inheritance**: Must inherit from `Entity`.
*   **Structure**:
    ```csharp
    namespace DemoApi.Domain.Entities;

    public class Product : Entity
    {
        #region Properties
        public uint Id { get; set; }
        public required string Name { get; set; }
        #endregion
    }
    ```

#### 2. Define Repository Interface
*   **Location**: `Interfaces/I[Entity]Repository.cs`
*   **Structure**: Must extend `IRepository<T>`.

### Phase 2: Infrastructure Layer (`src/DemoApi.Infra`)

#### 1. Implement Repository
*   **Location**: `Repositories/[Entity]Repository.cs`
*   **Inheritance**: Inherits `Repository<T>` and implements `I[Entity]Repository`.
*   **Database**: Uses EF Core `DbContext`.

### Phase 3: Application Layer (`src/DemoApi.Application`)

#### 1. Create ViewModels
*   **Location**: `Models/[Context]/[Entity]ViewModel.cs`
*   **Standard**: Use properties matching the Entity, but flattening complex objects if needed.

#### 2. Create Validator
*   **Location**: `Validators/[Entity]Validator.cs`
*   **Library**: FluentValidation `AbstractValidator<T>`.

#### 3. Implement Service
*   **Location**: `Services/[Entity]AppService.cs`
*   **Inheritance**: Must inherit `BaseServices` and implement `I[Entity]AppService`.
*   **Dependency Injection**: Use **Primary Constructors**.
*   **Error Handling**: Use `_notificator.AddError("Message")` (Notification Pattern), do NOT throw exceptions for validation errors.
*   **Template**:
    ```csharp
    public class ProductAppService(
        IMapper mapper, 
        INotificatorHandler notificator, 
        IProductRepository repository) : BaseServices, IProductAppService
    {
        // ... Implementation using _mapper and _repository
    }
    ```

#### 4. AutoMapper Configuration
*   **Action**: Update `Automapper/AutoMapperConfig.cs` (or Profile) to map `Entity <-> ViewModel`.

### Phase 4: API Layer (`src/DemoApi.Api`)

#### 1. Create Controller
*   **Location**: `V1/Controllers/[Entity]Controller.cs`
*   **Inheritance**: `MainApiController`.
*   **Attributes**:
    *   `[ApiVersion("1.0")]`
    *   `[Route("api/v{version:apiVersion}/[resource]")]`
    *   `[Authorize]` (if secured)
*   **Response Handling**: ALWAYS use `CustomResponse(result)` methods.
*   **Template**:
    ```csharp
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/products")]
    public class ProductController(
        INotificatorHandler notificator, 
        IProductAppService appService) : MainApiController(notificator)
    {
        [HttpGet]
        [ProducesResponseType(typeof(ListResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            return CustomResponse(await appService.GetAll());
        }
    }
    ```

## 📦 Best Practices Checklist

| Category | Requirement | Criticality |
|----------|-------------|-------------|
| **DI** | Use Primary Constructors for all classes | 🔴 High |
| **Async** | All I/O methods must be `async Task` | 🔴 High |
| **Response** | Never return `Ok()` directly; use `CustomResponse()` | 🔴 High |
| **Validation** | Never throw exceptions for bad requests; use `Notificator` | 🔴 High |
| **Swagger** | Add `[ProducesResponseType]` to ALL endpoints | 🟡 Medium |

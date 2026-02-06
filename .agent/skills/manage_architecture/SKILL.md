---
name: manage_architecture
description: Enforces the Clean Architecture and design patterns used in the project (Domain-Centric, Notification Pattern).
version: 1.0.0
---

# 🏗️ Manage Architecture & Patterns

**Guarantees structural integrity and adherence to the `demo-api` architectural rules.**

## 🎯 Scope
Use this skill when:
*   Refactoring code or moving classes.
*   Designing a new module.
*   Checking dependency violations.

## 📐 core Principles

### 1. The Dependency Rule (Strict)
*   **Domain**: ⛔ Depends on NOTHING. (No NuGets describing db access, http, etc).
*   **Application**: ⬅️ Depends on `Domain`.
*   **Infrastructure**: ⬅️ Depends on `Domain` (Interfaces) and `Application` (Services).
*   **API**: ⬅️ Depends on `Application`.

### 2. Key Patterns

#### The Notification Pattern (Handling Errors)
Instead of throwing Exceptions for business logic errors (e.g., "Product already exists"), use the `INotificatorHandler`.

*   **Wrong**: `throw new BusinessException("Exists");`
*   **Right**:
    ```csharp
    if (exists) {
        _notificator.AddError("Product already exists");
        return null; // or false
    }
    ```

#### The Repository Pattern
*   Interface defined in **Domain** (`IProductRepository`).
*   Implementation in **Infrastructure** (`ProductRepository`).
*   **Must** return **Entities** (`Product`), NOT ViewModels/DTOs.

#### The ViewModel Pattern (Application)
*   Services receive and return `ViewModel`.
*   Services map `ViewModel` -> `Entity` internally (using AutoMapper) before calling Repositories.

## 📂 Folder Structure

```
/src
├── DemoApi.Domain
│   ├── Entities (Rich models)
│   ├── Interfaces (Repositories, Services)
│   └── Validations (Domain rules)
├── DemoApi.Application
│   ├── Services (Implement logic, use Notificator)
│   ├── Models (ViewModels)
│   └── Interfaces (For Services)
├── DemoApi.Infra
│   ├── Repositories (EF Core impl)
│   └── Data (DbContext, Mappings)
└── DemoApi.Api
    ├── Controllers (Thin, delegate to AppService)
    └── Configurations
```

## 🚨 Architectural Red Flags
*   **Entity in Controller**: Entities should never leak to the API layer. Map to ViewModel.
*   **Logic in Controller**: Controllers only handle HTTP (Status Codes, Routing).
*   **Framework in Domain**: No `AspNetCore`, `EntityFramework` references in Domain project.

---
name: perform_code_review
description: Checklist for validating code against the project's strict standards (derived from cartolafy docs but adapted for demo-api).
version: 1.0.0
---

# 🕵️ Perform Code Review

**Validates code quality, security, and architectural compliance for the Demo API.**

## 🎯 Scope
Use this skill when:
*   USER asks to "review my code".
*   Before committing any complex feature.
*   Debugging obscure issues.

## ✅ Review Checklist

### 1. 🏗️ Architecture & Patterns
- [ ] **Layering**: Domain depends on nothing; Application depends on Domain; Api depends on Application.
- [ ] **Service Pattern**: Services inherit `BaseServices` and use `INotificatorHandler` (No throwing exceptions for business logic).
- [ ] **Repository Pattern**: Repositories return Entities, not ViewModels.
- [ ] **Controller Pattern**: Controllers inherit `MainApiController` and return `CustomResponse`.

### 2. 🛡️ Security
- [ ] **Authorization**: `[Authorize]` attribute present on protected endpoints?
- [ ] **Input Validation**: Validators (`AbstractValidator`) exist and are called in the Service layer?
- [ ] **Injection**: No raw SQL; usage of EF Core methods only.
- [ ] **Secrets**: No API keys or connection strings hardcoded?

### 3. 🧹 Clean Code (C# 14 / .NET 10)
- [ ] **Primary Constructors**: Used for DI instead of fields + ctor assignments.
    *   *Bad*: `public Class(IService s) { _s = s; }`
    *   *Good*: `public Class(IService s)` (Primary Constructor)
- [ ] **Async/Await**: No `.Result` or `.Wait()`. usage of `await`.
- [ ] **Naming**: 
    *   `I[Name]Repository`
    *   `[Name]AppService`
    *   `[Name]ViewModel`
    *   `[Name]Controller`

### 4. 📝 Documentation
- [ ] **Swagger**: `[ProducesResponseType]` attributes on all Controller actions?
- [ ] **Comments**: XML comments on public API methods?

### 5. 🧪 Testing
- [ ] **Unit Tests**: Exist in `Tests` project?
- [ ] **Mocking**: Dependencies mocked using `Moq`?
- [ ] **Assertions**: Testing `CustomResponse` results correctly?

## 🚨 Critical "Red Flags" (Immediate Rejection)
1.  **Direct `Ok(), BadRequest()` usage**: Must use `CustomResponse`.
2.  **Throwing Exceptions for Validation**: Must use `INotificator`.
3.  **Returning Entities in API**: Must map to `ViewModel` before Controller return.
4.  **Mixing Logic in Controller**: Controllers should be thin proxies to Services.

---
**Reference**: Derived from project `cartolafy/docs/guides/code-review-checklist.md` and `demo-api/swagger-jwt-docker` implementation.

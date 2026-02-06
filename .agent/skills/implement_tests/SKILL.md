---
name: implement_tests
description: Guide to implementing unit tests following the project's strict standards (xUnit, Moq, FluentAssertions).
version: 1.0.0
---

# 🧪 Implement Unit Tests

**Generates unit tests that comply with the `DemoApi` testing standards.**

## 🎯 Scope
Use this skill when:
*   Creating tests for a new Service or Controller.
*   Adding coverage for existing legacy code.
*   The USER asks to "add tests".

## 📋 Standards & Conventions

### 1. Files & Organization
*   **Project**: `[ProjectName].Tests` (e.g., `DemoApi.Application.Tests`).
*   **Granularity**: One file per **Process/Method** being tested (e.g., `CreateProductTests.cs` instead of `ProductServiceTests.cs`).
*   **Base Class**: Inherit from common base for setup (e.g., `ProductTests` which setups the Mocks).

### 2. Naming Convention
*   **Format**: `[MethodName]_Should[ExpectedResult]_When[Scenario]`
*   **Example**: `Create_ShouldReturnProduct_WhenRepositoryCreatesSuccessfully`

### 3. Tools Breakdown
*   **Framework**: `xUnit` (`[Fact]`).
*   **Mocking**: `Moq` (`Mock<IInterface>`).
*   **Assertions**: `FluentAssertions` (`result.Should().Be(...)`).
*   **Data Generation**: **Builders** pattern (e.g., `ProductBuilder.New().Build()`).

## 🧱 Implementation Template

```csharp
using FluentAssertions;
using Moq;
using Xunit;

namespace DemoApi.Application.Tests.Products;

public class CreateProductTests : ProductTests // Inherit shared setup
{
    [Fact]
    public async Task Create_ShouldReturnProduct_WhenRepositoryCreatesSuccessfully()
    {
        // 1. Arrange
        var (notificator, repository, service) = SetProductAppService();
        var fakeEntity = ProductBuilder.New().Build();
        var viewModel = _mapper.Map<ProductViewModel>(fakeEntity);

        repository.Setup(x => x.GetByName(viewModel.Name!))
                  .ReturnsAsync((Product?)null); // Setup specific scenario

        repository.Setup(x => x.Create(It.IsAny<Product>()))
                  .ReturnsAsync(fakeEntity);

        // 2. Act
        var result = await service.Create(viewModel);

        // 3. Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(fakeEntity.Name);

        // Verify Mocks
        repository.Verify(x => x.Create(It.IsAny<Product>()), Times.Once);
        notificator.Verify(x => x.AddError(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Create_ShouldReturnNull_WhenValidationFails()
    {
        // Arrange
        var (notificator, repository, service) = SetProductAppService();
        
        // ... Setup failure scenario ...

        // Act
        var result = await service.Create(invalidViewModel);

        // Assert
        result.Should().BeNull();
        notificator.Verify(x => x.AddError(It.IsAny<string>()), Times.Once);
    }
}
```

## 🚨 Critical Checkpoints
1.  **Never** mock objects you are testing (the System Under Test).
2.  **Always** verify `INotificatorHandler` calls for failure scenarios.
3.  **Use Builders** (`ProductBuilder`) instead of manual object instantiation where possible.
4.  **No `Assert.Equal`**; use `result.Should().Be()`.

using DemoApi.Application.Models.Products;
using DemoApi.Application.Validators.Products;
using FluentAssertions;
using FluentValidation.Results;
using DemoApi.Tests.Builders.Products;

namespace DemoApi.Application.Tests.Products.Validators
{
    public class ProductValidatorTests
    {
        #region Properties

        private readonly ProductValidator _validator;

        #endregion

        #region Constructors

        public ProductValidatorTests()
        {
            _validator = new ProductValidator();
        }

        #endregion

        #region Name Validation Tests

        [Fact]
        public void Validate_EmptyName_ReturnsValidationError()
        {
            // Arrange
            ProductViewModel model = ProductViewModelBuilder.New()
                .WithEmptyName()
                .Build();

            // Act
            ValidationResult result = _validator.Validate(model);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => 
                e.PropertyName == "Name" && 
                e.ErrorMessage == "Name is required");
        }

        [Fact]
        public void Validate_NullName_ReturnsValidationError()
        {
            // Arrange
            ProductViewModel model = ProductViewModelBuilder.New()
                .WithNullName()
                .Build();

            // Act
            ValidationResult result = _validator.Validate(model);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => 
                e.PropertyName == "Name" && 
                e.ErrorMessage == "Name is required");
        }

        [Fact]
        public void Validate_WhitespaceName_ReturnsValidationError()
        {
            // Arrange
            ProductViewModel model = ProductViewModelBuilder.New()
                .WithWhitespaceName()
                .Build();

            // Act
            ValidationResult result = _validator.Validate(model);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => 
                e.PropertyName == "Name" && 
                e.ErrorMessage == "Name is required");
        }

        [Fact]
        public void Validate_ValidName_ReturnsNoNameError()
        {
            // Arrange
            ProductViewModel model = ProductViewModelBuilder.New().Build();

            // Act
            ValidationResult result = _validator.Validate(model);

            // Assert
            result.Errors.Should().NotContain(e => e.PropertyName == "Name");
        }

        [Theory]
        [InlineData("\t")]
        [InlineData("\n")]
        [InlineData("\r")]
        [InlineData("\t\n\r")]
        [InlineData("   \t   ")]
        public void Validate_NameWithWhitespaceCharacters_ReturnsValidationError(string name)
        {
            // Arrange
            ProductViewModel model = ProductViewModelBuilder.New()
                .WithName(name)
                .Build();

            // Act
            ValidationResult result = _validator.Validate(model);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => 
                e.PropertyName == "Name" && 
                e.ErrorMessage == "Name is required");
        }

        [Theory]
        [InlineData("Product Name with Trailing Spaces   ")]
        [InlineData("   Product Name with Leading Spaces")]
        [InlineData("  Product Name with Both  ")]
        public void Validate_NameWithLeadingOrTrailingSpaces_ReturnsNoError(string name)
        {
            // Arrange
            ProductViewModel model = ProductViewModelBuilder.New()
                .WithName(name)
                .Build();

            // Act
            ValidationResult result = _validator.Validate(model);

            // Assert
            result.Errors.Should().NotContain(e => e.PropertyName == "Name");
        }

        #endregion

        #region Weight Validation Tests

        [Fact]
        public void Validate_ZeroWeight_ReturnsValidationError()
        {
            // Arrange
            ProductViewModel model = ProductViewModelBuilder.New()
                .WithZeroWeight()
                .Build();

            // Act
            ValidationResult result = _validator.Validate(model);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => 
                e.PropertyName == "Weight" && 
                e.ErrorMessage == "Weight must be greater than 0");
        }

        [Fact]
        public void Validate_NegativeWeight_ReturnsValidationError()
        {
            // Arrange
            ProductViewModel model = ProductViewModelBuilder.New()
                .WithNegativeWeight()
                .Build();

            // Act
            ValidationResult result = _validator.Validate(model);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => 
                e.PropertyName == "Weight" && 
                e.ErrorMessage == "Weight must be greater than 0");
        }

        [Theory]
        [InlineData(-100)]
        [InlineData(-10.5)]
        [InlineData(-0.01)]
        public void Validate_MultipleNegativeWeights_ReturnsValidationError(double weight)
        {
            // Arrange
            ProductViewModel model = ProductViewModelBuilder.New()
                .WithWeight(weight)
                .Build();

            // Act
            ValidationResult result = _validator.Validate(model);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => 
                e.PropertyName == "Weight" && 
                e.ErrorMessage == "Weight must be greater than 0");
        }

        [Theory]
        [InlineData(0.01)]
        [InlineData(1.0)]
        [InlineData(10.5)]
        [InlineData(100)]
        [InlineData(999.99)]
        public void Validate_PositiveWeight_ReturnsNoWeightError(double weight)
        {
            // Arrange
            ProductViewModel model = ProductViewModelBuilder.New()
                .WithWeight(weight)
                .Build();

            // Act
            ValidationResult result = _validator.Validate(model);

            // Assert
            result.Errors.Should().NotContain(e => e.PropertyName == "Weight");
        }

        [Theory]
        [InlineData(0.0001)]
        [InlineData(0.001)]
        [InlineData(0.01)]
        [InlineData(0.1)]
        public void Validate_SmallPositiveWeights_ReturnsNoWeightError(double weight)
        {
            // Arrange
            ProductViewModel model = ProductViewModelBuilder.New()
                .WithWeight(weight)
                .Build();

            // Act
            ValidationResult result = _validator.Validate(model);

            // Assert
            result.Errors.Should().NotContain(e => e.PropertyName == "Weight");
        }

        [Theory]
        [InlineData(1000)]
        [InlineData(10000)]
        [InlineData(100000)]
        [InlineData(1000000)]
        public void Validate_LargePositiveWeights_ReturnsNoWeightError(double weight)
        {
            // Arrange
            ProductViewModel model = ProductViewModelBuilder.New()
                .WithWeight(weight)
                .Build();

            // Act
            ValidationResult result = _validator.Validate(model);

            // Assert
            result.Errors.Should().NotContain(e => e.PropertyName == "Weight");
        }

        [Fact]
        public void Validate_WeightWithManyDecimalPlaces_ReturnsNoWeightError()
        {
            // Arrange
            ProductViewModel model = ProductViewModelBuilder.New()
                .WithPreciseWeight()
                .Build();

            // Act
            ValidationResult result = _validator.Validate(model);

            // Assert
            result.Errors.Should().NotContain(e => e.PropertyName == "Weight");
        }

        #endregion

        #region Multiple Errors Tests

        [Fact]
        public void Validate_InvalidNameAndWeight_ReturnsMultipleValidationErrors()
        {
            // Arrange
            ProductViewModel model = ProductViewModelBuilder.New()
                .WithEmptyName()
                .WithNegativeWeight()
                .Build();

            // Act
            ValidationResult result = _validator.Validate(model);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(2);
            result.Errors.Should().Contain(e => e.PropertyName == "Name");
            result.Errors.Should().Contain(e => e.PropertyName == "Weight");
        }

        [Fact]
        public void Validate_AllValidFields_ReturnsNoErrors()
        {
            // Arrange
            ProductViewModel model = ProductViewModelBuilder.New().Build();

            // Act
            ValidationResult result = _validator.Validate(model);

            // Assert
            result.IsValid.Should().BeTrue();
            result.Errors.Should().BeEmpty();
        }

        [Theory]
        [InlineData("", 0)]
        [InlineData("", -1)]
        [InlineData(null, 0)]
        [InlineData(null, -1)]
        [InlineData("   ", 0)]
        [InlineData("   ", -10.5)]
        public void Validate_MultipleInvalidFieldCombinations_ReturnsMultipleErrors(string? name, double weight)
        {
            // Arrange
            ProductViewModel model = ProductViewModelBuilder.New()
                .WithName(name!)
                .WithWeight(weight)
                .Build();

            // Act
            ValidationResult result = _validator.Validate(model);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(2);
            result.Errors.Select(e => e.PropertyName).Should().Contain(new[] { "Name", "Weight" });
        }

        #endregion

        #region Boundary Tests

        [Fact]
        public void Validate_VerySmallPositiveWeight_ReturnsNoWeightError()
        {
            // Arrange
            ProductViewModel model = ProductViewModelBuilder.New()
                .WithVerySmallPositiveWeight()
                .Build();

            // Act
            ValidationResult result = _validator.Validate(model);

            // Assert
            result.Errors.Should().NotContain(e => e.PropertyName == "Weight");
        }

        [Fact]
        public void Validate_MaximumWeight_ReturnsNoWeightError()
        {
            // Arrange
            ProductViewModel model = ProductViewModelBuilder.New()
                .WithMaximumWeight()
                .Build();

            // Act
            ValidationResult result = _validator.Validate(model);

            // Assert
            result.Errors.Should().NotContain(e => e.PropertyName == "Weight");
        }

        [Fact]
        public void Validate_MinimumWeight_ReturnsValidationError()
        {
            // Arrange
            ProductViewModel model = ProductViewModelBuilder.New()
                .WithMinimumWeight()
                .Build();

            // Act
            ValidationResult result = _validator.Validate(model);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Weight");
        }

        [Fact]
        public void Validate_NegativeZero_ReturnsValidationError()
        {
            // Arrange
            ProductViewModel model = ProductViewModelBuilder.New()
                .WithWeight(-0.0)
                .Build();

            // Act
            ValidationResult result = _validator.Validate(model);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Weight");
        }

        #endregion

        #region Edge Cases

        [Fact]
        public void Validate_NameWithOnlySpaces_ReturnsValidationError()
        {
            // Arrange
            ProductViewModel model = ProductViewModelBuilder.New()
                .WithWhitespaceName()
                .Build();

            // Act
            ValidationResult result = _validator.Validate(model);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Name");
        }

        [Fact]
        public void Validate_NameWithSpecialCharacters_ReturnsNoNameError()
        {
            // Arrange
            ProductViewModel model = ProductViewModelBuilder.New()
                .WithSpecialCharactersName()
                .Build();

            // Act
            ValidationResult result = _validator.Validate(model);

            // Assert
            result.Errors.Should().NotContain(e => e.PropertyName == "Name");
        }

        [Fact]
        public void Validate_VeryLongName_ReturnsNoNameError()
        {
            // Arrange
            ProductViewModel model = ProductViewModelBuilder.New()
                .WithLongName(500)
                .Build();

            // Act
            ValidationResult result = _validator.Validate(model);

            // Assert
            result.Errors.Should().NotContain(e => e.PropertyName == "Name");
        }

        [Fact]
        public void Validate_NameWithUnicodeCharacters_ReturnsNoNameError()
        {
            // Arrange
            ProductViewModel model = ProductViewModelBuilder.New()
                .WithUnicodeName()
                .Build();

            // Act
            ValidationResult result = _validator.Validate(model);

            // Assert
            result.Errors.Should().NotContain(e => e.PropertyName == "Name");
        }

        [Fact]
        public void Validate_SingleCharacterName_ReturnsNoNameError()
        {
            // Arrange
            ProductViewModel model = ProductViewModelBuilder.New()
                .WithSingleCharacterName()
                .Build();

            // Act
            ValidationResult result = _validator.Validate(model);

            // Assert
            result.Errors.Should().NotContain(e => e.PropertyName == "Name");
        }

        [Theory]
        [InlineData("??")]
        [InlineData("????")]
        [InlineData("Product ??")]
        [InlineData("Café ?")]
        public void Validate_NameWithEmojis_ReturnsNoNameError(string name)
        {
            // Arrange
            ProductViewModel model = ProductViewModelBuilder.New()
                .WithName(name)
                .Build();

            // Act
            ValidationResult result = _validator.Validate(model);

            // Assert
            result.Errors.Should().NotContain(e => e.PropertyName == "Name");
        }

        [Theory]
        [InlineData("Product\nWith\nNewLines")]
        [InlineData("Product\tWith\tTabs")]
        [InlineData("Product\rWith\rCarriageReturns")]
        public void Validate_NameWithControlCharacters_ReturnsNoNameError(string name)
        {
            // Arrange
            ProductViewModel model = ProductViewModelBuilder.New()
                .WithName(name)
                .Build();

            // Act
            ValidationResult result = _validator.Validate(model);

            // Assert
            result.Errors.Should().NotContain(e => e.PropertyName == "Name");
        }

        [Fact]
        public void Validate_NameWithOnlyNumbers_ReturnsNoNameError()
        {
            // Arrange
            ProductViewModel model = ProductViewModelBuilder.New()
                .WithName("1234567890")
                .Build();

            // Act
            ValidationResult result = _validator.Validate(model);

            // Assert
            result.Errors.Should().NotContain(e => e.PropertyName == "Name");
        }

        [Fact]
        public void Validate_NameWithMixedCasing_ReturnsNoNameError()
        {
            // Arrange
            ProductViewModel model = ProductViewModelBuilder.New()
                .WithName("PrOdUcT NaMe")
                .Build();

            // Act
            ValidationResult result = _validator.Validate(model);

            // Assert
            result.Errors.Should().NotContain(e => e.PropertyName == "Name");
        }

        #endregion

        #region Id Validation Tests

        [Fact]
        public void Validate_IdZero_ReturnsNoIdError()
        {
            // Arrange
            ProductViewModel model = ProductViewModelBuilder.New()
                .WithIdZero()
                .Build();

            // Act
            ValidationResult result = _validator.Validate(model);

            // Assert
            result.Errors.Should().NotContain(e => e.PropertyName == "Id");
        }

        [Fact]
        public void Validate_IdPositive_ReturnsNoIdError()
        {
            // Arrange
            ProductViewModel model = ProductViewModelBuilder.New()
                .WithId(12345)
                .Build();

            // Act
            ValidationResult result = _validator.Validate(model);

            // Assert
            result.Errors.Should().NotContain(e => e.PropertyName == "Id");
        }

        [Fact]
        public void Validate_IdMaxValue_ReturnsNoIdError()
        {
            // Arrange
            ProductViewModel model = ProductViewModelBuilder.New()
                .WithId(uint.MaxValue)
                .Build();

            // Act
            ValidationResult result = _validator.Validate(model);

            // Assert
            result.Errors.Should().NotContain(e => e.PropertyName == "Id");
        }

        #endregion

        #region Validator Metadata Tests

        [Fact]
        public void CreateDescriptor_NameProperty_ReturnsValidationRules()
        {
            // Arrange & Act
            FluentValidation.IValidatorDescriptor descriptor = _validator.CreateDescriptor();
            var nameRules = descriptor.GetMembersWithValidators()
                .Where(m => m.Key == "Name")
                .SelectMany(m => m);

            // Assert
            nameRules.Should().NotBeEmpty();
        }

        [Fact]
        public void CreateDescriptor_WeightProperty_ReturnsValidationRules()
        {
            // Arrange & Act
            FluentValidation.IValidatorDescriptor descriptor = _validator.CreateDescriptor();
            var weightRules = descriptor.GetMembersWithValidators()
                .Where(m => m.Key == "Weight")
                .SelectMany(m => m);

            // Assert
            weightRules.Should().NotBeEmpty();
        }

        [Fact]
        public void CreateDescriptor_ShouldHaveExactlyTwoPropertiesWithRules()
        {
            // Arrange & Act
            FluentValidation.IValidatorDescriptor descriptor = _validator.CreateDescriptor();
            var propertiesWithRules = descriptor.GetMembersWithValidators().Select(m => m.Key).Distinct();

            // Assert
            propertiesWithRules.Should().HaveCount(2);
            propertiesWithRules.Should().Contain(new[] { "Name", "Weight" });
        }

        [Fact]
        public void CreateDescriptor_IdProperty_ShouldNotHaveValidationRules()
        {
            // Arrange & Act
            FluentValidation.IValidatorDescriptor descriptor = _validator.CreateDescriptor();
            var idRules = descriptor.GetMembersWithValidators()
                .Where(m => m.Key == "Id")
                .SelectMany(m => m);

            // Assert
            idRules.Should().BeEmpty();
        }

        #endregion

        #region Validation Result Tests

        [Fact]
        public void Validate_ValidProduct_ShouldReturnEmptyErrorCollection()
        {
            // Arrange
            ProductViewModel model = ProductViewModelBuilder.New().Build();

            // Act
            ValidationResult result = _validator.Validate(model);

            // Assert
            result.Errors.Should().BeEmpty();
            result.Errors.Should().HaveCount(0);
        }

        [Fact]
        public void Validate_InvalidProduct_ShouldReturnCorrectErrorCount()
        {
            // Arrange
            ProductViewModel model = ProductViewModelBuilder.New()
                .WithNullName()
                .WithZeroWeight()
                .Build();

            // Act
            ValidationResult result = _validator.Validate(model);

            // Assert
            result.Errors.Should().HaveCount(2);
        }

        [Fact]
        public void Validate_ShouldReturnSameValidatorInstance()
        {
            // Arrange
            ProductViewModel model = ProductViewModelBuilder.New().Build();

            // Act
            ValidationResult result1 = _validator.Validate(model);
            ValidationResult result2 = _validator.Validate(model);

            // Assert
            result1.Should().NotBeSameAs(result2);
            result1.IsValid.Should().Be(result2.IsValid);
        }

        #endregion
    }
}
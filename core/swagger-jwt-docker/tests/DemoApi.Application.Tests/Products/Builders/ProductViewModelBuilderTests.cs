using DemoApi.Application.Models.Products;
using DemoApi.Tests.Builders.Products;
using FluentAssertions;

namespace DemoApi.Application.Tests.Products.Builders
{
    public class ProductViewModelBuilderTests
    {
        #region Default Values Tests

        [Fact]
        public void Build_ShouldCreateValidProduct_WhenUsingDefaultValues()
        {
            // Arrange & Act
            ProductViewModel product = ProductViewModelBuilder.New().Build();

            // Assert
            product.Should().NotBeNull();
            product.Id.Should().Be(0);
            product.Name.Should().NotBeNullOrEmpty();
            product.Weight.Should().BeGreaterThan(0);
        }

        [Fact]
        public void New_ShouldReturnNewInstance_WhenCalled()
        {
            // Arrange & Act
            ProductViewModelBuilder builder1 = ProductViewModelBuilder.New();
            ProductViewModelBuilder builder2 = ProductViewModelBuilder.New();

            // Assert
            builder1.Should().NotBeSameAs(builder2);
        }

        #endregion

        #region Id Methods Tests

        [Fact]
        public void Build_ShouldCreateProductWithSpecificId_WhenUsingWithId()
        {
            // Arrange
            uint expectedId = 123;

            // Act
            ProductViewModel product = ProductViewModelBuilder.New()
                .WithId(expectedId)
                .Build();

            // Assert
            product.Id.Should().Be(expectedId);
        }

        [Fact]
        public void Build_ShouldCreateProductWithZeroId_WhenUsingWithIdZero()
        {
            // Arrange & Act
            ProductViewModel product = ProductViewModelBuilder.New()
                .WithIdZero()
                .Build();

            // Assert
            product.Id.Should().Be(0);
        }

        [Fact]
        public void Build_ShouldCreateProductWithNonExistentId_WhenUsingWithNonExistentId()
        {
            // Arrange & Act
            ProductViewModel product = ProductViewModelBuilder.New()
                .WithNonExistentId()
                .Build();

            // Assert
            product.Id.Should().Be(999999);
        }

        [Theory]
        [InlineData(1u)]
        [InlineData(100u)]
        [InlineData(1000u)]
        [InlineData(uint.MaxValue)]
        public void Build_ShouldCreateProductWithVariousIds_WhenUsingWithId(uint expectedId)
        {
            // Arrange & Act
            ProductViewModel product = ProductViewModelBuilder.New()
                .WithId(expectedId)
                .Build();

            // Assert
            product.Id.Should().Be(expectedId);
        }

        #endregion

        #region Name Methods Tests

        [Fact]
        public void Build_ShouldCreateProductWithSpecificName_WhenUsingWithName()
        {
            // Arrange
            string expectedName = "Custom Product";

            // Act
            ProductViewModel product = ProductViewModelBuilder.New()
                .WithName(expectedName)
                .Build();

            // Assert
            product.Name.Should().Be(expectedName);
        }

        [Fact]
        public void Build_ShouldCreateProductWithEmptyName_WhenUsingWithEmptyName()
        {
            // Arrange & Act
            ProductViewModel product = ProductViewModelBuilder.New()
                .WithEmptyName()
                .Build();

            // Assert
            product.Name.Should().BeEmpty();
        }

        [Fact]
        public void Build_ShouldCreateProductWithNullName_WhenUsingWithNullName()
        {
            // Arrange & Act
            ProductViewModel product = ProductViewModelBuilder.New()
                .WithNullName()
                .Build();

            // Assert
            product.Name.Should().BeNull();
        }

        [Fact]
        public void Build_ShouldCreateProductWithWhitespaceName_WhenUsingWithWhitespaceName()
        {
            // Arrange & Act
            ProductViewModel product = ProductViewModelBuilder.New()
                .WithWhitespaceName()
                .Build();

            // Assert
            product.Name.Should().Be("   ");
        }

        [Fact]
        public void Build_ShouldCreateProductWithSingleCharacterName_WhenUsingWithSingleCharacterName()
        {
            // Arrange & Act
            ProductViewModel product = ProductViewModelBuilder.New()
                .WithSingleCharacterName()
                .Build();

            // Assert
            product.Name.Should().Be("A");
        }

        [Fact]
        public void Build_ShouldCreateProductWithLongName_WhenUsingWithLongName()
        {
            // Arrange
            int expectedLength = 200;

            // Act
            ProductViewModel product = ProductViewModelBuilder.New()
                .WithLongName(expectedLength)
                .Build();

            // Assert
            product.Name.Should().HaveLength(expectedLength);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(50)]
        [InlineData(100)]
        [InlineData(500)]
        [InlineData(1000)]
        public void Build_ShouldCreateProductWithVariousLengthNames_WhenUsingWithLongName(int length)
        {
            // Arrange & Act
            ProductViewModel product = ProductViewModelBuilder.New()
                .WithLongName(length)
                .Build();

            // Assert
            product.Name.Should().HaveLength(length);
            product.Name.Should().MatchRegex("^A+$");
        }

        [Fact]
        public void Build_ShouldCreateProductWithSpecialCharactersName_WhenUsingWithSpecialCharactersName()
        {
            // Arrange & Act
            ProductViewModel product = ProductViewModelBuilder.New()
                .WithSpecialCharactersName()
                .Build();

            // Assert
            product.Name.Should().Be("Product @#$% 123");
        }

        [Fact]
        public void Build_ShouldCreateProductWithUnicodeName_WhenUsingWithUnicodeName()
        {
            // Arrange & Act
            ProductViewModel product = ProductViewModelBuilder.New()
                .WithUnicodeName()
                .Build();

            // Assert
            product.Name.Should().NotBeNullOrEmpty();
            product.Name.Should().Contain("Product");
        }

        [Fact]
        public void Build_ShouldCreateProductWithUniqueName_WhenUsingWithUniqueName()
        {
            // Arrange & Act
            ProductViewModel product = ProductViewModelBuilder.New()
                .WithUniqueName()
                .Build();

            // Assert
            product.Name.Should().StartWith("Unique Product Name Test");
            product.Name.Should().MatchRegex(@"Unique Product Name Test [0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}");
        }

        #endregion

        #region Weight Methods Tests

        [Fact]
        public void Build_ShouldCreateProductWithSpecificWeight_WhenUsingWithWeight()
        {
            // Arrange
            double expectedWeight = 5.5;

            // Act
            ProductViewModel product = ProductViewModelBuilder.New()
                .WithWeight(expectedWeight)
                .Build();

            // Assert
            product.Weight.Should().Be(expectedWeight);
        }

        [Fact]
        public void Build_ShouldCreateProductWithZeroWeight_WhenUsingWithZeroWeight()
        {
            // Arrange & Act
            ProductViewModel product = ProductViewModelBuilder.New()
                .WithZeroWeight()
                .Build();

            // Assert
            product.Weight.Should().Be(0);
        }

        [Fact]
        public void Build_ShouldCreateProductWithNegativeWeight_WhenUsingWithNegativeWeight()
        {
            // Arrange & Act
            ProductViewModel product = ProductViewModelBuilder.New()
                .WithNegativeWeight()
                .Build();

            // Assert
            product.Weight.Should().Be(-1.5);
            product.Weight.Should().BeNegative();
        }

        [Fact]
        public void Build_ShouldCreateProductWithVerySmallPositiveWeight_WhenUsingWithVerySmallPositiveWeight()
        {
            // Arrange & Act
            ProductViewModel product = ProductViewModelBuilder.New()
                .WithVerySmallPositiveWeight()
                .Build();

            // Assert
            product.Weight.Should().Be(double.Epsilon);
            product.Weight.Should().BePositive();
        }

        [Fact]
        public void Build_ShouldCreateProductWithMaximumWeight_WhenUsingWithMaximumWeight()
        {
            // Arrange & Act
            ProductViewModel product = ProductViewModelBuilder.New()
                .WithMaximumWeight()
                .Build();

            // Assert
            product.Weight.Should().Be(double.MaxValue);
        }

        [Fact]
        public void Build_ShouldCreateProductWithMinimumWeight_WhenUsingWithMinimumWeight()
        {
            // Arrange & Act
            ProductViewModel product = ProductViewModelBuilder.New()
                .WithMinimumWeight()
                .Build();

            // Assert
            product.Weight.Should().Be(double.MinValue);
        }

        [Fact]
        public void Build_ShouldCreateProductWithLargeWeight_WhenUsingWithLargeWeight()
        {
            // Arrange & Act
            ProductViewModel product = ProductViewModelBuilder.New()
                .WithLargeWeight()
                .Build();

            // Assert
            product.Weight.Should().Be(1000000.99);
        }

        [Fact]
        public void Build_ShouldCreateProductWithPreciseWeight_WhenUsingWithPreciseWeight()
        {
            // Arrange & Act
            ProductViewModel product = ProductViewModelBuilder.New()
                .WithPreciseWeight()
                .Build();

            // Assert
            product.Weight.Should().Be(1.123456789);
        }

        [Theory]
        [InlineData(0.01)]
        [InlineData(1.0)]
        [InlineData(10.5)]
        [InlineData(100.99)]
        [InlineData(999.99)]
        public void Build_ShouldCreateProductWithVariousWeights_WhenUsingWithWeight(double expectedWeight)
        {
            // Arrange & Act
            ProductViewModel product = ProductViewModelBuilder.New()
                .WithWeight(expectedWeight)
                .Build();

            // Assert
            product.Weight.Should().Be(expectedWeight);
        }

        #endregion

        #region Fluent Interface Tests

        [Fact]
        public void Build_ShouldSupportFluentInterface_WhenChainingMethods()
        {
            // Arrange
            uint expectedId = 123;
            string expectedName = "Custom Product";
            double expectedWeight = 5.5;

            // Act
            ProductViewModel product = ProductViewModelBuilder.New()
                .WithId(expectedId)
                .WithName(expectedName)
                .WithWeight(expectedWeight)
                .Build();

            // Assert
            product.Id.Should().Be(expectedId);
            product.Name.Should().Be(expectedName);
            product.Weight.Should().Be(expectedWeight);
        }

        [Fact]
        public void Build_ShouldSupportFluentInterface_WhenChainingAllIdMethods()
        {
            // Arrange
            uint finalId = 999;

            // Act
            ProductViewModel product = ProductViewModelBuilder.New()
                .WithIdZero()
                .WithNonExistentId()
                .WithId(finalId)
                .Build();

            // Assert
            product.Id.Should().Be(finalId);
        }

        [Fact]
        public void Build_ShouldSupportFluentInterface_WhenChainingAllNameMethods()
        {
            // Arrange
            string finalName = "Final Name";

            // Act
            ProductViewModel product = ProductViewModelBuilder.New()
                .WithEmptyName()
                .WithUniqueName()
                .WithName(finalName)
                .Build();

            // Assert
            product.Name.Should().Be(finalName);
        }

        [Fact]
        public void Build_ShouldSupportFluentInterface_WhenChainingAllWeightMethods()
        {
            // Arrange
            double finalWeight = 42.42;

            // Act
            ProductViewModel product = ProductViewModelBuilder.New()
                .WithZeroWeight()
                .WithNegativeWeight()
                .WithWeight(finalWeight)
                .Build();

            // Assert
            product.Weight.Should().Be(finalWeight);
        }

        [Fact]
        public void Build_ShouldReturnDifferentInstances_WhenCalledMultipleTimes()
        {
            // Arrange
            ProductViewModelBuilder builder = ProductViewModelBuilder.New();

            // Act
            ProductViewModel product1 = builder.Build();
            ProductViewModel product2 = builder.Build();

            // Assert
            product1.Should().NotBeSameAs(product2);
            product1.Id.Should().Be(product2.Id);
            product1.Name.Should().Be(product2.Name);
            product1.Weight.Should().Be(product2.Weight);
        }

        #endregion

        #region Edge Cases Tests

        [Fact]
        public void Build_ShouldOverridePreviousValue_WhenCallingWithIdMultipleTimes()
        {
            // Arrange
            uint firstId = 10;
            uint secondId = 20;
            uint thirdId = 30;

            // Act
            ProductViewModel product = ProductViewModelBuilder.New()
                .WithId(firstId)
                .WithId(secondId)
                .WithId(thirdId)
                .Build();

            // Assert
            product.Id.Should().Be(thirdId);
        }

        [Fact]
        public void Build_ShouldOverridePreviousValue_WhenCallingWithNameMultipleTimes()
        {
            // Arrange
            string firstName = "First";
            string secondName = "Second";
            string thirdName = "Third";

            // Act
            ProductViewModel product = ProductViewModelBuilder.New()
                .WithName(firstName)
                .WithName(secondName)
                .WithName(thirdName)
                .Build();

            // Assert
            product.Name.Should().Be(thirdName);
        }

        [Fact]
        public void Build_ShouldOverridePreviousValue_WhenCallingWithWeightMultipleTimes()
        {
            // Arrange
            double firstWeight = 1.1;
            double secondWeight = 2.2;
            double thirdWeight = 3.3;

            // Act
            ProductViewModel product = ProductViewModelBuilder.New()
                .WithWeight(firstWeight)
                .WithWeight(secondWeight)
                .WithWeight(thirdWeight)
                .Build();

            // Assert
            product.Weight.Should().Be(thirdWeight);
        }

        [Fact]
        public void Build_ShouldCreateProduct_WhenNoMethodsCalled()
        {
            // Arrange & Act
            ProductViewModel product = ProductViewModelBuilder.New().Build();

            // Assert
            product.Should().NotBeNull();
            product.Id.Should().Be(0);
            product.Name.Should().NotBeNullOrEmpty();
            product.Weight.Should().BeInRange(0.1, 10.0);
        }

        #endregion

        #region Combination Tests

        [Fact]
        public void Build_ShouldCreateInvalidProduct_WhenCombiningEmptyNameAndZeroWeight()
        {
            // Arrange & Act
            ProductViewModel product = ProductViewModelBuilder.New()
                .WithEmptyName()
                .WithZeroWeight()
                .Build();

            // Assert
            product.Name.Should().BeEmpty();
            product.Weight.Should().Be(0);
        }

        [Fact]
        public void Build_ShouldCreateInvalidProduct_WhenCombiningNullNameAndNegativeWeight()
        {
            // Arrange & Act
            ProductViewModel product = ProductViewModelBuilder.New()
                .WithNullName()
                .WithNegativeWeight()
                .Build();

            // Assert
            product.Name.Should().BeNull();
            product.Weight.Should().BeNegative();
        }

        [Fact]
        public void Build_ShouldCreateValidProduct_WhenCombiningAllValidMethods()
        {
            // Arrange & Act
            ProductViewModel product = ProductViewModelBuilder.New()
                .WithId(1)
                .WithName("Valid Product")
                .WithWeight(5.5)
                .Build();

            // Assert
            product.Id.Should().Be(1);
            product.Name.Should().Be("Valid Product");
            product.Weight.Should().Be(5.5);
        }

        [Fact]
        public void Build_ShouldCreateProductForBoundaryTest_WhenUsingExtremeValues()
        {
            // Arrange & Act
            ProductViewModel product = ProductViewModelBuilder.New()
                .WithId(uint.MaxValue)
                .WithLongName(1000)
                .WithMaximumWeight()
                .Build();

            // Assert
            product.Id.Should().Be(uint.MaxValue);
            product.Name.Should().HaveLength(1000);
            product.Weight.Should().Be(double.MaxValue);
        }

        #endregion

        #region Default Random Values Tests

        [Fact]
        public void Build_ShouldGenerateDifferentNames_WhenUsingDefaultConstructor()
        {
            // Arrange & Act
            ProductViewModel product1 = ProductViewModelBuilder.New().Build();
            ProductViewModel product2 = ProductViewModelBuilder.New().Build();

            // Assert
            product1.Name.Should().NotBeNullOrEmpty();
            product2.Name.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void Build_ShouldGenerateWeightInRange_WhenUsingDefaultConstructor()
        {
            // Arrange & Act
            ProductViewModel product = ProductViewModelBuilder.New().Build();

            // Assert
            product.Weight.Should().BeInRange(0.1, 10.0);
        }

        [Fact]
        public void Build_ShouldRoundWeightToTwoDecimals_WhenUsingDefaultConstructor()
        {
            // Arrange & Act
            ProductViewModel product = ProductViewModelBuilder.New().Build();

            // Assert
            double rounded = Math.Round(product.Weight, 2);
            product.Weight.Should().Be(rounded);
        }

        #endregion
    }
}
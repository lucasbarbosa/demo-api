using DemoApi.Domain.Entities;
using DemoApi.Tests.Builders.Products;
using FluentAssertions;

namespace DemoApi.Application.Tests.Products.Builders
{
    public class ProductBuilderTests
    {
        #region Default Values Tests

        [Fact]
        public void Build_ShouldCreateValidProduct_WhenUsingDefaultValues()
        {
            // Arrange & Act
            Product product = ProductBuilder.New().Build();

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
            ProductBuilder builder1 = ProductBuilder.New();
            ProductBuilder builder2 = ProductBuilder.New();

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
            Product product = ProductBuilder.New()
                .WithId(expectedId)
                .Build();

            // Assert
            product.Id.Should().Be(expectedId);
        }

        [Theory]
        [InlineData(0u)]
        [InlineData(1u)]
        [InlineData(100u)]
        [InlineData(1000u)]
        [InlineData(uint.MaxValue)]
        public void Build_ShouldCreateProductWithVariousIds_WhenUsingWithId(uint expectedId)
        {
            // Arrange & Act
            Product product = ProductBuilder.New()
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
            Product product = ProductBuilder.New()
                .WithName(expectedName)
                .Build();

            // Assert
            product.Name.Should().Be(expectedName);
        }

        [Theory]
        [InlineData("A")]
        [InlineData("Product")]
        [InlineData("Very Long Product Name With Many Characters")]
        [InlineData("Product @#$% 123")]
        [InlineData("Produto ção ãé ñ中文")]
        public void Build_ShouldCreateProductWithVariousNames_WhenUsingWithName(string expectedName)
        {
            // Arrange & Act
            Product product = ProductBuilder.New()
                .WithName(expectedName)
                .Build();

            // Assert
            product.Name.Should().Be(expectedName);
        }

        #endregion

        #region Weight Methods Tests

        [Fact]
        public void Build_ShouldCreateProductWithSpecificWeight_WhenUsingWithWeight()
        {
            // Arrange
            double expectedWeight = 5.5;

            // Act
            Product product = ProductBuilder.New()
                .WithWeight(expectedWeight)
                .Build();

            // Assert
            product.Weight.Should().Be(expectedWeight);
        }

        [Theory]
        [InlineData(0.0)]
        [InlineData(0.01)]
        [InlineData(1.0)]
        [InlineData(10.5)]
        [InlineData(100.99)]
        [InlineData(999.99)]
        [InlineData(-1.5)]
        public void Build_ShouldCreateProductWithVariousWeights_WhenUsingWithWeight(double expectedWeight)
        {
            // Arrange & Act
            Product product = ProductBuilder.New()
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
            Product product = ProductBuilder.New()
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
        public void Build_ShouldReturnDifferentInstances_WhenCalledMultipleTimes()
        {
            // Arrange
            ProductBuilder builder = ProductBuilder.New();

            // Act
            Product product1 = builder.Build();
            Product product2 = builder.Build();

            // Assert
            product1.Should().NotBeSameAs(product2);
            product1.Id.Should().Be(product2.Id);
            product1.Name.Should().Be(product2.Name);
            product1.Weight.Should().Be(product2.Weight);
        }

        [Fact]
        public void Build_ShouldOverridePreviousValue_WhenCallingWithIdMultipleTimes()
        {
            // Arrange
            uint firstId = 10;
            uint secondId = 20;
            uint thirdId = 30;

            // Act
            Product product = ProductBuilder.New()
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
            Product product = ProductBuilder.New()
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
            Product product = ProductBuilder.New()
                .WithWeight(firstWeight)
                .WithWeight(secondWeight)
                .WithWeight(thirdWeight)
                .Build();

            // Assert
            product.Weight.Should().Be(thirdWeight);
        }

        #endregion

        #region Edge Cases Tests

        [Fact]
        public void Build_ShouldCreateProduct_WhenNoMethodsCalled()
        {
            // Arrange & Act
            Product product = ProductBuilder.New().Build();

            // Assert
            product.Should().NotBeNull();
            product.Id.Should().Be(0);
            product.Name.Should().NotBeNullOrEmpty();
            product.Weight.Should().BeInRange(0.1, 10.0);
        }

        [Fact]
        public void Build_ShouldCreateProductWithBoundaryValues_WhenUsingExtremeValues()
        {
            // Arrange & Act
            Product product = ProductBuilder.New()
                .WithId(uint.MaxValue)
                .WithName(new string('A', 1000))
                .WithWeight(double.MaxValue)
                .Build();

            // Assert
            product.Id.Should().Be(uint.MaxValue);
            product.Name.Should().HaveLength(1000);
            product.Weight.Should().Be(double.MaxValue);
        }

        [Fact]
        public void Build_ShouldCreateProductWithMinimumValues_WhenUsingMinimumBoundaries()
        {
            // Arrange & Act
            Product product = ProductBuilder.New()
                .WithId(0)
                .WithName("A")
                .WithWeight(double.Epsilon)
                .Build();

            // Assert
            product.Id.Should().Be(0);
            product.Name.Should().Be("A");
            product.Weight.Should().Be(double.Epsilon);
        }

        #endregion

        #region Combination Tests

        [Fact]
        public void Build_ShouldCreateValidProduct_WhenCombiningAllMethods()
        {
            // Arrange & Act
            Product product = ProductBuilder.New()
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
        public void Build_ShouldCreateProductWithZeroValues_WhenSpecified()
        {
            // Arrange & Act
            Product product = ProductBuilder.New()
                .WithId(0)
                .WithWeight(0)
                .Build();

            // Assert
            product.Id.Should().Be(0);
            product.Weight.Should().Be(0);
        }

        [Fact]
        public void Build_ShouldCreateProductWithNegativeWeight_WhenSpecified()
        {
            // Arrange & Act
            Product product = ProductBuilder.New()
                .WithWeight(-10.5)
                .Build();

            // Assert
            product.Weight.Should().Be(-10.5);
            product.Weight.Should().BeNegative();
        }

        #endregion

        #region Default Random Values Tests

        [Fact]
        public void Build_ShouldGenerateNameFromBogus_WhenUsingDefaultConstructor()
        {
            // Arrange & Act
            Product product = ProductBuilder.New().Build();

            // Assert
            product.Name.Should().NotBeNullOrEmpty();
            product.Name.Should().NotBe("Product"); // Should be a Bogus-generated name
        }

        [Fact]
        public void Build_ShouldGenerateWeightInRange_WhenUsingDefaultConstructor()
        {
            // Arrange & Act
            Product product = ProductBuilder.New().Build();

            // Assert
            product.Weight.Should().BeInRange(0.1, 10.0);
        }

        [Fact]
        public void Build_ShouldRoundWeightToTwoDecimals_WhenUsingDefaultConstructor()
        {
            // Arrange & Act
            Product product = ProductBuilder.New().Build();

            // Assert
            double rounded = Math.Round(product.Weight, 2);
            product.Weight.Should().Be(rounded);
        }

        [Fact]
        public void Build_ShouldGenerateConsistentValues_WhenCalledOnSameBuilder()
        {
            // Arrange
            ProductBuilder builder = ProductBuilder.New();

            // Act
            Product product1 = builder.Build();
            Product product2 = builder.Build();

            // Assert
            product1.Name.Should().Be(product2.Name);
            product1.Weight.Should().Be(product2.Weight);
        }

        #endregion

        #region Type Safety Tests

        [Fact]
        public void Build_ShouldReturnProductEntity_NotViewModel()
        {
            // Arrange & Act
            Product product = ProductBuilder.New().Build();

            // Assert
            product.Should().BeOfType<Product>();
            product.Should().NotBeNull();
        }

        [Fact]
        public void Build_ShouldInheritFromEntity_WhenCreated()
        {
            // Arrange & Act
            Product product = ProductBuilder.New().Build();

            // Assert
            product.Should().BeAssignableTo<Entity>();
        }

        #endregion
    }
}
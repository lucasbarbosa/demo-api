using Bogus;
using DemoApi.Application.Models.Products;

namespace DemoApi.Tests.Builders.Products
{
    public class ProductViewModelBuilder
    {
        #region Properties

        private uint _id = 0;
        private string _name;
        private double _weight;
        private static readonly Faker _faker = new();

        #endregion

        #region Constructors

        public ProductViewModelBuilder()
        {
            _name = _faker.Commerce.ProductName();
            _weight = Math.Round(_faker.Random.Double(0.1, 10.0), 2);
        }

        #endregion

        #region Id Methods

        public ProductViewModelBuilder WithId(uint id)
        {
            _id = id;
            return this;
        }

        public ProductViewModelBuilder WithIdZero()
        {
            _id = 0;
            return this;
        }

        public ProductViewModelBuilder WithNonExistentId()
        {
            _id = 999999;
            return this;
        }

        #endregion

        #region Name Methods

        public ProductViewModelBuilder WithName(string name)
        {
            _name = name;
            return this;
        }

        public ProductViewModelBuilder WithEmptyName()
        {
            _name = string.Empty;
            return this;
        }

        public ProductViewModelBuilder WithNullName()
        {
            _name = null!;
            return this;
        }

        public ProductViewModelBuilder WithWhitespaceName()
        {
            _name = "   ";
            return this;
        }

        public ProductViewModelBuilder WithSingleCharacterName()
        {
            _name = "A";
            return this;
        }

        public ProductViewModelBuilder WithLongName(int length = 100)
        {
            _name = new string('A', length);
            return this;
        }

        public ProductViewModelBuilder WithSpecialCharactersName()
        {
            _name = "Product @#$% 123";
            return this;
        }

        public ProductViewModelBuilder WithUnicodeName()
        {
            _name = "Product ção ãé ñ??";
            return this;
        }

        public ProductViewModelBuilder WithUniqueName()
        {
            _name = $"Unique Product Name Test {Guid.NewGuid()}";
            return this;
        }

        #endregion

        #region Weight Methods

        public ProductViewModelBuilder WithWeight(double weight)
        {
            _weight = weight;
            return this;
        }

        public ProductViewModelBuilder WithZeroWeight()
        {
            _weight = 0;
            return this;
        }

        public ProductViewModelBuilder WithNegativeWeight()
        {
            _weight = -1.5;
            return this;
        }

        public ProductViewModelBuilder WithVerySmallPositiveWeight()
        {
            _weight = double.Epsilon;
            return this;
        }

        public ProductViewModelBuilder WithMaximumWeight()
        {
            _weight = double.MaxValue;
            return this;
        }

        public ProductViewModelBuilder WithMinimumWeight()
        {
            _weight = double.MinValue;
            return this;
        }

        public ProductViewModelBuilder WithLargeWeight()
        {
            _weight = 1000000.99;
            return this;
        }

        public ProductViewModelBuilder WithPreciseWeight()
        {
            _weight = 1.123456789;
            return this;
        }

        #endregion

        #region Build Methods

        public ProductViewModel Build()
        {
            return new ProductViewModel
            {
                Id = _id,
                Name = _name,
                Weight = _weight
            };
        }

        public static ProductViewModelBuilder New() => new();

        #endregion
    }
}
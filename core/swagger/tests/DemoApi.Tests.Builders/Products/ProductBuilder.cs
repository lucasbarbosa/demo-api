namespace DemoApi.Tests.Builders.Products;

using Bogus;
using DemoApi.Domain.Entities;

public class ProductBuilder
{
    #region Properties

    private uint _id = 0;
    private string _name;
    private double _weight;
    private static readonly Faker _faker = new();

    #endregion

    #region Constructors

    public ProductBuilder()
    {
        _name = _faker.Commerce.ProductName();
        _weight = Math.Round(_faker.Random.Double(0.1, 10.0), 2);
    }

    #endregion

    #region Public Methods

    public ProductBuilder WithId(uint id)
    {
        _id = id;
        return this;
    }

    public ProductBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public ProductBuilder WithWeight(double weight)
    {
        _weight = weight;
        return this;
    }

    public Product Build()
    {
        return new Product
        {
            Id = _id,
            Name = _name,
            Weight = _weight
        };
    }

    public static ProductBuilder New() => new();

    #endregion
}
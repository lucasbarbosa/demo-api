namespace DemoApi.Domain.Entities;

public class Product : Entity
{
    #region Properties

    public uint Id { get; set; }

    public required string Name { get; set; }

    public double Weight { get; set; }

    #endregion
}
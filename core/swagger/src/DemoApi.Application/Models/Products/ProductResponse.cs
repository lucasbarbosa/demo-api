namespace DemoApi.Application.Models.Products;

public class ProductResponse : ResponseViewModel
{
    public new ProductViewModel? Data { get; set; }
}
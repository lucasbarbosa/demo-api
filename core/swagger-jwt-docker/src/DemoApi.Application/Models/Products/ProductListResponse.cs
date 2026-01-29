namespace DemoApi.Application.Models.Products;

public class ProductListResponse : ResponseViewModel
{
    public new IList<ProductViewModel>? Data { get; set; }
}
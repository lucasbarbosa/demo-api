namespace DemoApi.Application.Automapper;

using AutoMapper;
using DemoApi.Application.Models.Products;
using DemoApi.Domain.Entities;
public class AutomapperConfig : Profile
{
    #region Constructors

    public AutomapperConfig()
    {
        CreateMap<Product, ProductViewModel>().ReverseMap();
    }

    #endregion
}
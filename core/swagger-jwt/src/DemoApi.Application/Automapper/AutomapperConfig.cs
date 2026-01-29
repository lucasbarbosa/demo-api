using AutoMapper;

using DemoApi.Application.Models.Products;
using DemoApi.Domain.Entities;

namespace DemoApi.Application.Automapper;

public class AutomapperConfig : Profile
{
    #region Constructors

    public AutomapperConfig()
    {
        CreateMap<Product, ProductViewModel>().ReverseMap();
    }

    #endregion
}
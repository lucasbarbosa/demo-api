namespace DemoApi.Application.Services;

using AutoMapper;
using DemoApi.Application.Interfaces;
using DemoApi.Application.Models.Products;
using DemoApi.Domain.Entities;
using DemoApi.Domain.Interfaces;
public class ProductAppService(
    IMapper mapper, INotificatorHandler notificator, IProductRepository productRepository) : BaseServices, IProductAppService
{
    #region Properties

    private readonly IMapper _mapper = mapper;
    private readonly INotificatorHandler _notificator = notificator;
    private readonly IProductRepository _productRepository = productRepository;

    #endregion
    
    #region Public Methods

    public async Task<IList<ProductViewModel>> GetAll()
    {
        IList<ProductViewModel> response = _mapper.Map<IList<ProductViewModel>>(await _productRepository.GetAll());

        return response;
    }

    public async Task<ProductViewModel?> GetById(uint id)
    {
        ProductViewModel? response = _mapper.Map<ProductViewModel>(await _productRepository.GetById(id));

        if (response is null)
        {
            _notificator.AddError("Product was not found");
            return null;
        }

        return response;
    }

    public async Task<ProductViewModel?> Create(ProductViewModel product)
    {
        ProductViewModel? response = null;

        if (product is null)
        {
            _notificator.AddError("Product could not be created");
            return response;
        }

        if (await _productRepository.GetByName(product.Name!) is not null)
        {
            _notificator.AddError($"Product ({product.Name!}) is already registered");
            return response;
        }

        response = _mapper.Map<ProductViewModel>(await _productRepository.Create(_mapper.Map<Product>(product)));

        if (response is null)
            _notificator.AddError("Product could not be created");
        
        return response;
    }

    public async Task<bool> Update(ProductViewModel product)
    {
        bool response = false;

        if (product is null)
        {
            _notificator.AddError("Product could not be updated");
            return response;
        }

        if (await _productRepository.GetById(product.Id) is null)
        {
            _notificator.AddError("Product was not found");
            return false;
        }

        response = await _productRepository.Update(_mapper.Map<Product>(product));

        if (response is false)
            _notificator.AddError("Product could not be updated");

        return response;
    }

    public async Task<bool> DeleteById(uint id)
    {
        bool response = false;

        if (await _productRepository.GetById(id) is null)
        {
            _notificator.AddError("Product was not found");
            return response;
        }

        response = await _productRepository.DeleteById(id);

        if (response is false)
            _notificator.AddError("Product could not be deleted");

        return response;
    }

    #endregion
}
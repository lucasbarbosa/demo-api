using Asp.Versioning;
using DemoApi.Api.Controllers;
using DemoApi.Application.Interfaces;
using DemoApi.Application.Models;
using DemoApi.Application.Models.Products;
using DemoApi.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace DemoApi.Api.V1.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/products")]
    [Produces("application/json")]
    public class ProductController(INotificatorHandler notificator, IProductAppService productApplication) : MainApiController(notificator)
    {
        #region Properties

        private readonly IProductAppService _productApplication = productApplication;

        #endregion
        
        #region Public Methods

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseViewModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseViewModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseViewModel), StatusCodes.Status412PreconditionFailed)]
        public async Task<IActionResult> GetById(uint id)
        {
            ProductViewModel? product = await _productApplication.GetById(id);

            return CustomResponse(product);
        }

        [HttpGet]
        [ProducesResponseType(typeof(ProductListResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseViewModel), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAll()
        {
            IList<ProductViewModel> products = await _productApplication.GetAll();

            return CustomResponse(products);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ResponseViewModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseViewModel), StatusCodes.Status412PreconditionFailed)]
        public async Task<IActionResult> Create([FromBody] ProductViewModel product)
        {
            if (ModelState.IsValid is false) return CustomResponse(ModelState);

            ProductViewModel? response = await _productApplication.Create(product);

            return CustomResponseCreate(response);
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ResponseViewModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseViewModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseViewModel), StatusCodes.Status412PreconditionFailed)]
        public async Task<IActionResult> Update([FromBody] ProductViewModel product)
        {
            if (ModelState.IsValid is false) return CustomResponse(ModelState);

            return await _productApplication.Update(product)
                ? CustomResponse(HttpStatusCode.NoContent, true)
                : CustomResponse();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ResponseViewModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseViewModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseViewModel), StatusCodes.Status412PreconditionFailed)]
        public async Task<IActionResult> Delete(uint id)
        {
            return await _productApplication.DeleteById(id)
                ? CustomResponse(HttpStatusCode.NoContent, true)
                : CustomResponse();
        }

        #endregion
    }
}
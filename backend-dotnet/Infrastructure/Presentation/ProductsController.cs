using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Presentation.Attributes;
using Services.Abstractions;
using Shared;
using Shared.DTOS.ProductDTOS;
using Shared.ErrorModels;
using System.Threading.Tasks;
namespace Presentation
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController(IServiceManager _serviceManager) : ControllerBase
    {
        [HttpGet] // GET : BaseURL/api/products
        [ProducesResponseType(StatusCodes.Status200OK,Type = typeof(PaginationResponse<ProductResponse>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError,Type = typeof(ErrorDetails))]
        [ProducesResponseType(StatusCodes.Status400BadRequest,Type = typeof(ErrorDetails))]
        [Cache(50)]
        public async Task<ActionResult<PaginationResponse<ProductResponse>>> GetAllProducts([FromQuery] ProductQueryParameters parameters)
        {
            var result  = await _serviceManager.ProductService.GetAllProductAsync(parameters);
            if (result is null) return BadRequest(); //400
            return Ok(result); //200
        }
        [HttpGet("{id}")] // GET : BaseURL/api/products/[id]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProductResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorDetails))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorDetails))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorDetails))]
        [Cache(50)]

        public async Task<ActionResult<ProductResponse>> GetProductbyId(int? id)
        {
            if (id is null) return BadRequest();
            var result = await _serviceManager.ProductService.GetProductByIdAsync(id.Value);
            return Ok(result); //200
        }
        [HttpGet("brands")] // GET : BaseURL/api/products/brands
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<BrandTypeResponse>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorDetails))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorDetails))]
        [Cache(50)]

        public async Task<ActionResult<IEnumerable<BrandTypeResponse>>> GetAllBrands()
        {
            var result = await _serviceManager.ProductService.GetAllBrandsAsync();
            if (result is null) return NotFound(); //400
            return Ok(result); //200
        }
        [HttpGet("types")] // GET : BaseURL/api/products/types
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<BrandTypeResponse>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorDetails))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorDetails))]
        [Cache(50)]

        public async Task<ActionResult<IEnumerable<BrandTypeResponse>>> GetAllTypes()
        {
            var result = await _serviceManager.ProductService.GetAllTypesAsync();
            if (result is null) return NotFound(); //400
            return Ok(result); //200
        }
    }
}

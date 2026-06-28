using Microsoft.AspNetCore.Mvc;
using Services.Abstractions;
using Shared.DTOS.BasketDTOS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation
{
    [ApiController]
    [Route("api/[controller]")]
    public class BasketsController(IServiceManager _serviceManager) : ControllerBase
    {
        [HttpGet]// Get :  baseUrl/api/baskets?id
        public async Task<IActionResult> GetBasketById(string id)
        {
            var result = await _serviceManager.BasketService.GetBasketAsync(id);
            return Ok(result);
        }
        [HttpPost]// Post :  baseUrl/api/baskets
        public async Task<IActionResult> CreateOrUpdateBasket(BasketDTO basketDTO)
        {
            var result = await _serviceManager.BasketService.CreateBasketAsync(basketDTO,TimeSpan.FromDays(1));
            return Ok(result);
        }
        [HttpDelete]// Get :  baseUrl/api/baskets?id
        public async Task<IActionResult> DeleteBasket(string id)
        {
            var result = await _serviceManager.BasketService.DeleteBasketAsync(id);
            return NoContent(); //204
        }
    }
}

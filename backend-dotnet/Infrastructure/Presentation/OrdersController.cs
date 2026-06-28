using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Abstractions;
using Shared.DTOS.OrderDTOS;
using System.Security.Claims;

namespace Presentation
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController(IServiceManager _serviceManager) : ControllerBase
    {
        //Create Order
        [HttpPost] // Post : BaseUel/api/orders 
        [Authorize]
        public async Task<IActionResult> CreateOrder(OrderRequest request)
        {
            var userEmailClaim = User.FindFirst(ClaimTypes.Email);
            var result = await _serviceManager.OrderService.CreateOrderAsync(request,userEmailClaim.Value);
            return Ok(result);  
        }
        //Get All Delivery Methods
        [HttpGet("deliveryMethods")] // Get : BaseUel/api/orders/deliveryMethods
        public async Task<IActionResult> GetAllDeliveryMethods()
        {
            var result = await  _serviceManager.OrderService.GetAllDeliveryMethodsAsync();
            return Ok(result);
        }
        //Get All Delivery Methods
        [HttpGet] // Get : BaseUel/api/orders
        [Authorize]
        public async Task<IActionResult> GetOrdersForSpecificUser()
        {
            var userEmailClaim = User.FindFirst(ClaimTypes.Email);
            var result = await _serviceManager.OrderService.GetOrdersForSpecificUserAsync(userEmailClaim.Value);
            return Ok(result);
        }
        [HttpGet("{id}")] // Get : BaseUel/api/orders/{id}
        [Authorize]
        public async Task<IActionResult> GetOrderByIdForSpecificUser(Guid id)
        {
            var userEmailClaim = User.FindFirst(ClaimTypes.Email);
            var result = await _serviceManager.OrderService.GetOrderByIdForSpecificUserAsync(id,userEmailClaim.Value);
            return Ok(result);
        }
    }
}

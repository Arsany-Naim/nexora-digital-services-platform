using Microsoft.AspNetCore.Mvc;
using Services.Abstractions;
using Shared.DTOS.BasketDTOS;
using Stripe;

namespace Presentation
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentsController(IServiceManager _serviceManager) : ControllerBase
    {
        [HttpPost("{basketId}")]
        public async Task<ActionResult<BasketDTO>> CreateOrUpdate(string basketId)
        {
            return Ok(await _serviceManager.PaymentService.CreateOrUpdatePaymentIntent(basketId));
        }


        [HttpPost("webhook")]
        public async Task<IActionResult> WebHook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            await _serviceManager.PaymentService.UpdateOrderPaymentStatusAsync(json,
                Request.Headers["Stripe-Signature"]!);
            return new EmptyResult();
        }
    }
}

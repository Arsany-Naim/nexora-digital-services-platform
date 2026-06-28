using Shared.DTOS.BasketDTOS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Abstractions.Payments
{
    public interface IPaymentService
    {
        Task<BasketDTO> CreateOrUpdatePaymentIntent(string BasketId);
        Task UpdateOrderPaymentStatusAsync(string jsonRequest, string stripeHeader);
        Task UpdatePaymentFailedAsync(string paymentIntentId);
        Task UpdatePaymentReceivedAsync(string paymentIntentId);
    }
}

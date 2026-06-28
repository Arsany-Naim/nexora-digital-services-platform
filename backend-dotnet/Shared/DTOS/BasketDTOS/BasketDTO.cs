using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOS.BasketDTOS
{
    public class BasketDTO
    {
        public string Id { get; set; }
        public List<BasketItemDTO> Items { get; set; }
        public int? DeliveryMethodId { get; set; }
        public string? PaymentIntentId { get; set; }
        public string? ClientSecret { get; set; }
        public decimal? ShippingCost { get; set; }
    }
}

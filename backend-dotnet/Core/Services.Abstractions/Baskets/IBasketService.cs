using Shared.DTOS.BasketDTOS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Abstractions.Baskets
{
    public interface IBasketService
    {
        Task<BasketDTO?> GetBasketAsync(string id);
        Task<BasketDTO?> CreateBasketAsync(BasketDTO dto,TimeSpan duration);
        Task<bool> DeleteBasketAsync(string id);
    }
}

using Domain.Entities.Baskets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Contract
{
    public interface IBasketRepository
    {
        Task<CustomerBasket?> GetBasketAsync(string id);
        Task<CustomerBasket?> CreateBasketAsync(CustomerBasket basket,TimeSpan duration);
        Task<bool> DeleteBasketAsync(string id);
    }
}

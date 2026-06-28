using AutoMapper;
using Domain.Contract;
using Domain.Entities.Baskets;
using Domain.Exceptions.BadRequest;
using Domain.Exceptions.NotFound;
using Services.Abstractions.Baskets;
using Shared.DTOS.BasketDTOS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Basket
{
    public class BasketService(IBasketRepository _basketRepository,IMapper _mapper) : IBasketService
    {
        public async Task<BasketDTO?> GetBasketAsync(string id)
        {
            var basket = await _basketRepository.GetBasketAsync(id);
            if(basket is null) throw new BasketNotFoundException(id);
            var result =  _mapper.Map<BasketDTO>(basket);
            return result;
        }
        public async Task<BasketDTO?> CreateBasketAsync(BasketDTO dto, TimeSpan duration)
        {
            var basket = _mapper.Map<CustomerBasket>(dto);
            var result = await _basketRepository.CreateBasketAsync(basket, duration);
            if(result is null) throw new CreateOrUpdateBasketBadRequestException();
            return _mapper.Map<BasketDTO>(result);
        }
        public async Task<bool> DeleteBasketAsync(string id)
        {
            var flag = await _basketRepository.DeleteBasketAsync(id);
            if(!flag) throw new DeleteBasketBadRequestException();
            return flag;
        }

    }
}

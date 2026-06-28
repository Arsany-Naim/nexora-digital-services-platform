using AutoMapper;
using Domain.Entities.Baskets;
using Shared.DTOS.BasketDTOS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Mapping.Baskets
{
    public class BasketProfile : Profile
    {
        public BasketProfile()
        {
            CreateMap<CustomerBasket,BasketDTO>().ReverseMap();
            CreateMap<BasketItem,BasketItemDTO>().ReverseMap();
        }
    }
}

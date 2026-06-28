using AutoMapper;
using Domain.Entities.Orders;
using Shared.DTOS.OrderDTOS;

namespace Services.Mapping.Order
{
    public class OrderProfile : Profile
    {
        public OrderProfile()
        {
            CreateMap<OrderAddress, OrderAddressDTO>().ReverseMap();

            CreateMap<Domain.Entities.Orders.Order, OrderResponse>().
                ForMember(D => D.DeliveryMethod, O => O.MapFrom(S => S.DeliveryMethod.ShortName)).
                ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString())).
                ForMember(D => D.Total, O => O.MapFrom(S => S.GetTotal()));

            CreateMap<OrderItem, OrderItemDTO>().
                ForMember(D => D.ProductId , S => S.MapFrom(O=>O.Product.ProductId)).
                ForMember(D => D.ProductName , S => S.MapFrom(O=>O.Product.ProductName)).
                ForMember(D => D.PictureUrl , S => S.MapFrom(O=>O.Product.PictureUrl));
            CreateMap<DeliveryMethod, DeliveryMethodResponse>();

        }
    }
}

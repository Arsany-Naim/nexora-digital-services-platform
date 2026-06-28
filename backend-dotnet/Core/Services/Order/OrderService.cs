using AutoMapper;
using Domain.Contract;
using Domain.Entities.Orders;
using Domain.Entities.Product;
using Domain.Exceptions.BadRequest;
using Domain.Exceptions.NotFound;
using Services.Abstractions.Orders;
using Services.Specifications.Orders;
using Shared.DTOS.OrderDTOS;
using System.Collections.Generic;
namespace Services.Order
{
    public class OrderService(IUnitOfWork _unitOfWork,IMapper _mapper,IBasketRepository _basketRepository) : IOrderService
    {
        public async Task<OrderResponse?> CreateOrderAsync(OrderRequest request, string userEmail)
        {
            //1-Get Shipping Address
            var shippingAddress = _mapper.Map<OrderAddress>(request.ShipToAddress);

            //2-Get Delivery Method By ID
            var deliveryMethod = await _unitOfWork.GetRepository<int,DeliveryMethod>().GetAsync(request.DeliveryMethodId);
            if(deliveryMethod == null) throw new DeliveryMethodNotFoundException(request.DeliveryMethodId);

            //3-Get Basket From Basket Service

            //3.1 Get Basket By ID
            var basket = await _basketRepository.GetBasketAsync(request.BasketId);
            if(basket == null) throw new BasketNotFoundException(request.BasketId);
            //3.2 Convert Every Basket Item To Order Item
            var OrderItems = new List<OrderItem>();
            foreach (var item in basket.Items)
            {
                //Check Price
                //Get Product From Db
                var product = await _unitOfWork.GetRepository<int, Product>().GetAsync(item.Id);
                if(product == null) throw new ProductNotFoundException(item.Id);
                if(product.Price != item.Price)
                {
                    //Update Price In Basket
                    item.Price = product.Price;
                }
                var productInOrderItem = new ProductInOrderItem(item.Id,item.ProductName,item.PictureUrl);
                var orderitem = new OrderItem(productInOrderItem,item.Price,item.Quantity);
                OrderItems.Add(orderitem);
            }
            //4-Calculate SubTotal
            var subTotal = OrderItems.Sum(item => item.Price * item.Quantity);
            //5-Check If Order Exits With Same Payment Intent Id
            var spec = new OrderWithpaymrntIntentSpecifications(basket.PaymentIntentId);
            var existingOrder = await _unitOfWork.GetRepository<Guid, Domain.Entities.Orders.Order>().GetAsync(spec);
            //Check Payment Intent Id
            if(existingOrder != null)
            {
                _unitOfWork.GetRepository<Guid, Domain.Entities.Orders.Order>().Delete(existingOrder);
            }
            //Create Ordeer
            var order = new Domain.Entities.Orders.Order(userEmail, shippingAddress,deliveryMethod,OrderItems,subTotal,basket.PaymentIntentId);
            //Create Order In Data Base
            await _unitOfWork.GetRepository<Guid,Domain.Entities.Orders.Order>().AddAsync(order);
            var count = await _unitOfWork.SaveChangesAsync();
            if(count <= 0) throw new CreateOrderBadRequestException();
            return _mapper.Map<OrderResponse>(order);
        }

        public async Task<IEnumerable<DeliveryMethodResponse>> GetAllDeliveryMethodsAsync()
        {
            var deliveryMethods = await _unitOfWork.GetRepository<int,DeliveryMethod>().GetAllAsync();
            return _mapper.Map<IEnumerable<DeliveryMethodResponse>>(deliveryMethods);
        }

        public async Task<OrderResponse?> GetOrderByIdForSpecificUserAsync(Guid Id, string userEmail)
        {
            var specs = new OrderSpecifications(Id, userEmail);
            var order = await _unitOfWork.GetRepository<Guid, Domain.Entities.Orders.Order>().GetAsync(specs,Id);
            if(order == null) throw new OrderNotFoundException(Id);
            return _mapper.Map<OrderResponse>(order);

        }

        public async Task<IEnumerable<OrderResponse>> GetOrdersForSpecificUserAsync(string userEmail)
        {
            var specs = new OrderSpecifications(userEmail);
            var order = await _unitOfWork.GetRepository<Guid, Domain.Entities.Orders.Order>().GetAllAsync(specs);
            return _mapper.Map<IEnumerable<OrderResponse>>(order);
        }
    }
}

using AutoMapper;
using Domain.Contract;
using Domain.Entities.Orders;
using Domain.Entities.Product;
using Domain.Exceptions.NotFound;
using Microsoft.Extensions.Configuration;
using Services.Abstractions.Payments;
using Services.Specifications.Orders;
using Shared.DTOS.BasketDTOS;
using Stripe;
using Product = Domain.Entities.Product.Product;

namespace Services.Payment
{
    public class PaymentService(IBasketRepository _basketRepository,IUnitOfWork _unitOfWork,IConfiguration _configuration,IMapper _mapper) : IPaymentService
    {
        public async Task<BasketDTO> CreateOrUpdatePaymentIntent(string basketId)
        {
            //StripeConfiguration.ApiKey = Environment.GetEnvironmentVariable("Key");
            //StripeConfiguration.ApiKey = configuration["Stripe:SecretKey"];
            StripeConfiguration.ApiKey = _configuration.GetRequiredSection("Stripe")["SecretKey"];
            // BasketRepo => Get Basket 
            var basket = await _basketRepository.GetBasketAsync(basketId)
                ?? throw new BasketNotFoundException(basketId);

            var productRepo = _unitOfWork.GetRepository<int,Product>();
            foreach (var item in basket.Items)
            {
                var product = await productRepo.GetAsync(item.Id)
                    ?? throw new ProductNotFoundException(item.Id);
                    item.Price = product.Price;
            }

            ArgumentNullException.ThrowIfNull(basket.DeliveryMethodId);

            var method = await _unitOfWork.GetRepository<int,DeliveryMethod>()
                .GetAsync(basket.DeliveryMethodId.Value)
                ?? throw new DeliveryMethodNotFoundException(basket.DeliveryMethodId.Value);

            basket.ShippingCost = method.Price;

            var amount = (long)(basket.Items.Sum(item => item.Quantity * item.Price) + method.Price) * 100;

            // Create || Update 
            var service = new PaymentIntentService();
            if (string.IsNullOrWhiteSpace(basket.PaymentIntentId)) // Create 
            {
                var options = new PaymentIntentCreateOptions
                {
                    Amount = amount,
                    Currency = "usd",
                    PaymentMethodTypes = new List<string> { "card" }
                };

                var paymentIntent = await service.CreateAsync(options);

                basket.PaymentIntentId = paymentIntent.Id;
                basket.ClientSecret = paymentIntent.ClientSecret;
            }
            else // Update 
            {
                var options = new PaymentIntentUpdateOptions
                {
                    Amount = amount,
                };

                await service.UpdateAsync(basket.PaymentIntentId, options);
            }
            await _basketRepository.CreateBasketAsync(basket,TimeSpan.FromDays(1));
            return _mapper.Map<BasketDTO>(basket);
        }
        public async Task UpdateOrderPaymentStatusAsync(string jsonRequest, string stripeHeader)
        {
            var endpointSecret = _configuration.GetRequiredSection("Stripe")["EndPointSecret"];
            var stripeEvent = EventUtility.ConstructEvent(jsonRequest,stripeHeader, endpointSecret);

            var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
            switch (stripeEvent.Type)
            {
                case EventTypes.PaymentIntentPaymentFailed:
                    await UpdatePaymentFailedAsync(paymentIntent.Id);
                    break;
                case EventTypes.PaymentIntentSucceeded:
                    await UpdatePaymentReceivedAsync(paymentIntent.Id);
                    break;
                // ... handle other event types
                default:
                    Console.WriteLine("Unhandled event type: {0}", stripeEvent.Type);
                    break;
            }

        }
        public async Task UpdatePaymentReceivedAsync(string paymentIntentId)
        {
            var order = await _unitOfWork.GetRepository<Guid,Domain.Entities.Orders.Order>()
                .GetAsync(new OrderWithpaymrntIntentSpecifications(paymentIntentId));

            order.Status = OrderStatus.PaymentSuccess;

            _unitOfWork.GetRepository<Guid, Domain.Entities.Orders.Order>().Update(order);

            await _unitOfWork.SaveChangesAsync();
        }
        public async Task UpdatePaymentFailedAsync(string paymentIntentId)
        {
            var order = await _unitOfWork.GetRepository<Guid, Domain.Entities.Orders.Order>()
                .GetAsync(new OrderWithpaymrntIntentSpecifications(paymentIntentId));

            order.Status = OrderStatus.PaymentFailed;

            _unitOfWork.GetRepository<Guid, Domain.Entities.Orders.Order>().Update(order);

            await _unitOfWork.SaveChangesAsync();
        }

    }
}

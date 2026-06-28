using AutoMapper;
using Domain.Contract;
using Domain.Entities.Identities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Services.Abstractions;
using Services.Abstractions.Auth;
using Services.Abstractions.Baskets;
using Services.Abstractions.Cache;
using Services.Abstractions.Orders;
using Services.Abstractions.Payments;
using Services.Abstractions.Products;
using Services.Auth;
using Services.Basket;
using Services.Cache;
using Services.Order;
using Services.Payment;
using Services.ProductServices;
using Shared;

namespace Services
{
    public class ServiceManager(IUnitOfWork _unitOfWork , IMapper _mapper , IBasketRepository _basketRepository,ICacheRepository _cacheRepository,UserManager<AppUser> _appUser,IOptions<JwtOptions> _options,IConfiguration _configuration) : IServiceManager
    {
        public IProductService ProductService { get; } = new ProductService(_unitOfWork, _mapper);

        public IBasketService BasketService { get; } = new BasketService(_basketRepository,_mapper);
        public ICacheService CacheService { get; } = new CacheService(_cacheRepository);
        public IAuthService AuthService { get; } = new AuthService(_appUser, _options,_mapper);
        public IOrderService OrderService { get; } = new OrderService(_unitOfWork,_mapper,_basketRepository);
        public IPaymentService PaymentService { get; } = new PaymentService(_basketRepository,_unitOfWork,_configuration,_mapper);

    }
}

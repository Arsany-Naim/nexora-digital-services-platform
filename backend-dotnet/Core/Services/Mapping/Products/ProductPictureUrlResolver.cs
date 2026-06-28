using AutoMapper;
using Domain.Entities.Product;
using Microsoft.Extensions.Configuration;
using Shared.DTOS.ProductDTOS;

namespace Services.Mapping.Products
{
    public class ProductPictureUrlResolver(IConfiguration _configuration) : IValueResolver<Product, ProductResponse, string>
    {
        public string Resolve(Product source, ProductResponse destination, string destMember, ResolutionContext context)
        {
            if(!string.IsNullOrEmpty(source.PictureUrl))
            {
                return $"{_configuration["BaseUrl"]}/{source.PictureUrl}";
            }
            return string.Empty;
        }
    }
}

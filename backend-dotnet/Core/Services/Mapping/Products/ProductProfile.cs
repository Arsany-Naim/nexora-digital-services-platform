using AutoMapper;
using Domain.Entities.Product;
using Microsoft.Extensions.Configuration;
using Shared.DTOS.ProductDTOS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Mapping.Products
{
    public class ProductProfile : Profile
    {
        public ProductProfile(IConfiguration _configuration)
        {
            CreateMap<Product,ProductResponse>()
                .ForMember(D => D.Brand , O=> O.MapFrom(S=> S.Brand.Name))
                .ForMember(T => T.Type , O=> O.MapFrom(S=> S.Type.Name))
                //.ForMember(P => P.PictureUrl , O => O.MapFrom(S=> $"{_configuration["BaseUrl"]}/{S.PictureUrl}"));
                .ForMember(D => D.PictureUrl , O => O.MapFrom(new ProductPictureUrlResolver(_configuration)));
            CreateMap<ProductBrand , BrandTypeResponse>();
            CreateMap<ProductType  , BrandTypeResponse>();
        }
    }
}

using Domain.Entities.Product;
using Shared.DTOS.ProductDTOS;

namespace Services.Specifications
{
    public class ProductCountSpecifications : BaseSpecifications<int,Product>
    {
        public ProductCountSpecifications(ProductQueryParameters parameters) : 
            base
            (
                P=>
                    (!parameters.BrandId.HasValue || P.BrandId == parameters.BrandId)
                    &&
                    (!parameters.TypeId.HasValue || P.TypeId == parameters.TypeId)
                    &&
                    (string.IsNullOrEmpty(parameters.Search) || P.Name.ToLower().Contains(parameters.Search.ToLower()))
            )
        {
            
        }
    }
}

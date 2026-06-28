using Domain.Entities.Product;
using Shared.DTOS.ProductDTOS;
using System.Linq.Expressions;

namespace Services.Specifications.product
{
    public class ProductWithBrandandTypeSpecifications : BaseSpecifications<int , Product>
    {
        public ProductWithBrandandTypeSpecifications(ProductQueryParameters parameters) 
            : base(
                    P=> 
                    (!parameters.BrandId.HasValue || P.BrandId == parameters.BrandId)
                    && 
                    (!parameters.TypeId.HasValue ||P.TypeId == parameters.TypeId)
                    &&
                    (string.IsNullOrEmpty(parameters.Search) || P.Name.ToLower().Contains(parameters.Search.ToLower()))
                  )
        {
            ApplyPagination(parameters.PageIndex,parameters.PageSize);
            ApplySorting(parameters.Sort);
            ApplyIncludes();
        }
        public ProductWithBrandandTypeSpecifications(int id) : base(P=> P.Id == id)
        {
            ApplyIncludes();
        }
        private void ApplyIncludes()
        {
            Includes.Add(p => p.Brand);
            Includes.Add(p => p.Type);
        }
        private void ApplySorting(string? sort)
        {
            if (!string.IsNullOrEmpty(sort))
            {
                //Check value
                switch (sort.ToLower())
                {
                    case "priceasc":
                        AddOrderBy(P => P.Price);
                        break;
                    case "pricedesc":
                        AddOrderByDescending(P => P.Price);
                        break;
                    default:
                        AddOrderBy(OrderBy = P => P.Name);
                        break;
                }
            }
            else
            {
                AddOrderBy(OrderBy = P => P.Name);
            }
        }
    }
}

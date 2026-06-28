using AutoMapper;
using Domain.Contract;
using Domain.Entities.Product;
using Domain.Exceptions.NotFound;
using Services.Abstractions.Products;
using Services.Specifications;
using Services.Specifications.product;
using Shared;
using Shared.DTOS.ProductDTOS;

namespace Services.ProductServices
{
    public class ProductService(IUnitOfWork _unitOfWork , IMapper _mapper) : IProductService
    {
        public async Task<PaginationResponse<ProductResponse>> GetAllProductAsync(ProductQueryParameters parameters)
        {
            var spec = new ProductWithBrandandTypeSpecifications(parameters);
            var products = await _unitOfWork.GetRepository<int, Product>().GetAllAsync(spec);
            var result = _mapper.Map<IEnumerable<ProductResponse>>(products);
            var countSpec = new ProductCountSpecifications(parameters);
            var count = await _unitOfWork.GetRepository<int, Product>().GetCountAsync(countSpec);
            return new PaginationResponse<ProductResponse>(parameters.PageIndex,parameters.PageSize,count,result);
        }
        public async Task<ProductResponse> GetProductByIdAsync(int id)
        {
            var spec = new ProductWithBrandandTypeSpecifications(id);
            var product = await _unitOfWork.GetRepository<int, Product>().GetAsync(spec,id);
            if (product is null) throw new ProductNotFoundException(id);
            var result =  _mapper.Map<ProductResponse>(product);
            return result;
        }
        public async Task<IEnumerable<BrandTypeResponse>> GetAllBrandsAsync()
        {
            var brands = await _unitOfWork.GetRepository<int, ProductBrand>().GetAllAsync();
            var result = _mapper.Map<IEnumerable<BrandTypeResponse>>(brands);
            return result;
        }

        public async Task<IEnumerable<BrandTypeResponse>> GetAllTypesAsync()
        {
            var types = await _unitOfWork.GetRepository<int, ProductType>().GetAllAsync();
            var result = _mapper.Map<IEnumerable<BrandTypeResponse>>(types);
            return result;
        }

    }
}

using AutoMapper;
using BussinessLayer.BaseService;
using DataAccessLayer.BaseRepository;
using DataAccessLayer.Models;
using DataAccessLayer.Repository.product;
using Microsoft.EntityFrameworkCore;
using WarehouseDTOs;

namespace BussinessLayer.Service.product
{
    public class ProductService : BaseService<Product>, IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        

        public ProductService(
            IProductRepository productRepository,
            IMapper mapper) : base(productRepository)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<ProductDTO> CreateProductAsync(ProductDTO productDto)
        {
            var product = _mapper.Map<Product>(productDto);
            product.CreatedAt = DateTime.Now;
            product.CreatedBy = 1; 
            await AddAsync(product);
            return _mapper.Map<ProductDTO>(product);
        }

        public async Task<bool> DeleteMultipleProductsAsync(List<int> ids)
        {
            var products = await GetAllQueryable()
                .Where(p => ids.Contains(p.ProductId))
                .ToListAsync();

            if (products.Count != ids.Count)
                return false;

            foreach (var product in products)
            {
                await DeleteAsync(product.ProductId);
            }

            return true;
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            return await DeleteAsync(id);
        }

        public async Task<List<ProductDTO>> GetAllProductsAsync()
        {
            var products = await GetAllQueryable()
                .Include(p => p.Category)
                .ToListAsync();

            return _mapper.Map<List<ProductDTO>>(products);
        }

        public async Task<ProductDTO> GetProductByIdAsync(int id)
        {
            var product = await GetByIdAsync(id);
            return _mapper.Map<ProductDTO>(product);
        }

        public async Task<UpdateProductRequest> UpdateProductAsync(int id, UpdateProductRequest productDto)
        {
            // Lấy sản phẩm hiện tại
            var existingProduct = await GetByIdAsync(id);
            if (existingProduct == null)
            {
                return null;
            }

            // Ánh xạ dữ liệu từ DTO sang entity
            _mapper.Map(productDto, existingProduct);

            // Gán UpdatedAt và UpdatedBy
            existingProduct.UpdatedAt = DateTime.Now;
            existingProduct.UpdatedBy = 1;

            // Cập nhật sản phẩm
            await UpdateAsync(existingProduct);

            // Chuyển đổi entity thành DTO để trả về
            return _mapper.Map<UpdateProductRequest>(existingProduct);
        }
    }
}
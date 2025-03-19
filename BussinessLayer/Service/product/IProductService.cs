using BussinessLayer.BaseService;
using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseDTOs;

namespace BussinessLayer.Service.product
{
    public interface IProductService : IBaseService<Product>
    {
        Task<List<ProductDTO>> GetAllProductsAsync();
        Task<ProductDTO> GetProductByIdAsync(int id);
        Task<ProductDTO> CreateProductAsync(ProductDTO productDto);
        Task<UpdateProductRequest> UpdateProductAsync(int id, UpdateProductRequest productDto);
        Task<bool> DeleteProductAsync(int id);
        Task<bool> DeleteMultipleProductsAsync(List<int> ids);
        Task<ProductDTO?> GetProductByNameAsync(string name);

    }
}

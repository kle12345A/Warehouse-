using BussinessLayer.BaseService;
using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseDTOs;

namespace BussinessLayer.Service.shipping
{
    public interface IShippingService : IBaseService<Shipping>
    {
        Task<List<ShippingDTO>> GetAllShippingsAsync();
        Task<ShippingDTO> GetShippingByIdAsync(int id);
        Task<ShippingDTO> CreateShippingAsync(ShippingDTO shippingDto);
        Task<ShippingDTO> UpdateShippingAsync(int id, ShippingDTO shippingDto);
        Task<bool> DeleteShippingAsync(int id);
    }
}

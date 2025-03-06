using BussinessLayer.BaseService;
using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseDTOs;

namespace BussinessLayer.Service.supplier
{
    public interface ISupplierService : IBaseService<Supplier>
    {
        Task<List<SupplierDTO>> GetAllSuppliersAsync();
        Task<SupplierDTO> GetSupplierByIdAsync(int id);
        Task<SupplierDTO> CreateSupplierAsync(SupplierDTO supplierDto);
        Task<SupplierUpdateDTO> UpdateSupplierAsync(int id, SupplierUpdateDTO supplierDto);
        Task<bool> DeleteSupplierAsync(int id);
    }
}

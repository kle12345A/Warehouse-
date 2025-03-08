using DataAccessLayer.BaseRepository;
using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseDTOs;

namespace DataAccessLayer.Repository.orderdetail
{
    public interface IOrderDetailRepository : IBaseRepository<OrderDetail>
    {
        Task<OrderDetailWithSupplierDTO> GetOrderDetailsWithSupplierByOrderIdAsync(int orderId);
                Task<List<OrderDetail>> GetByOrderIdAsync(int orderId);
        Task<OrderDetailWithCustomerDTO> GetOrderDetailsWithCutsomerByOrderIdAsync(int orderId);


    }
}

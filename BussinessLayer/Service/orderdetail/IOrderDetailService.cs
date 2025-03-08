using BussinessLayer.BaseService;
using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseDTOs;

namespace BussinessLayer.Service.orderdetail
{
    public interface IOrderDetailService : IBaseService<OrderDetail>
    {
        Task<OrderDetailWithSupplierDTO> GetAllAsync(int id);
        Task<OrderDetailWithCustomerDTO> GetAllWithCustomerAsync(int id);

    }
}

using BussinessLayer.BaseService;
using DataAccessLayer.BaseRepository;
using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseDTOs;

namespace BussinessLayer.Service.customer
{
    public interface ICustomerService : IBaseService<Customer>
    {
        
            Task<CustomerDTO> CreateCustomerAsync(CustomerDTO customerDto);
            Task<List<CustomerDTO>> GetAllCustomersAsync();
            Task<CustomerDTO> GetCustomerByIdAsync(int id);
            Task<CustomerDTO> UpdateCustomerAsync(int id, CustomerUpdateDTO customerDto);
        Task<int> GetTotalCustomersAsync();
        Task<bool> DeleteMultipleCustomersAsync(List<int> ids);
    }
}

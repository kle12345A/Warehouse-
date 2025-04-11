using DataAccessLayer.BaseRepository;
using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repository.customer
{
    public interface ICustomerRepository : IBaseRepository<Customer>
    {
        Task<List<Customer>> GetCustomersByIdsAsync(List<int> ids);
        Task DeleteMultipleAsync(List<Customer> customers);
        Task<int> GetTotalCustomersAsync();

    }
}

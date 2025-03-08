using DataAccessLayer.BaseRepository;
using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Repository.customer
{
    public class CustomerRepository : BaseRepository<Customer>, ICustomerRepository
    {
        private readonly WarehouseDbContext _dbContext;

        public CustomerRepository(WarehouseDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Customer>> GetCustomersByIdsAsync(List<int> ids)
        {
            return await _dbContext.Customers.Where(c => ids.Contains(c.CustomerId)).ToListAsync();
        }

        public async Task DeleteMultipleAsync(List<Customer> customers)
        {
            _dbContext.Customers.RemoveRange(customers);
            await _dbContext.SaveChangesAsync();
        }
    }

}

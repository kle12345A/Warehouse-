using AutoMapper;
using BussinessLayer.BaseService;
using DataAccessLayer.Models;
using DataAccessLayer.Repository.customer;
using Microsoft.EntityFrameworkCore;
using WarehouseDTOs;

namespace BussinessLayer.Service.customer
{
    public class CustomerService : BaseService<Customer>, ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;

        public CustomerService(ICustomerRepository baseRepository, IMapper mapper) : base(baseRepository)
        {
            _customerRepository = baseRepository;
            _mapper = mapper;
        }

        public async Task<CustomerDTO> CreateCustomerAsync(CustomerDTO customerDto)
        {
            if (customerDto == null)
            {
                throw new ArgumentNullException(nameof(customerDto));
            }

            var customer = _mapper.Map<Customer>(customerDto);
            customer.CreatedAt = DateTime.Now;

            await AddAsync(customer); 
            return _mapper.Map<CustomerDTO>(customer);
        }

        public async Task<bool> DeleteMultipleCustomersAsync(List<int> ids)
        {
            var customersToDelete = await _customerRepository.GetCustomersByIdsAsync(ids);

            if (!customersToDelete.Any())
                return false;

            await _customerRepository.DeleteMultipleAsync(customersToDelete);
            return true;
        }


        public async Task<List<CustomerDTO>> GetAllCustomersAsync()
        {
            var customers = await _customerRepository.GetAllAsync();
            return _mapper.Map<List<CustomerDTO>>(customers);
        }

        public async Task<CustomerDTO> GetCustomerByIdAsync(int id)
        {
            var customer = await _customerRepository.GetByIdAsync(id);
            if (customer == null)
            {
                return null;
            }
            return _mapper.Map<CustomerDTO>(customer);
        }

        public async Task<CustomerDTO> UpdateCustomerAsync(int id, CustomerUpdateDTO customerDto)
        {
            if (id <= 0)
            {
                throw new ArgumentException("Customer ID must be greater than 0.", nameof(id));
            }

            if (customerDto == null)
            {
                throw new ArgumentNullException(nameof(customerDto));
            }

            var existingCustomer = await GetByIdAsync(id);
            if (existingCustomer == null)
            {
                throw new KeyNotFoundException($"Customer with ID {id} not found.");
            }

            _mapper.Map(customerDto, existingCustomer);
            existingCustomer.UpdatedAt = DateTime.Now;

            await UpdateAsync(existingCustomer);
            return _mapper.Map<CustomerDTO>(existingCustomer);
        }
    }
}

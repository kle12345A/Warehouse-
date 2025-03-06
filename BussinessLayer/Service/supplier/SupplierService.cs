using AutoMapper;
using BussinessLayer.BaseService;
using DataAccessLayer.Models;
using DataAccessLayer.Repository.supplier;
using WarehouseDTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BussinessLayer.Service.supplier
{
    public class SupplierService : BaseService<Supplier>, ISupplierService
    {
        private readonly ISupplierRepository _supplierRepository;
        private readonly IMapper _mapper;

        public SupplierService(ISupplierRepository baseRepository, IMapper mapper) : base(baseRepository)
        {
            _supplierRepository = baseRepository;
            _mapper = mapper;
        }

        public async Task<List<SupplierDTO>> GetAllSuppliersAsync()
        {
            var suppliers = await GetAllAsync();
            if (suppliers == null)
            {
                throw new InvalidOperationException("Failed to retrieve suppliers.");
            }
            return _mapper.Map<List<SupplierDTO>>(suppliers);
        }

        public async Task<SupplierDTO> GetSupplierByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("Supplier ID must be greater than 0.", nameof(id));
            }

            var supplier = await GetByIdAsync(id);
            if (supplier == null)
            {
                throw new KeyNotFoundException($"Supplier with ID {id} not found.");
            }
            return _mapper.Map<SupplierDTO>(supplier);
        }

        public async Task<SupplierDTO> CreateSupplierAsync(SupplierDTO supplierDto)
        {
            var supplier = _mapper.Map<Supplier>(supplierDto);
            await AddAsync(supplier);
            return _mapper.Map<SupplierDTO>(supplier);
        }

        public async Task<SupplierUpdateDTO> UpdateSupplierAsync(int id, SupplierUpdateDTO supplierDto)
        {
            if (id <= 0)
            {
                throw new ArgumentException("Supplier ID must be greater than 0.", nameof(id));
            }

            

            var existingSupplier = await GetByIdAsync(id);
            if (existingSupplier == null)
            {
                throw new KeyNotFoundException($"Supplier with ID {id} not found.");
            }

            _mapper.Map(supplierDto, existingSupplier);
            await UpdateAsync(existingSupplier);
            return _mapper.Map<SupplierUpdateDTO>(existingSupplier);
        }

        public async Task<bool> DeleteSupplierAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("Supplier ID must be greater than 0.", nameof(id));
            }

            return await DeleteAsync(id);
        }
    }
}
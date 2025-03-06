using AutoMapper;
using BussinessLayer.BaseService;
using DataAccessLayer.Models;
using DataAccessLayer.Repository.shipping;
using WarehouseDTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BussinessLayer.Service.shipping
{
    public class ShippingService : BaseService<Shipping>, IShippingService
    {
        private readonly IShippingRepository _shippingRepository;
        private readonly IMapper _mapper;

        public ShippingService(IShippingRepository baseRepository, IMapper mapper) : base(baseRepository)
        {
            _shippingRepository = baseRepository;
            _mapper = mapper;
        }

        public async Task<List<ShippingDTO>> GetAllShippingsAsync()
        {
            var shippings = await GetAllAsync();
            if (shippings == null)
            {
                throw new InvalidOperationException("Failed to retrieve shippings.");
            }
            return _mapper.Map<List<ShippingDTO>>(shippings);
        }

        public async Task<ShippingDTO> GetShippingByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("Shipping ID must be greater than 0.", nameof(id));
            }

            var shipping = await GetByIdAsync(id);
            if (shipping == null)
            {
                throw new KeyNotFoundException($"Shipping with ID {id} not found.");
            }
            return _mapper.Map<ShippingDTO>(shipping);
        }

        public async Task<ShippingDTO> CreateShippingAsync(ShippingDTO shippingDto)
        {
            var shipping = _mapper.Map<Shipping>(shippingDto);
            await AddAsync(shipping);
            return _mapper.Map<ShippingDTO>(shipping);
        }

        public async Task<ShippingDTO> UpdateShippingAsync(int id, ShippingDTO shippingDto)
        {
            if (id <= 0)
            {
                throw new ArgumentException("Shipping ID must be greater than 0.", nameof(id));
            }

            var existingShipping = await GetByIdAsync(id);
            if (existingShipping == null)
            {
                throw new KeyNotFoundException($"Shipping with ID {id} not found.");
            }

            _mapper.Map(shippingDto, existingShipping);
            await UpdateAsync(existingShipping);
            return _mapper.Map<ShippingDTO>(existingShipping);
        }

        public async Task<bool> DeleteShippingAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("Shipping ID must be greater than 0.", nameof(id));
            }

            return await DeleteAsync(id);
        }
    }
}
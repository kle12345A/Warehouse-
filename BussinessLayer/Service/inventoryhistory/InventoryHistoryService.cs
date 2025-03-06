using AutoMapper;
using BussinessLayer.BaseService;
using DataAccessLayer.Models;
using WarehouseDTOs;
using System.Collections.Generic;
using System.Threading.Tasks;
using BussinessLayer.Service.inventoryhistory;
using DataAccessLayer.Repository.inventoryhistory;

namespace BussinessLayer.Service.inventoryHistory
{
    public class InventoryHistoryService : BaseService<InventoryHistory>, IInventoryHistoryService
    {
        private readonly IInventoryHistoryRepository _inventoryHistoryRepository;
        private readonly IMapper _mapper;

        public InventoryHistoryService(IInventoryHistoryRepository baseRepository, IMapper mapper) : base(baseRepository)
        {
            _inventoryHistoryRepository = baseRepository;
            _mapper = mapper;
        }

        public async Task<List<InventoryHistoryDTO>> GetAllInventoryHistoriesAsync()
        {
            var inventoryHistories = await GetAllAsync();
            if (inventoryHistories == null)
            {
                throw new InvalidOperationException("Failed to retrieve inventory histories.");
            }
            return _mapper.Map<List<InventoryHistoryDTO>>(inventoryHistories);
        }

        public async Task<InventoryHistoryDTO> GetInventoryHistoryByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("InventoryHistory ID must be greater than 0.", nameof(id));
            }

            var inventoryHistory = await GetByIdAsync(id);
            if (inventoryHistory == null)
            {
                throw new KeyNotFoundException($"InventoryHistory with ID {id} not found.");
            }
            return _mapper.Map<InventoryHistoryDTO>(inventoryHistory);
        }

        public async Task<InventoryHistoryDTO> CreateInventoryHistoryAsync(InventoryHistoryDTO inventoryHistoryDto)
        {
            var inventoryHistory = _mapper.Map<InventoryHistory>(inventoryHistoryDto);
            await AddAsync(inventoryHistory);
            return _mapper.Map<InventoryHistoryDTO>(inventoryHistory);
        }

        public async Task<InventoryHistoryDTO> UpdateInventoryHistoryAsync(int id, InventoryHistoryDTO inventoryHistoryDto)
        {
            if (id <= 0)
            {
                throw new ArgumentException("InventoryHistory ID must be greater than 0.", nameof(id));
            }

            var existingInventoryHistory = await GetByIdAsync(id);
            if (existingInventoryHistory == null)
            {
                throw new KeyNotFoundException($"InventoryHistory with ID {id} not found.");
            }

            _mapper.Map(inventoryHistoryDto, existingInventoryHistory);
            await UpdateAsync(existingInventoryHistory);
            return _mapper.Map<InventoryHistoryDTO>(existingInventoryHistory);
        }

        public async Task<bool> DeleteInventoryHistoryAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("InventoryHistory ID must be greater than 0.", nameof(id));
            }

            return await DeleteAsync(id);
        }
    }
}
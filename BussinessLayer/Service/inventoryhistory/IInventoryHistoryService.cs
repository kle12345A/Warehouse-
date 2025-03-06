using BussinessLayer.BaseService;
using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseDTOs;

namespace BussinessLayer.Service.inventoryhistory
{
    public interface IInventoryHistoryService : IBaseService<InventoryHistory>
    {
        Task<List<InventoryHistoryDTO>> GetAllInventoryHistoriesAsync();
        Task<InventoryHistoryDTO> GetInventoryHistoryByIdAsync(int id);
        Task<InventoryHistoryDTO> CreateInventoryHistoryAsync(InventoryHistoryDTO inventoryHistoryDto);
        Task<InventoryHistoryDTO> UpdateInventoryHistoryAsync(int id, InventoryHistoryDTO inventoryHistoryDto);
        Task<bool> DeleteInventoryHistoryAsync(int id);
    }
}

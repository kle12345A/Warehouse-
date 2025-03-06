using BussinessLayer.Service.inventoryHistory;
using Microsoft.AspNetCore.Mvc;
using WarehouseDTOs;
using BussinessLayer.Service.inventoryhistory;

namespace Warehouse.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryHistoryController : ControllerBase
    {
        private readonly IInventoryHistoryService _inventoryHistoryService;

        public InventoryHistoryController(IInventoryHistoryService inventoryHistoryService)
        {
            _inventoryHistoryService = inventoryHistoryService;
        }

        [HttpGet]
        public async Task<ActionResult<List<InventoryHistoryDTO>>> GetAllInventoryHistories()
        {
            var inventoryHistories = await _inventoryHistoryService.GetAllInventoryHistoriesAsync();
            return Ok(inventoryHistories);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<InventoryHistoryDTO>> GetInventoryHistoryById(int id)
        {
            var inventoryHistory = await _inventoryHistoryService.GetInventoryHistoryByIdAsync(id);
            return Ok(inventoryHistory);
        }

        [HttpPost]
        public async Task<ActionResult<InventoryHistoryDTO>> CreateInventoryHistory([FromBody] InventoryHistoryDTO inventoryHistoryDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var createdInventoryHistory = await _inventoryHistoryService.CreateInventoryHistoryAsync(inventoryHistoryDto);
            return CreatedAtAction(nameof(GetInventoryHistoryById), new { id = createdInventoryHistory.HistoryId }, createdInventoryHistory);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<InventoryHistoryDTO>> UpdateInventoryHistory(int id, [FromBody] InventoryHistoryDTO inventoryHistoryDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var updatedInventoryHistory = await _inventoryHistoryService.UpdateInventoryHistoryAsync(id, inventoryHistoryDto);
            return Ok(updatedInventoryHistory);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteInventoryHistory(int id)
        {
            var result = await _inventoryHistoryService.DeleteInventoryHistoryAsync(id);
            if (!result)
            {
                return NotFound(new { message = "Lịch sử tồn kho không tồn tại" });
            }
            return NoContent();
        }
    }
}
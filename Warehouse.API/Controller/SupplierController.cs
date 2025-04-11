using BussinessLayer.Service.supplier;
using Microsoft.AspNetCore.Mvc;
using WarehouseDTOs;

namespace Warehouse.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SupplierController : ControllerBase
    {
        private readonly ISupplierService _supplierService;

        public SupplierController(ISupplierService supplierService)
        {
            _supplierService = supplierService;
        }

        [HttpGet]
        public async Task<ActionResult<List<SupplierDTO>>> GetAllSuppliers()
        {
            var suppliers = await _supplierService.GetAllSuppliersAsync();
            return Ok(suppliers);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SupplierDTO>> GetSupplierById(int id)
        {
            var supplier = await _supplierService.GetSupplierByIdAsync(id);
            return Ok(supplier);
        }

        [HttpPost]
        public async Task<ActionResult<SupplierDTO>> CreateSupplier([FromBody] SupplierDTO supplierDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);

            }
            var createdSupplier = await _supplierService.CreateSupplierAsync(supplierDto);
            return CreatedAtAction(nameof(GetSupplierById), new { id = createdSupplier.SupplierId }, createdSupplier);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<SupplierDTO>> UpdateSupplier(int id, [FromBody] SupplierUpdateDTO supplierDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var updatedSupplier = await _supplierService.UpdateSupplierAsync(id, supplierDto);
            return Ok(updatedSupplier);
        }
        [HttpGet("TotalSuppliers")]
        public async Task<ActionResult<int>> GetTotalSuppliers()
        {
            var totalSuppliers = await _supplierService.GetTotalSuppliersAsync();
            return Ok(totalSuppliers);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteSupplier(int id)
        {
            var result = await _supplierService.DeleteSupplierAsync(id);
            if (!result)
            {
                return NotFound(new { message = "Nhà cung cấp không tồn tại" });
            }
            return NoContent();
        }
    }
}
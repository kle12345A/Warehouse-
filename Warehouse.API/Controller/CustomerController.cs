using BussinessLayer.Service.customer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WarehouseDTOs;

namespace Warehouse.API.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }
        [HttpGet]
        public async Task<ActionResult<List<CustomerDTO>>> GetAllCustomers()
        {
            try
            {
                var customers = await _customerService.GetAllCustomersAsync();
                return Ok(customers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi lấy danh sách khách hàng.", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<CustomerUpdateDTO>> UpdateCustomer(int id, [FromBody] CustomerUpdateDTO customerDto)
        {
            try
            {
                if (customerDto == null)
                {
                    return BadRequest(new { message = "Dữ liệu khách hàng không hợp lệ." });
                }

                if (id != customerDto.CustomerId)
                {
                    return BadRequest(new { message = "ID trong URL không khớp với ID trong dữ liệu." });
                }

                var updatedCustomer = await _customerService.UpdateCustomerAsync(id, customerDto);
                if (updatedCustomer == null)
                {
                    return NotFound(new { message = $"Khách hàng với ID {id} không tồn tại." });
                }
                return Ok(updatedCustomer);
          
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi cập nhật khách hàng.", error = ex.Message });
            }
        }
        [HttpGet("TotalCustomers")]
        public async Task<ActionResult<int>> GetTotalCustomers()
        {
            var totalCustomers = await _customerService.GetTotalCustomersAsync();
            return Ok(totalCustomers);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerDTO>> GetCustomerById(int id)
        {
            try
            {
                var customer = await _customerService.GetCustomerByIdAsync(id);
                if (customer == null)
                {
                    return NotFound(new { message = $"Khách hàng với ID {id} không tồn tại." });
                }
                return Ok(customer);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi lấy thông tin khách hàng.", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<CustomerDTO>> AddCustomer([FromBody] CustomerDTO customerDto)
        {
            try
            {
                if (customerDto == null)
                {
                    return BadRequest(new { message = "Dữ liệu khách hàng không hợp lệ." });
                }

                var createdCustomer = await _customerService.CreateCustomerAsync(customerDto);
                if (createdCustomer == null)
                {
                    return StatusCode(500, new { message = "Không thể thêm khách hàng." });
                }

                return CreatedAtAction(nameof(GetCustomerById), new { id = createdCustomer.CustomerId }, createdCustomer);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi thêm khách hàng.", error = ex.Message });
            }
        }

        [HttpPost("delete-multiple")]
        public async Task<IActionResult> DeleteMultipleCustomers([FromBody] List<int> ids)
        {
            try
            {
                if (ids == null || !ids.Any())
                {
                    return BadRequest(new { message = "Danh sách ID không hợp lệ." });
                }

                var result = await _customerService.DeleteMultipleCustomersAsync(ids);
                if (!result)
                {
                    return NotFound(new { message = "Không tìm thấy khách hàng để xóa." });
                }

                return Ok(new { success = true, message = "Xóa khách hàng thành công." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Đã xảy ra lỗi khi xóa khách hàng.", error = ex.Message });
            }
        }


    }
}

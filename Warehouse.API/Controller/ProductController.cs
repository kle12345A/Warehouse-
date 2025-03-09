using BussinessLayer.Service.import;
using BussinessLayer.Service.product;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WarehouseDTOs;

namespace Warehouse.API.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly Cloudinary _cloudinary;
        private readonly IProductImportService _productImportService;
        public ProductsController(IProductService productService, Cloudinary cloudinary,IProductImportService productImportService)
        {
            _productService = productService;
            _cloudinary = cloudinary;
            _productImportService = productImportService;
        }

        // POST api/monthlyreport/read-file-month-report
        [HttpPost("read-file-product")]
        public async Task<IActionResult> ImportExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            try
            {
                using (var stream = file.OpenReadStream())
                {
                    var records = await _productImportService.ReadExcelFileAsync(stream);
                    return Ok(records);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }
        [HttpGet]
        public async Task<ActionResult<List<ProductDTO>>> GetAllProducts()
        {
            var products = await _productService.GetAllProductsAsync();
            return Ok(products);
        }

       
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDTO>> GetProductById(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null) return NotFound(new { message = "Sản phẩm không tồn tại" });
            return Ok(product);
        }
   


        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<ProductDTO>> CreateProduct([FromForm] CreateProductRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var productDto = new ProductDTO
            {
                Name = request.Name,
                Description = request.Description,
                Unit = request.Unit,
                AvailableQuantity = request.AvailableQuantity,
                Price = request.Price,
                CostPrice = request.CostPrice,
                CategoryId = request.CategoryId
                
            };

            if (request.ImageFile != null && request.ImageFile.Length > 0)
            {
                try
                {
                    using var stream = request.ImageFile.OpenReadStream();
                    var fileNameWithoutExt = Path.GetFileNameWithoutExtension(request.ImageFile.FileName);
                    var uploadParams = new ImageUploadParams
                    {
                        File = new FileDescription(request.ImageFile.FileName, stream),
                        PublicId = $"{fileNameWithoutExt}" 
                    };

                    var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                    productDto.Images = uploadResult.SecureUrl.ToString();
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new { message = "Error uploading image", error = ex.Message });
                }
            }

            var createdProduct = await _productService.CreateProductAsync(productDto);
            return CreatedAtAction(nameof(GetProductById), new { id = createdProduct.ProductId }, createdProduct);
        }


        [HttpPost("create-product")]
        
        public async Task<ActionResult<ProductDTO>> CreateProduct2([FromBody] CreateProductRequest2 request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var productDto = new ProductDTO
            {
                Name = request.Name,
                Description = request.Description,
                Unit = request.Unit,
                AvailableQuantity = request.AvailableQuantity,
                Price = request.Price,
                CostPrice = request.CostPrice,
                CategoryId = request.CategoryId,
                Images = request.Images,

            };

          
            var createdProduct = await _productService.CreateProductAsync(productDto);
            return CreatedAtAction(nameof(GetProductById), new { id = createdProduct.ProductId }, createdProduct);
        }



        [HttpPut("{id}")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<UpdateProductRequest>> UpdateProduct(int id, [FromForm] UpdateProductRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (id != request.ProductId)
            {
                return BadRequest(new { message = "ID trong URL không khớp với ID trong dữ liệu gửi lên" });
            }
            var existingProduct = await _productService.GetProductByIdAsync(id);
            if (existingProduct == null)
            {
                return NotFound(new { message = "Sản phẩm không tồn tại" });
            }

            if (request.ImageFile != null && request.ImageFile.Length > 0)
            {
                try
                {
                    using var stream = request.ImageFile.OpenReadStream();
                    var fileNameWithoutExt = Path.GetFileNameWithoutExtension(request.ImageFile.FileName);
                    var uploadParams = new ImageUploadParams
                    {
                        File = new FileDescription(request.ImageFile.FileName, stream),
                        PublicId = $"{fileNameWithoutExt}"
                    };

                    var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                    request.Images = uploadResult.SecureUrl.ToString();
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new { message = "Error uploading image", error = ex.Message });
                }
            }
            else
            {
                if (string.IsNullOrEmpty(request.Images))
                {
                    request.Images = existingProduct.Images;
                }
            }

            var updatedProduct = await _productService.UpdateProductAsync(id, request);
            if (updatedProduct == null)
            {
                return NotFound(new { message = "Sản phẩm không tồn tại" });
            }

            return Ok(updatedProduct);
        }



        [HttpPost("DeleteMultiple")]
        public async Task<ActionResult> DeleteMultiple([FromBody] List<int> ids)
        {
            if (ids == null || !ids.Any())
                return BadRequest(new { message = "Danh sách ID không hợp lệ." });

            try
            {
                var result = await _productService.DeleteMultipleProductsAsync(ids);
                if (!result)
                    return BadRequest(new { message = "Không thể xóa một số sản phẩm." });

                return Ok(new { message = "Xóa thành công." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Lỗi server: {ex.Message}" });
            }
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            var result = await _productService.DeleteProductAsync(id);
            if (!result) return NotFound(new { message = "Sản phẩm không tồn tại" });

            return NoContent();
        }
    }
}

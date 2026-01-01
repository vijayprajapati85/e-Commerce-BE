using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using ProductSale.Lib.App.Models;
using ProductSale.Lib.App.Services;
using ProductSale.Lib.Infra.WebApi;

namespace ProductSale.Controllers
{
    [EnableCors("CorsPolicy")]
    [Route("v1/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ILogger<ProductController> _logger;
        private readonly IProductService _service;
        private readonly IWebHostEnvironment _env;
        public ProductController(IProductService service, ILogger<ProductController> logger, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _service = service;
            _env = webHostEnvironment;
        }

        [HttpPost("InsertUpdate")]
        public async Task<IActionResult> UpsertProduct([FromHeader(Name = "userid")] string userid, [FromForm] ProductInfoRequest productInfo)
        {
            _service.UserId = userid;
            var result = await _service.UpsertProductAsync(productInfo, string.Format("{0}/Product", _env.WebRootPath.ToString()));
            if (result != 0)
            {
                return Ok(JsonResultVm<int>.SuccessResponse("Product saved successfully.", result));
            }
            return BadRequest(JsonResultVm<int>.FailResponse("Error", "Something went wrong."));
        }

        [HttpGet("Get")]
        public async Task<IActionResult> GetProduct([FromQuery] int id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null || result.Id == 0)
            {
                return BadRequest(JsonResultVm<int>.FailResponse("No Records", "Record not found"));
            }
            return Ok(JsonResultVm<ProductInfoDto>.SuccessResponse("Record found", result));
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetProducts()
        {
            var baseUrl = $"{Request.Scheme}://{Request.Host}{Request.PathBase}/images/Product/";
            var result = await _service.GetAllProductsAsync(baseUrl);
            if (result == null || result.Count == 0)
            {
                return Ok(JsonResultVm<ProductInfoDto>.FailResponse("No Records", "Record not found", null));
            }
            return Ok(JsonResultVm<List<ProductInfoDto>>.SuccessResponse("Record found", result, result.Count));
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> DeleteProduct([FromQuery] int id)
        {
            var result = await _service.DeleteProductAsync(id);
            if (result != 0)
            {
                return Ok(JsonResultVm<int>.SuccessResponse("Product deleted successfully.", id));
            }
            return BadRequest(JsonResultVm<int>.FailResponse("Error", "Something went wrong."));
        }

        [HttpPost("GetProducts")]
        public async Task<IActionResult> GetProductsByCatSubCat([FromBody] ProductFilterDto product)
        {
            var baseUrl = $"{Request.Scheme}://{Request.Host}{Request.PathBase}/images/Product/";
            var result = await _service.GetProductByCatSubCat(product, baseUrl);
            if (result == null || result.Count == 0)
            {
                return Ok(JsonResultVm<ProductInfoDto>.FailResponse("No Records", "Record not found", null));
            }

            return Ok(JsonResultVm<List<ProductInfoDto>>.SuccessResponse("Record found", result, result.Count));
        }
    }
}

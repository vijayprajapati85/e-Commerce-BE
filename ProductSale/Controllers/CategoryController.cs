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
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _service;
        public CategoryController(ICategoryService service, ILogger<CategoryController> logger)
        {
            _service = service;
        }

        [HttpPost("InsertUpdate")]
        public async Task<IActionResult> Upsert([FromHeader(Name = "userid")] string userid, [FromBody] CategoryRequest request)
        {
            _service.UserId = userid;
            var result = request != null ? await _service.UpsertCateogryAsync(request) : 0;
            if (result != 0)
            {
                return Ok(JsonResultVm<int>.SuccessResponse("Category saved successfully.", result));
            }
            return BadRequest(JsonResultVm<int>.FailResponse("Error", "Something went wrong."));
        }

        [HttpGet("Get")]
        public async Task<IActionResult> GetCategory([FromQuery] long id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null || result.Id == 0)
            {
                return BadRequest(JsonResultVm<int>.FailResponse("No Records", "Record not found"));
            }
            return Ok(JsonResultVm<CategoryDto>.SuccessResponse("Record found", result));
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllCategory()
        {
            var result = await _service.GetAllCategoryAsync();
            if (result == null || result.Count == 0)
            {
                return BadRequest(JsonResultVm<int>.FailResponse("No Records", "Record not found"));
            }
            return Ok(JsonResultVm<List<CategoryDto>>.SuccessResponse("Record found", result, result.Count));
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> DeleteCategory([FromQuery] long id, [FromHeader(Name = "userid")] string userid)
        {
            _service.UserId = userid;
            var result = await _service.DeleteCategoryAsync(id);
            if (result != 0)
            {
                return Ok(JsonResultVm<long>.SuccessResponse("Category deleted successfully.", id));
            }
            return BadRequest(JsonResultVm<int>.FailResponse("Error", "Something went wrong."));
        }

        [HttpGet("Test")]
        public async Task<IActionResult> TestResult()
        {
            return Ok(JsonResultVm<int>.SuccessResponse("Test successfully.", 1));
        }
    }
}

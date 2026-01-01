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
    public class SubCategoryController : ControllerBase
    {
        private readonly ILogger<SubCategoryController> _logger;
        private readonly ISubCategoryService _service;
        public SubCategoryController(ISubCategoryService service, ILogger<SubCategoryController> logger)
        {
            _logger = logger;
            _service = service;
        }

        [HttpPost("InsertUpdate")]
        public async Task<IActionResult> Upsert([FromHeader(Name = "userid")] string userid, [FromBody] SubCategoryRequest request)
        {
            _service.UserId = userid;
            var result = await _service.UpsertSubCateogryAsync(request);
            if (result != 0)
            {
                return Ok(JsonResultVm<int>.SuccessResponse("SubCategory saved successfully.", result));
            }
            return BadRequest(JsonResultVm<int>.FailResponse("Error", "Something went wrong."));
        }

        [HttpGet("Get")]
        public async Task<IActionResult> GetSubCategory([FromQuery] long id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null || result.Id == 0)
            {
                return BadRequest(JsonResultVm<int>.FailResponse("No Records", "Record not found"));
            }
            return Ok(JsonResultVm<SubCategoryDto>.SuccessResponse("Record found", result));
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllCategory()
        {
            var result = await _service.GetAllSubCategoryAsync();
            if (result == null || result.Count == 0)
            {
                return BadRequest(JsonResultVm<int>.FailResponse("No Records", "Record not found"));
            }
            return Ok(JsonResultVm<List<SubCategoryDto>>.SuccessResponse("Record found", result, result.Count));
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> DeleteSubCategory([FromQuery] long id, [FromHeader(Name = "userid")] string userid)
        {
            _service.UserId = userid;
            var result = await _service.DeleteSubCategoryAsync(id);
            if (result != 0)
            {
                return Ok(JsonResultVm<long>.SuccessResponse("SubCategory deleted successfully.", id));
            }
            return BadRequest(JsonResultVm<int>.FailResponse("Error", "Something went wrong."));
        }

        [HttpGet("GetByCatId")]
        public async Task<IActionResult> GetByCatId([FromQuery] long id)
        {
            var result = await _service.GetByCatIdAsync(id);
            if (result == null || result.Count == 0)
            {
                return BadRequest(JsonResultVm<int>.FailResponse("No Records", "Record not found"));
            }
            return Ok(JsonResultVm<List<SubCategoryDto>>.SuccessResponse("Record found", result));
        }
    }
}

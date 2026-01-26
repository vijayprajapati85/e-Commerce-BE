using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using ProductSale.Lib.App.Models.Email;
using ProductSale.Lib.App.Services;
using ProductSale.Lib.Infra.WebApi;

namespace ProductSale.Controllers
{
    [EnableCors("CorsPolicy")]
    [Route("v1/[controller]")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        private readonly IContactInfoService _service;
        public ContactController(IContactInfoService service, ILogger<ContactController> logger)
        {
            _service = service;
        }

        [HttpPost("Submit")]
        public async Task<IActionResult> Submit([FromBody] ContactInfo request)
        {
            var result = request != null ? await _service.Submit(request) : 0;
            if (result != 0)
            {
                return Ok(JsonResultVm<int>.SuccessResponse("Contact information submit successfully.", result));
            }
            return BadRequest(JsonResultVm<int>.FailResponse("Error", "Something went wrong."));
        }
    }
}

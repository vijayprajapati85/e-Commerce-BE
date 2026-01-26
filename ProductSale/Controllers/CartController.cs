using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using ProductSale.Lib.App.Models.Cart;
using ProductSale.Lib.App.Services;
using ProductSale.Lib.Infra.WebApi;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ProductSale.Controllers
{
    [EnableCors("CorsPolicy")]
    [Route("v1/[controller]")]
    [Authorize]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartInfoService _service;
        public CartController(ICartInfoService service, ILogger<CategoryController> logger)
        {
            _service = service;
        }

        [HttpPost("InsertUpdate")]
        public async Task<IActionResult> Upsert([FromBody] CartRequestDto request)
        {
            string authorizationHeader = Request.Headers[HeaderNames.Authorization].ToString();
            var handler = new JwtSecurityTokenHandler();

            if (!string.IsNullOrEmpty(authorizationHeader) && authorizationHeader.StartsWith("Bearer "))
            {
                string token = authorizationHeader.Substring("Bearer ".Length).Trim();
                var jsonToken = handler.ReadJwtToken(token);
                var userIdClaim = jsonToken.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Sid);
                _service.UserId = Convert.ToInt64(userIdClaim?.Value);
            }

            var result = request != null ? await _service.UpsertCartInfoAsync(request) : 0;
            if (result != 0)
            {
                return Ok(JsonResultVm<int>.SuccessResponse("Cart saved successfully.", result));
            }
            return BadRequest(JsonResultVm<int>.FailResponse("Error", "Something went wrong."));
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete([FromQuery] int productId)
        {
            string authorizationHeader = Request.Headers[HeaderNames.Authorization].ToString();
            var handler = new JwtSecurityTokenHandler();

            if (!string.IsNullOrEmpty(authorizationHeader) && authorizationHeader.StartsWith("Bearer "))
            {
                string token = authorizationHeader.Substring("Bearer ".Length).Trim();
                var jsonToken = handler.ReadJwtToken(token);
                var userIdClaim = jsonToken.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Sid);
                _service.UserId = Convert.ToInt64(userIdClaim?.Value);
            }
            var result = productId != 0 ? await _service.DeleteCartInfoAsync(productId) : 0;
            if (result != 0)
            {
                return Ok(JsonResultVm<int>.SuccessResponse("Cart saved successfully.", result));
            }
            return BadRequest(JsonResultVm<int>.FailResponse("Error", "Something went wrong."));
        }

        [HttpPost("Order")]
        public async Task<IActionResult> OrderEmail()
        {
            string authorizationHeader = Request.Headers[HeaderNames.Authorization].ToString();
            var handler = new JwtSecurityTokenHandler();

            if (!string.IsNullOrEmpty(authorizationHeader) && authorizationHeader.StartsWith("Bearer "))
            {
                string token = authorizationHeader.Substring("Bearer ".Length).Trim();
                var jsonToken = handler.ReadJwtToken(token);
                var userIdClaim = jsonToken.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Sid);
                _service.UserId = Convert.ToInt64(userIdClaim?.Value);
            }
            string message = await _service.CartOrderAsync();
            return Ok(JsonResultVm<int>.SuccessResponse(message, 1));
        }

        [HttpPost("PendingCart")]
        public async Task<IActionResult> GetPendingCart()
        {
            string authorizationHeader = Request.Headers[HeaderNames.Authorization].ToString();
            var handler = new JwtSecurityTokenHandler();
            if (!string.IsNullOrEmpty(authorizationHeader) && authorizationHeader.StartsWith("Bearer "))
            {
                string token = authorizationHeader.Substring("Bearer ".Length).Trim();
                var jsonToken = handler.ReadJwtToken(token);
                var userIdClaim = jsonToken.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Sid);
                _service.UserId = Convert.ToInt64(userIdClaim?.Value);
            }
            var result = await _service.GetPendingCartAsync();
            if (result != null)
            {
                return Ok(JsonResultVm<List<Order>>.SuccessResponse("Cart fetched successfully.", result));
            }
            return BadRequest(JsonResultVm<List<Order>>.FailResponse("Error", "Something went wrong."));
        }
    }
}

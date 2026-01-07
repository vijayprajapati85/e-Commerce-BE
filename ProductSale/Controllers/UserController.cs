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
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUserInfoService _service;
        private readonly IMailService _emailService;
        public UserController(IUserInfoService service, ILogger<UserController> logger, IMailService emailService)
        {
            _logger = logger;
            _service = service;
            _emailService = emailService;
        }
        [HttpPost("CreateUser")]
        public async Task<IActionResult> CreateUser([FromBody] UserInfoDto userInfo)
        {
            try
            {
                var result = await _service.CreateUser(userInfo);
                if (result >= 1)
                {
                    return Ok(JsonResultVm<int>.SuccessResponse("Email sent.", result));
                }

                return BadRequest(JsonResultVm<int>.FailResponse("something worng during create user.", "Error..", result));
            }
            catch (Exception ex)
            {
                return BadRequest(JsonResultVm<string>.FailResponse(ex.Message, userInfo.EmailId));
            }
        }

        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] string emailId)
        {
            try
            {
                var result = await _service.ForgotPassword(emailId);
                if (result >= 1)
                {
                    return Ok(JsonResultVm<int>.SuccessResponse("Email sent with reset password.", result));
                }

                return BadRequest(JsonResultVm<int>.FailResponse("something worng during reset password.", "Error..", result));

            }
            catch (Exception ex)
            {
                return BadRequest(JsonResultVm<string>.FailResponse(ex.Message, emailId));
            }
        }

        [HttpPost("SignIn")]
        public async Task<IActionResult> SignIn([FromBody] UserSignin userSignin)
        {
            try
            {
                var userProfile = await _service.UserSigin(userSignin);
                return Ok(JsonResultVm<UserProfile>.SuccessResponse("Login Success.", userProfile));

            }
            catch(Exception ex)
            {
               return BadRequest(JsonResultVm<string>.FailResponse(ex.Message, userSignin.EmailId));
            }
        }

        [HttpPost("TestEmail")]
        public async Task<IActionResult> TestEmail([FromBody] string emailId)
        {
            try
            {
                _logger.LogInformation("Attempting to Test Email with emailId: {EmailId}", emailId);

                await _emailService.SendEmailAsync(emailId, "Welcome !!!!", $"This is your test email.<br/><br/> Thanks");

                _logger.LogInformation("Test email Sent successfully");

                return Ok(JsonResultVm<int>.SuccessResponse("Email sent.", 1));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while Test email {EmailId}", emailId);
                return BadRequest(JsonResultVm<string>.FailResponse(ex.Message, emailId));
            }
        }
    }
}

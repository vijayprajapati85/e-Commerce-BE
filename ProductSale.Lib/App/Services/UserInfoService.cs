using ProductSale.Lib.App.Constants;
using ProductSale.Lib.App.Exceptions;
using ProductSale.Lib.App.Extensions;
using ProductSale.Lib.App.Models;
using ProductSale.Lib.App.Models.Cart;
using ProductSale.Lib.App.Models.Email;
using ProductSale.Lib.Infra.Repo;
using System.Text.RegularExpressions;
using static Humanizer.In;

namespace ProductSale.Lib.App.Services
{
    public class UserInfoService : IUserInfoService
    {
        private readonly IUserInfoRepository _repository;
        private readonly IMailService _emailService;
        private readonly ITokenService _tokenService;
        private readonly ICartInfoService _cartInfoService;
        public UserInfoService(IUserInfoRepository repository, IMailService emailService, ITokenService tokenService, ICartInfoService cartInfoService)
        {
            _repository = repository;
            _emailService = emailService;
            _tokenService = tokenService;
            _cartInfoService = cartInfoService;
        }

        public async Task<int> ForgotPassword(string emailId)
        {
            try
            {
                bool validEmail = emailId.ValidEmail();
                if (!validEmail)
                {
                    throw new BusinessRuleException("Email address is not valid.");
                }

                var user = await _repository.GetUserByEmail(emailId);

                if (user == null)
                {
                    throw new BusinessRuleException("Email not exist.");
                }

                string password = PasswordGeneratHelper.GeneratePassword();

                EmailCommand emailCommand = new EmailCommand
                {
                    EmailType = RecipientType.ForgotPassword,
                    EmailData = new Dictionary<string, string>
                    {
                        { "RecipientName", user.FullName },
                        { "RecipientEmail", emailId },
                        { "Password", password },
                    }
                };

                var isEmailSend = await _emailService.SendEmailAsync(emailCommand);

                return await _repository.UpdatePassword(new UserInfo
                {
                    Id = user.Id,
                    EmailId = emailId,
                    Password = password,
                    EmailSend = isEmailSend,
                });
            }
            catch
            {
                throw;
            }
        }
        public async Task<int> CreateUser(UserInfoDto userInfo)
        {
            try
            {
                bool validEmail = userInfo.EmailId.ValidEmail();
                if (!validEmail)
                {
                    throw new BusinessRuleException("Email address is not valid.");
                }

                var user = await _repository.GetUserByEmail(userInfo.EmailId);

                if (user != null)
                {
                    throw new BusinessRuleException("Email already exist.");
                }

                string password = PasswordGeneratHelper.GeneratePassword();

                EmailCommand emailCommand = new EmailCommand
                {
                    EmailType = RecipientType.SignUp,
                    EmailData = new Dictionary<string, string>
                    {
                        { "RecipientName", userInfo.FullName },
                        { "RecipientEmail", userInfo.EmailId },
                        { "Password", password },
                    }
                };

                var isEmailSend =  await _emailService.SendEmailAsync(emailCommand);

                return await _repository.CreateUser(new UserInfo
                {
                    FullName = userInfo.FullName,
                    EmailId = userInfo.EmailId,
                    Password = password,
                    EmailSend = isEmailSend,
                });
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Task<UserInfo?> GetUserByEmail(string emailId)
        {
            throw new NotImplementedException();
        }

        public Task<int> UpdateEmailSendStatus(UserInfoDto userInfoDto)
        {
            throw new NotImplementedException();
        }

        public Task<int> UpdatePassword(UserInfoDto userInfoDto)
        {
            throw new NotImplementedException();
        }

        public async Task<UserProfile> UserSigin(UserSignin userSignin, string folderPath)
        {
            try
            {
                bool validEmail = userSignin.EmailId.ValidEmail();
                if (!validEmail)
                {
                    throw new BusinessRuleException("Email address is not valid.");
                }

                var user = await _repository.GetUserByEmail(userSignin.EmailId);

                if (user != null && string.Equals(userSignin.Password,user.Password))
                {
                   string token = _tokenService.GenerateToken(user);

                    _cartInfoService.UserId = user.Id;
                    List<Order>? orders = await _cartInfoService.GetPendingCartAsync();
                    if (orders != null)
                    {
                        orders.ForEach(result =>
                        {
                            if (!string.IsNullOrEmpty(result.ImageName))
                            {
                                result.ImageName = folderPath + $"{result.ImageName}";
                            }
                        });
                    }
                    return new UserProfile
                    {
                        FullName = user.FullName,
                        EmailId = user.EmailId,
                        Address = user.Address,
                        MobileNo = user.MobileNo,
                        Token = token,
                        OrderData = orders
                    };
                }
             
                throw new BusinessRuleException("Login Credential not match");
            }
            catch(Exception)
            {
                throw;
            }
        }

        public async Task<UserProfile> AdminSignin(UserSignin userSignin)
        {
            try 
            {
                var user = await _repository.GetUserByEmail(userSignin.EmailId);
                if (user != null && string.Equals(userSignin.Password, user.Password))
                {
                  string token = _tokenService.GenerateToken(user);

                    return new UserProfile
                    {
                        FullName = user.FullName,
                        EmailId = user.EmailId,
                        Token = token,
                        OrderData = null
                    };
                }
                throw new BusinessRuleException("Login Credential not match");
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<int> UpdateProfile(Profile profile)
        {
            try
            {
               return await _repository.UpdateProfile(profile);
            }
            catch(Exception)
            {
                throw;
            }
        }
    }
}

using ProductSale.Lib.App.Exceptions;
using ProductSale.Lib.App.Extensions;
using ProductSale.Lib.App.Models;
using ProductSale.Lib.Infra.Repo;
using System.Text.RegularExpressions;

namespace ProductSale.Lib.App.Services
{
    public class UserInfoService : IUserInfoService
    {
        private readonly IUserInfoRepository _repository;
        private readonly IMailService _emailService;
        private readonly ITokenService _tokenService;
        public UserInfoService(IUserInfoRepository repository, IMailService emailService, ITokenService tokenService)
        {
            _repository = repository;
            _emailService = emailService;
            _tokenService = tokenService;
        }

        public async Task<int> ForgotPassword(string emailId)
        {
            try
            {
                bool validEmail = checkValidEmailId(emailId);
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

                var isEmailSend = await _emailService.SendEmailAsync(emailId, "Generate new password", $"Your password is <b>{password}</b>. you may login with this credential and email.<br/><br/> Thanks");

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
                bool validEmail = checkValidEmailId(userInfo.EmailId);
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

                var isEmailSend =  await _emailService.SendEmailAsync(userInfo.EmailId, "Welcome !!!!", $"Your password is <b>{password}</b>. you may login with this credential and email.<br/><br/> Thanks");

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

        public async Task<UserProfile> UserSigin(UserSignin userSignin)
        {
            try
            {
                string token = string.Empty;
                bool validEmail = checkValidEmailId(userSignin.EmailId);
                if (!validEmail)
                {
                    throw new BusinessRuleException("Email address is not valid.");
                }

                var user = await _repository.GetUserByEmail(userSignin.EmailId);

                if (user != null && string.Equals(userSignin.Password,user.Password))
                {
                    token = _tokenService.GenerateToken(userSignin);

                    return new UserProfile
                    {
                        FullName = user.FullName,
                        EmailId = user.EmailId,
                        Token = token,
                    };
                }
             
                throw new BusinessRuleException("Login Credential not match");
            }
            catch(Exception)
            {
                throw;
            }
        }

        private bool checkValidEmailId(string email)
        {
            try
            {
                string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
                return Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase);
            }
            catch
            {
                return false;
            }
        }
    }
}

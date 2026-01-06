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
        public UserInfoService(IUserInfoRepository repository, IMailService emailService)
        {
            _repository = repository;
            _emailService = emailService;
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

                await _emailService.SendEmailAsync(userInfo.FullName, "Welcome !!!!", $"Your password is <b>{password}</b>. you may login with this credential and email.<br/><br/> Thanks");

                return await _repository.CreateUser(new UserInfo
                {
                    FullName = userInfo.FullName,
                    EmailId = userInfo.EmailId,
                    Password = password,
                    EmailSend = false,
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

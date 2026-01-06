using ProductSale.Lib.App.Models;

namespace ProductSale.Lib.App.Services
{
    public interface IUserInfoService
    {
        Task<int> CreateUser(UserInfoDto userInfo);
        Task<UserInfo?> GetUserByEmail(string emailId);
        Task<int> UpdateEmailSendStatus(UserInfoDto userInfoDto);
        Task<int> UpdatePassword(UserInfoDto userInfoDto);
    }
}

using ProductSale.Lib.App.Models;

namespace ProductSale.Lib.Infra.Repo
{
    public interface IUserInfoRepository
    {
        Task<int> CreateUser(UserInfo userInfo);
        Task<UserInfo?> GetUserByEmail(string emailId);
        Task<int> UpdateEmailSendStatus(UserInfo userInfo);
        Task<int> UpdatePassword(UserInfo userInfo);
    }
}

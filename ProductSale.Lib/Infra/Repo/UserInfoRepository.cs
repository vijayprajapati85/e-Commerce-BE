using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using ProductSale.Lib.App.Models;
using SqlKata.Compilers;
using SqlKata.Execution;

namespace ProductSale.Lib.Infra.Repo
{
    public class UserInfoRepository : IUserInfoRepository
    {
        private const string TableName = "UserInfo";
        public QueryFactory queryFactory { get; }
        public UserInfoRepository(IConfiguration configuration)
        {

            queryFactory = new QueryFactory(
                connection: new SqlConnection(configuration["EcomProduct"]),
                compiler: new SqlServerCompiler()
                );
        }
        public async Task<int> CreateUser(UserInfo userInfo)
        {
            try
            {
                return await queryFactory.Query(TableName)
                    .InsertAsync(new
                    {
                        fullname = userInfo.FullName,
                        emailId = userInfo.EmailId,
                        password = userInfo.Password,
                        emailsend = userInfo.EmailSend,
                        createddatetime = DateTime.Now,
                        updateddatetime = DateTime.Now
                    });
            }
            catch (Exception)
            {
                return 0;
            }
        }
        public async Task<UserInfo?> GetUserByEmail(string emailId)
        {
            try
            {

                var result = await queryFactory.Query(TableName)
                    .Where("emailId", emailId)
                    .GetAsync<UserInfo>();

                return result?.FirstOrDefault();

            }
            catch(Exception)
            {
                return null;
            }
        }
        public async Task<int> UpdateEmailSendStatus(UserInfo userInfo)
        {
            try
            {
                return await queryFactory.Query(TableName)
                    .Where("emailId", userInfo.EmailId)
                    .Where("id",userInfo.Id)
                    .UpdateAsync(new
                    {
                        emailsend = userInfo.EmailSend,
                    });

            }
            catch(Exception)
            {
                return 0;
            }
        }

        public async Task<int> UpdatePassword(UserInfo userInfo)
        {
            try
            {
                return await queryFactory.Query(TableName)
                     .Where("emailId", userInfo.EmailId)
                     .Where("id", userInfo.Id)
                         .UpdateAsync(new
                        {
                             emailsend = userInfo.EmailSend,
                             password = userInfo.Password,
                        });
            }
            catch(Exception)
            {
                return 0;
            }
        }
    }
}

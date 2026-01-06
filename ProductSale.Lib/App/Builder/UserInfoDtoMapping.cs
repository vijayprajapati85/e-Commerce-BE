using ProductSale.Lib.App.Models;
using ProductSale.Lib.Domain;

namespace ProductSale.Lib.App.Builder
{
    public class UserInfoDtoMapping
    {
        protected UserInfoDtoMapping() { }

        public static List<UserInfoDto> SetCategory(IEnumerable<UserInfo> userInfos)
        {
            return userInfos.Select(MapUserInfoDomainToDto).ToList();
        }
        public static UserInfoDto SetCategory(UserInfo userInfo)
        {
            return MapUserInfoDomainToDto(userInfo);
        }
        private static UserInfoDto MapUserInfoDomainToDto(UserInfo userInfo)
        {
            if (userInfo == null) return new UserInfoDto();

            return new UserInfoDto
            {
                Id = userInfo.Id,
                EmailId = userInfo.EmailId,
                EmailSend = userInfo.EmailSend,
            };
        }
    }
}

using ProductSale.Lib.App.Models;

namespace ProductSale.Lib.App.Services
{
    public interface ITokenService
    {
        string GenerateToken(UserSignin userSignin);
    }
}

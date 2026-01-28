using ProductSale.Lib.App.Models.Cart;

namespace ProductSale.Lib.Infra.Repo
{
    public interface ICartInfoRepository
    {
        Task<int> UpsertCartInfoAsync(CartInfo cartInfo);
        Task<List<CartInfo>?> GetCartByUserIdAsync(long userId);
        Task<List<Order>?> GetPendingCartAsync(long userId);
        Task<int> DeleteCartInfoAsync(CartInfo cartInfo);
        Task<(int, long)> CartOrderAsync(long userId);
        Task<List<Order>?> GetOrderByUserIdAsync(long userId, string orderId = "");
    }
}

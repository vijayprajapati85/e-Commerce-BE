using ProductSale.Lib.App.Models.Cart;

namespace ProductSale.Lib.Infra.Repo
{
    public interface ICartInfoRepository
    {
        Task<int> UpsertCartInfoAsync(CartInfo cartInfo);
        Task<List<CartInfo>?> GetCartByUserIdAsync(long userId);
        Task<List<Order>?> GetCartByStatusAsync(long userId, string status);
        Task<int> DeleteCartInfoAsync(CartInfo cartInfo);
        Task<(int, long)> CartOrderAsync(long userId);
        Task<List<Order>?> GetOrderByUserIdAsync(long userId, string orderId = "");
        Task<List<long>> GetUserIdPendingCart();
    }
}

using ProductSale.Lib.App.Models.Cart;

namespace ProductSale.Lib.App.Services
{
    public interface ICartInfoService
    {
        long UserId { get; set; }
        Task<int> UpsertCartInfoAsync(CartRequestDto cartInfo);
        Task<List<CartInfo>?> GetCartByUserIdAsync();
        Task<List<Order>?> GetPendingCartAsync();
        Task<List<OrderData>?> GetCartByStatusAsync(string status, string folderPath);
        Task<int> DeleteCartInfoAsync(int productId);
        Task<string> CartOrderAsync();
    }
}

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using ProductSale.Lib.App.Constants;
using ProductSale.Lib.App.Models.Cart;
using ProductSale.Lib.App.Models.Email;
using ProductSale.Lib.Infra.Repo;
using SqlKata.Execution;

namespace ProductSale.Lib.App.Services
{
    public class CartInfoService : ICartInfoService
    {
        private readonly ICartInfoRepository _repository;
        private readonly IUserInfoRepository _userrepository;
        private readonly ILogger<CartInfoService> _logger;
        private readonly IMemoryCache _cache;

        private readonly IMailService _emailService;

        public long UserId { get; set; }
        public CartInfoService(ICartInfoRepository repository, ILogger<CartInfoService> logger, IMemoryCache cache, IMailService emailService, IUserInfoRepository userrepository)
        {
            _repository = repository;
            _logger = logger;
            _cache = cache;
            _emailService = emailService;
            _userrepository = userrepository;
        }

        public async Task<int> UpsertCartInfoAsync(CartRequestDto cartData)
        {
            _logger.LogInformation("Inside UpsertCartInfoAsync ===");
            try
            {
                if (UserId == 0)
                {
                    _logger.LogWarning("UserId is not set in CartInfoService.");
                    return 0;
                }

                int result = 0;

                //foreach (var cartData in cartInfoReq)
                {
                    var existingCart = await _repository.GetCartByUserIdAsync(UserId);

                    CartInfo cartInfo = new CartInfo();

                    if (existingCart != null && existingCart.Count > 0)
                    {
                        var prodctData = existingCart.Where(x => x.ProductId == cartData.ProductId).FirstOrDefault();
                        if (prodctData != null)
                        {

                            if (prodctData.ProductId == cartData.ProductId && prodctData.Quantity == cartData.Quantity && prodctData.Status == CartStatus.Pending)
                            {
                                return 1;
                            }

                            cartInfo.Id = prodctData.Id;
                        }
                    }

                    cartInfo.UserId = UserId;
                    cartInfo.Quantity = cartData.Quantity;
                    cartInfo.ProductId = cartData.ProductId;
                    cartInfo.Status = CartStatus.Pending;
                    cartInfo.CreatedDateTime = cartInfo.UpdatedDateTime = DateTime.Now;

                    result = await _repository.UpsertCartInfoAsync(cartInfo);
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in UpsertCartInfoAsync: {Message}", ex.Message);
                return 0;
            }
        }

        public Task<List<CartInfo>?> GetCartByUserIdAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<int> DeleteCartInfoAsync(int productId)
        {
            _logger.LogInformation("Inside DeleteCartInfoAsync ===");
            try
            {
                if (UserId == 0)
                {
                    _logger.LogInformation("UserId is not set in CartInfoService.");
                    return 0;
                }

                CartInfo cart = new CartInfo();
                cart.UserId = UserId;
                cart.ProductId = productId;

                return await _repository.DeleteCartInfoAsync(cart);

            }
            catch (Exception ex)
            {
                _logger.LogError("Error in DeleteCartInfoAsync: {Message}", ex.Message);
                return 0;
            }
        }

        public async Task<string> CartOrderAsync()
        {
            _logger.LogInformation("Inside CartOrderAsync ===");
            try
            {
                if (UserId == 0)
                {
                    _logger.LogInformation("UserId is not set in CartOrderAsync.");
                    return "Login Required.";
                }

                var (result, orderId) = await _repository.CartOrderAsync(UserId);

                if (result == 0)
                {
                    return "Something wrong order booking.";
                }

                var user = await _userrepository.GetUserById(UserId);

                EmailCommand emailCommand = new EmailCommand
                {
                    EmailType = RecipientType.Order,
                    EmailData = new Dictionary<string, string>
                    {
                        { "RecipientName", user.FullName },
                        { "RecipientEmail", user.EmailId },
                        { "UserId", UserId.ToString() },
                        { "OrderId", orderId.ToString() }
                    }
                };

                var isEmailSend = await _emailService.SendEmailAsync(emailCommand);

                return isEmailSend ? "Order success." : "Order Email fail.";

            }
            catch (Exception ex)
            {
                _logger.LogError("Error in CartOrderAsync: {Message}", ex.Message);
                return "Something wrong order booking.";
            }
        }

        public async Task<List<Order>?> GetPendingCartAsync()
        {
            _logger.LogInformation("Inside GetPendingCartAsync ===");
            try
            {
                if (UserId == 0)
                {
                    _logger.LogWarning("Invalid userId provided: {UserId}", UserId);
                    return null;
                }

                return await _repository.GetPendingCartAsync(UserId);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetPendingCartAsync: {Message}", ex.Message);
                return null;
            }
        }
    }
}

using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ProductSale.Lib.App.Constants;
using ProductSale.Lib.App.Models.Cart;
using SqlKata;
using SqlKata.Compilers;
using SqlKata.Execution;

namespace ProductSale.Lib.Infra.Repo
{
    public class CartInfoRepository : ICartInfoRepository
    {
        private const string TableName = "CartInfo";
        private const string ProductInfo = "ProductInfo";
        private readonly ILogger<CartInfoRepository> _logger;

        public QueryFactory queryFactory { get; }
        public CartInfoRepository(IConfiguration configuration, ILogger<CartInfoRepository> logger)
        {

            queryFactory = new QueryFactory(
                connection: new SqlConnection(configuration["EcomProduct"]),
                compiler: new SqlServerCompiler()
                );

            _logger = logger;
        }
        public async Task<List<CartInfo>?> GetCartByUserIdAsync(long userId)
        {
            _logger.LogInformation("Inside GetCartByUserIdAsync ===");
            try
            {
                if (userId == 0)
                {
                    _logger.LogInformation("Invalid userId provided: {UserId}", userId);
                    return null;
                }
                var cartInfos = await queryFactory.Query(TableName)
                    .Where("UserId", userId)
                    .Where(q => q.Where("Status", CartStatus.Pending)
                                .OrWhere("Status", CartStatus.Deleted))
                    .GetAsync<CartInfo>();

                return cartInfos?.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetCartByUserIdAsync: {Message}", ex.Message);
                return null;
            }
        }

        public async Task<int> UpsertCartInfoAsync(CartInfo cartInfo)
        {
            _logger.LogInformation("Inside UpsertCartInfoAsync ===");
            try
            {
                if (cartInfo.Id != 0)
                {
                    return await queryFactory.Query(TableName)
                        .Where("Id", cartInfo.Id)
                        .UpdateAsync(new
                        {
                            UserId = cartInfo.UserId,
                            ProductId = cartInfo.ProductId,
                            Quantity = cartInfo.Quantity,
                            Status = cartInfo.Status,
                            UpdatedDateTime = cartInfo.UpdatedDateTime
                        });
                }

                return await queryFactory.Query(TableName)
                    .InsertAsync(new
                    {
                        UserId = cartInfo.UserId,
                        ProductId = cartInfo.ProductId,
                        Quantity = cartInfo.Quantity,
                        Status = cartInfo.Status,
                        CreatedDateTime = cartInfo.CreatedDateTime
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in UpsertCartInfoAsync: {Message}", ex.Message);
                return 0;
            }

        }

        public async Task<int> DeleteCartInfoAsync(CartInfo cartInfo)
        {
            _logger.LogInformation("Inside DeleteCartInfoAsync ===");
            try
            {
                if (cartInfo.UserId != 0 && cartInfo.ProductId != 0)
                {
                    return await queryFactory.Query(TableName)
                        .Where("UserId", cartInfo.UserId)
                        .Where("ProductId", cartInfo.ProductId)
                        .UpdateAsync(new
                        {
                            Status = CartStatus.Deleted,
                            UpdatedDateTime = DateTime.Now
                        });
                }

                return 0;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in DeleteCartInfoAsync: {Message}", ex.Message);
                return 0;
            }
        }

        public async Task<(int, long)> CartOrderAsync(long userId)
        {
            _logger.LogInformation("Inside CartOrderAsync ===");
            try
            {
                long orderId = DateTime.Now.Ticks;
                if (userId != 0)
                {
                    var result = await queryFactory.Query(TableName)
                        .Where("UserId", userId)
                        .Where("Status", CartStatus.Pending)
                        .UpdateAsync(new
                        {
                            Status = CartStatus.InProgress,
                            OrderId = orderId,
                            UpdatedDateTime = DateTime.Now
                        });

                    return (result, orderId);
                }

                return (0, 0);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in CartOrderAsync: {Message}", ex.Message);
                return (0, 0);
            }
        }

        public async Task<List<Order>?> GetOrderByUserIdAsync(long userId, string orderId = "")
        {
            _logger.LogInformation("Inside GetOrderByUserIdAsync ===");
            try
            {
                if (userId != 0 && (string.IsNullOrEmpty(orderId) || string.IsNullOrWhiteSpace(orderId)))
                {
                    return (List<Order>)await queryFactory.Query(ProductInfo)
                        .LeftJoin(TableName, "CartInfo.ProductId", "ProductInfo.Id")
                        .Where("CartInfo.UserId", userId)
                        .Where("CartInfo.Status", CartStatus.Pending)
                        .Select("ProductInfo.Name as Name", "ProductInfo.Price as Price", "CartInfo.Quantity as Quantity", "CartInfo.OrderId as OrderId")
                        .GetAsync<Order>();
                }
                else if (userId != 0 && !string.IsNullOrEmpty(orderId))
                {
                    return (List<Order>)await queryFactory.Query(ProductInfo)
                       .LeftJoin(TableName, "CartInfo.ProductId", "ProductInfo.Id")
                       .Where("CartInfo.UserId", userId)
                       .Where("CartInfo.OrderId", orderId)
                       .Where("CartInfo.Status", CartStatus.InProgress)
                       .Select("ProductInfo.Name as Name", "ProductInfo.Price as Price", "CartInfo.Quantity as Quantity", "CartInfo.OrderId as OrderId")
                       .GetAsync<Order>();
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetOrderByUserIdAsync: {Message}", ex.Message);
                return null;
            }
        }

        public async Task<List<Order>?> GetCartByStatusAsync(long userId, string status)
        {
            _logger.LogInformation("Inside GetPendingCartAsync ===");
            try
            {
                if (userId == 0)
                {
                    _logger.LogInformation("Invalid userId provided: {UserId}", userId);
                    return null;
                }
                var query = queryFactory.Query(ProductInfo)
                    .LeftJoin(TableName, "CartInfo.ProductId", "ProductInfo.Id")
                    // .Where("CartInfo.UserId", userId != -1 ? userId : "CartInfo.UserId")
                    .Where("CartInfo.Status", status)
                    .Select("ProductInfo.Id as ProductId",
                            "ProductInfo.ImageName as ImageName",
                            "ProductInfo.Name as Name",
                            "ProductInfo.Price as Price",
                            "CartInfo.Quantity as Quantity",
                            "CartInfo.OrderId as OrderId")
                    .SelectRaw(
                            "CAST(" +
                            "CASE " +
                            "WHEN CartInfo.CreatedDateTime > CartInfo.UpdatedDateTime THEN CartInfo.CreatedDateTime " +
                            "ELSE CartInfo.UpdatedDateTime " +
                            "END AS DATE" +
                            ") AS OrderDate"
                    );
                // .GetAsync<Order>();

                query = query.When(userId != -1, q =>
                {
                    return q.Where("CartInfo.UserId", userId);
                });

                var orders = (List<Order>)await query.GetAsync<Order>();

                return orders;

            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetPendingCartAsync: {Message}", ex.Message);
                return null;
            }
        }

        public async Task<List<long>> GetUserIdPendingCart()
        {
            _logger.LogInformation("Inside GetUserIdPendingCart ===");
            try
            {
                var query = await queryFactory.Query("CartInfo")
                            .Select("UserId")
                            .Distinct()
                            .Where("Status", "Pending")
                            .GetAsync<long>();

                return query.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetUserIdPendingCart: {Message}", ex.Message);
                return new List<long>();
            }
}
    }
}

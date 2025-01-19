using OnlineShop.Data.Common;
using OnlineShop.Data.Models;

namespace OnlineShop.Data.Interfaces;

public interface IProductService
{
    Task<Result<IEnumerable<Category>>> GetCategoriesAsync();
    Task<Result<Page<Product>>> GetProductsAsync(PageRequest pageRequest, Range priceRange, string? categoryLabel = null);
    Task<Result<Product>> GetProductByIdAsync(string id);
    Task<Result<bool>> ProductInCartAsync(string productId, string userId);
    Task<Result<Page<Order>>> GetOrdersAsync(string userId);
    Task<Result<Order>> GetOrderByIdAsync(string orderId, string userId);
    Task<Result<bool>> OrderProductsAsync(string userId);
}

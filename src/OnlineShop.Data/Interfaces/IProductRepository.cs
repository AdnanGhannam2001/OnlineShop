using OnlineShop.Data.Common;
using OnlineShop.Data.Models;

namespace OnlineShop.Data.Interfaces;

internal interface IProductRepository
{
    Task<IEnumerable<Category>> GetCategoriesAsync();
    Task<Page<Product>> GetProductsAsync(PageRequest pageRequest, Range priceRange, string? categoryLabel = null);
    Task<Product?> GetProductByIdAsync(string id);
    Task<bool> ProductInCartAsync(string productId, string userId);
    Task<Page<Order>> GetOrdersAsync(string userId);
    Task<Order?> GetOrderByIdAsync(string orderId, string userId);
    Task<bool> OrderProductsAsync(string userId);
}
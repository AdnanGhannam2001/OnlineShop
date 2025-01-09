using OnlineShop.Data.Common;
using OnlineShop.Data.Models;

namespace OnlineShop.Data.Interfaces;

public interface IProductService
{
    Task<IEnumerable<Category>> GetCategoriesAsync();
    Task<Page<Product>> GetProductsAsync(PageRequest pageRequest, Range priceRange, string? categoryLabel = null);
    Task<Product?> GetProductByIdAsync(string id);
    Task<bool> ProductInCartAsync(string productId, string userId);
}
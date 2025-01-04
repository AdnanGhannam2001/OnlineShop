using OnlineShop.Data.Common;
using OnlineShop.Data.Models;

namespace OnlineShop.Data.Interfaces;

public interface IProductService
{
    Task<IEnumerable<Category>> GetCategories();
    Task<Page<Product>> GetProducts(PageRequest pageRequest, Range priceRange, string? categoryLabel = null);
}
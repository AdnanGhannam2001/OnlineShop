using Microsoft.Extensions.Caching.Memory;
using OnlineShop.Data.Common;
using OnlineShop.Data.Interfaces;
using OnlineShop.Data.Models;

namespace OnlineShop.Data.Services;

internal class ProductService : IProductService
{
    private readonly IProductRepository _repo;
    private readonly IMemoryCache _cache;

    public ProductService(IProductRepository repo, IMemoryCache cache)
    {
        _repo = repo;
        _cache = cache;
    }

    public async Task<Result<IEnumerable<Category>>> GetCategoriesAsync()
    {
        var cached = await _cache.GetOrCreateAsync("Categories",
            async entiry =>
            {
                entiry.SlidingExpiration = TimeSpan.FromMinutes(10);
                entiry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15);
                return await _repo.GetCategoriesAsync();
            });

        return new(cached!);
    }

    public async Task<Result<Page<Product>>> GetProductsAsync(PageRequest pageRequest, Range priceRange, string? categoryLabel = null)
    {
        var cached = await _cache.GetOrCreateAsync(
            $"Products-{pageRequest.Number}-{pageRequest.Size}-{priceRange.Start}-{priceRange.End}-{categoryLabel}",
            async entiry =>
            {
                entiry.SlidingExpiration = TimeSpan.FromSeconds(100);
                entiry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2);
                return await _repo.GetProductsAsync(pageRequest, priceRange, categoryLabel);
            });

        return cached!;
    }

    public async Task<Result<Product>> GetProductByIdAsync(string id)
    {
        var cached = await _cache.GetOrCreateAsync("Product",
            async entiry =>
            {
                entiry.SlidingExpiration = TimeSpan.FromMinutes(10);
                entiry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15);
                return await _repo.GetProductByIdAsync(id);
            });

        if (cached is null)
        {
            return new ExceptionBase("Product", "Product was not found");
        }

        return cached;
    }

    public async Task<Result<bool>> ProductInCartAsync(string productId, string userId)
    {
        return new(await _repo.ProductInCartAsync(productId, userId));
    }

    public async Task<Result<Page<Order>>> GetOrdersAsync(string userId)
    {
        return new(await _repo.GetOrdersAsync(userId));
    }

    public async Task<Result<Order>> GetOrderByIdAsync(string orderId, string userId)
    {
        var cached = await _cache.GetOrCreateAsync("Order",
            async entiry =>
            {
                entiry.SlidingExpiration = TimeSpan.FromMinutes(10);
                entiry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15);
                return await _repo.GetOrderByIdAsync(orderId, userId);
            });

        if (cached is null)
        {
            return new ExceptionBase("Order", "Order was not found");
        }

        return cached;
    }

    public async Task<Result<bool>> OrderProductsAsync(string userId)
    {
        return new(await _repo.OrderProductsAsync(userId));
    }
}

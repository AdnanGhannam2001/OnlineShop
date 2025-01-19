using Microsoft.Extensions.Caching.Memory;
using OnlineShop.Data.Common;
using OnlineShop.Data.Interfaces;
using OnlineShop.Data.Models;

namespace OnlineShop.Data.Services;

internal class AppUserService : IAppUserService
{
    private readonly IAppUserRepository _repo;
    private readonly IMemoryCache _cache;

    public AppUserService(IAppUserRepository repo, IMemoryCache cache)
    {
        _repo = repo;
        _cache = cache;
    }

    public async Task<Result<AppUser>> GetUserByIdAsync(string id)
    {
        var cached = await _cache.GetOrCreateAsync($"User-{id}",
            async entry =>
            {
                entry.SlidingExpiration = TimeSpan.FromMinutes(10);
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15);
                return await _repo.GetUserByIdAsync(id);
            });

        if (cached is null)
        {
            return new ExceptionBase("User", "User was not found");
        }

        return cached;
    }

    public async Task<Result<AppUser>> GetUserByNameAsync(string username)
    {
        var cached = await _cache.GetOrCreateAsync($"User-{username}",
            async entry =>
            {
                entry.SlidingExpiration = TimeSpan.FromMinutes(10);
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15);
                return await _repo.GetUserByNameAsync(username);
            });

        if (cached is null)
        {
            return new ExceptionBase("User", $"User '{username}' was not found");
        }

        return cached!;
    }

    public async Task<Result<bool>> CreateUserAsync(AppUser user)
    {
        return new(await _repo.CreateUserAsync(user));
    }

    public async Task<Result<Page<UserProduct>>> GetUsersProductsAsync(string userId, PageRequest pageRequest)
    {
        var cached = await _cache.GetOrCreateAsync($"UserProducts-{userId}",
            async entry =>
            {
                entry.SlidingExpiration = TimeSpan.FromMinutes(10);
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15);
                return await _repo.GetUsersProductsAsync(userId, pageRequest);
            });
        
        return cached!;
    }

    public async Task AddToCartAsync(UserProduct item)
    {
        _cache.Remove($"UserProducts-{item.UserId}");
        await _repo.AddToCartAsync(item);
    }

    public async Task RemoveFromCartAsync(string productId, string userId)
    {
        _cache.Remove($"UserProducts-{userId}");
        await _repo.RemoveFromCartAsync(productId, userId);
    }
}

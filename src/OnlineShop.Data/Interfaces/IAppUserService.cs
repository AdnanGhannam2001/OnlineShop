using OnlineShop.Data.Common;
using OnlineShop.Data.Models;

namespace OnlineShop.Data.Interfaces;

public interface IAppUserService
{
    Task<AppUser?> GetUserByIdAsync(string id);
    Task<AppUser?> GetUserByNameAsync(string username);
    Task<bool> CreateUserAsync(AppUser user);

    Task<Page<UsersProducts>> GetUsersProductsAsync(string userId, PageRequest pageRequest);
    Task AddToCartAsync(string productId, string userId);
    Task RemoveFromCartAsync(string productId, string userId);
}

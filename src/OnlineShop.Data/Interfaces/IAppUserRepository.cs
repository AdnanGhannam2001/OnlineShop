using OnlineShop.Data.Common;
using OnlineShop.Data.Models;

namespace OnlineShop.Data.Interfaces;

internal interface IAppUserRepository
{
    Task<AppUser?> GetUserByIdAsync(string id);
    Task<AppUser?> GetUserByNameAsync(string username);
    Task<bool> CreateUserAsync(AppUser user);

    Task<Page<UserProduct>> GetUsersProductsAsync(string userId, PageRequest pageRequest);
    Task AddToCartAsync(UserProduct item);
    Task RemoveFromCartAsync(string productId, string userId);
}

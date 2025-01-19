using OnlineShop.Data.Common;
using OnlineShop.Data.Models;

namespace OnlineShop.Data.Interfaces;

public interface IAppUserService
{
    Task<Result<AppUser>> GetUserByIdAsync(string id);
    Task<Result<AppUser>> GetUserByNameAsync(string username);
    Task<Result<bool>> CreateUserAsync(AppUser user);

    Task<Result<Page<UserProduct>>> GetUsersProductsAsync(string userId, PageRequest pageRequest);
    Task AddToCartAsync(UserProduct item);
    Task RemoveFromCartAsync(string productId, string userId);
}

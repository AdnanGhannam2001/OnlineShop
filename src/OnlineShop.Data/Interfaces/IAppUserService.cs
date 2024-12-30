using OnlineShop.Data.Models;

namespace OnlineShop.Data.Interfaces;

public interface IAppUserService
{
    Task<AppUser?> GetUserByIdAsync(string id);
    Task<AppUser?> GetUserByNameAsync(string username);
}

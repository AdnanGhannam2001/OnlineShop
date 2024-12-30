using OnlineShop.Data.SqlModels;

namespace OnlineShop.Interfaces;

public interface IAppUserService
{
    Task<AppUser?> GetUserByIdAsync(string id);
    Task<AppUser?> GetUserByNameAsync(string username);
}

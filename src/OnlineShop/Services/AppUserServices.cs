using Dapper;
using OnlineShop.Data.SqlModels;
using OnlineShop.Interfaces;

namespace OnlineShop.Services;

internal class AppUserService : IAppUserService
{
    private readonly IDatabaseConnection _connection;

    public AppUserService(IDatabaseConnection connection)
    {
        _connection = connection;
    }

    public async Task<AppUser?> GetUserByIdAsync(string id)
    {
        using var db = _connection.Connection;
        return await db.QueryFirstOrDefaultAsync<AppUser>($"SELECT * FROM [AppUsers] WHERE [Id] = '{id}';");
    }

    public async Task<AppUser?> GetUserByNameAsync(string username)
    {
        using var db = _connection.Connection;
        return await db.QueryFirstOrDefaultAsync<AppUser>($"SELECT * FROM [AppUsers] WHERE [Username] = '{username}';");
    }
}

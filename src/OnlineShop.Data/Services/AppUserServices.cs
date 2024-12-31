using Dapper;
using OnlineShop.Data.Models;
using OnlineShop.Data.Interfaces;

namespace OnlineShop.Data.Services;

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
        return await db.QueryFirstOrDefaultAsync<AppUser>($"SELECT * FROM [AppUsers] WHERE [Id] = @Id;",
            new { Id = id });
    }

    public Task<AppUser?> GetUserByNameAsync(string username)
    {
        var db = _connection.Connection;
        return db.QueryFirstOrDefaultAsync<AppUser>($"SELECT * FROM [AppUsers] WHERE [Username] = @Username;",
            new { Username = username });
    }

    public async Task<bool> CreateUserAsync(AppUser user)
    {
        if (await GetUserByNameAsync(user.Username) is not null)
        {
            return false;
        }

        var db = _connection.Connection;
        await db.QueryAsync(
            "INSERT INTO [AppUsers] VALUES(@Id, @Username, @Role, @PasswordHash, @CreatedAt);",
            user);

        return true;
    }
}

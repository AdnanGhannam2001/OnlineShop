using Dapper;
using OnlineShop.Data.Models;
using OnlineShop.Data.Interfaces;
using OnlineShop.Data.Common;

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

    public async Task<Page<UsersProducts>> GetUsersProductsAsync(string userId, PageRequest pageRequest)
    {
        var db = _connection.Connection;
        var items = await db.QueryAsync<UsersProducts, Product, UsersProducts>($"""
            SELECT *
            FROM [UsersProducts] up
            LEFT JOIN [Products] p ON p.[Id] = up.[ProductId]
            WHERE up.[UserId] = @UserId;
        """,
            (up, p) =>
            {
                up.SetProduct(p);
                return up;
            },
            new { UserId = userId });

        var total = await db.QueryFirstAsync<int>($"""
            SELECT COUNT(*)
            FROM [UsersProducts]
            WHERE [UserId] = @UserId;
        """, new { UserId = userId });

        return new (total, items);
    }

    public Task AddToCartAsync(string productId, string userId)
    {
        var db = _connection.Connection;
        return db.QueryAsync($"""
            INSERT INTO [UsersProducts]
                ([UserId], [ProductId])
            VALUES (@UserId, @ProductId);
        """, new { UserId = userId, ProductId = productId });
    }

    public Task RemoveFromCartAsync(string productId, string userId)
    {
        var db = _connection.Connection;
        return db.QueryAsync($"""
            DELETE FROM [UsersProducts]
            WHERE [ProductId] = @ProductId AND [UserId] = @UserId;
        """, new { ProductId = productId, UserId = userId });
    }
}

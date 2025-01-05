using Dapper;
using OnlineShop.Data.Common;
using OnlineShop.Data.Interfaces;
using OnlineShop.Data.Models;

namespace OnlineShop.Data.Services;

internal class ProductService : IProductService
{
    private readonly IDatabaseConnection _connection;

    public ProductService(IDatabaseConnection connection)
    {
        _connection = connection;
    }

    public Task<IEnumerable<Category>> GetCategoriesAsync()
    {
        var db = _connection.Connection;
        return db.QueryAsync<Category>("SELECT * FROM [Categories];");
    }

    public async Task<Page<Product>> GetProductsAsync(PageRequest pageRequest, Range priceRange, string? categoryLabel = null)
    {
        var db = _connection.Connection;
        var inRange = $"p.[Cost] >= {priceRange.Start} AND p.[Cost] <= {priceRange.End}";
        var join = categoryLabel is null
            ? $"WHERE {inRange}" :
            $"""
                LEFT JOIN [Categories] c ON p.[CategoryId] = c.[Id]
                WHERE c.[Label] = @CategoryLabel AND {inRange}
            """;
        var orderBy = pageRequest.Desc ? "DESC" : "ASC";
        var products = await db.QueryAsync<Product>($"""
            SELECT p.*
            FROM [Products] p
            {join}
            ORDER BY [CreatedAt] {orderBy}
            OFFSET @Offset ROWS
            FETCH NEXT @Size ROWS ONLY;
        """, new { Offset = pageRequest.Size * pageRequest.Number, Size = pageRequest.Size, CategoryLabel = categoryLabel });

        var total = await db.QueryFirstAsync<int>($"""
            SELECT COUNT(*)
            FROM [Products] p
            {join};
        """, new { CategoryLabel = categoryLabel });

        return new(pageRequest.Size, total, products);
    }

    public async Task<Product?> GetProductByIdAsync(string id)
    {
        var db = _connection.Connection;
        return (await db.QueryAsync<Product, Category, Product>(
            """
                SELECT p.*, c.*
                FROM [Products] p
                LEFT JOIN [Categories] c ON c.[Id] = p.[CategoryId]
                WHERE p.[Id] = @Id;
            """,
            (p, c) =>
            {
                p.SetCategory(c);
                return p;
            }, new { Id = id })).FirstOrDefault();
    }

    public Task<bool> ProductInCartAsync(string productId, string userId)
    {
        var db = _connection.Connection;
        return db.QueryFirstAsync<bool>("""
            SELECT CASE WHEN (
                SELECT COUNT(*)
                FROM [UsersProducts]
                WHERE [ProductId] = @ProductId AND [UserId] = @UserId
            ) = 0
                THEN 0
                ELSE 1
            END AS IsEmpty;
        """, new { ProductId = productId, UserId = userId });
    }
}
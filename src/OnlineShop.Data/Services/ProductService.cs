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

    public Task<IEnumerable<Category>> GetCategories()
    {
        var db = _connection.Connection;
        return db.QueryAsync<Category>("SELECT * FROM [Categories];");
    }

    public async Task<Page<Product>> GetProducts(PageRequest pageRequest, Range priceRange, string? categoryLabel = null)
    {
        var db = _connection.Connection;
        var inRange = $"p.[Cost] >= {priceRange.Start} AND p.[Cost] <= {priceRange.End}";
        var join = categoryLabel is null
            ? inRange :
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
}
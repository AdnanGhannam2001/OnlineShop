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

    public async Task<Page<Product>> GetProducts(PageRequest pageRequest, string? categoryId = null)
    {
        var db = _connection.Connection;
        var condition = categoryId is null ? "" : "WHERE [CategoryId] = @CategoryId";
        var products = await db.QueryAsync<Product>($"""
            SELECT *
            FROM [Products]
            {condition}
            ORDER BY [CreatedAt]
            OFFSET @Offset ROWS
            FETCH NEXT @Size ROWS ONLY;
        """, new { Offset = pageRequest.Size * pageRequest.Number, Size = pageRequest.Size, CategoryId = categoryId });

        var total = await db.QueryFirstAsync<int>("SELECT COUNT(*) FROM [Products];");

        return new(pageRequest.Size, total, products);
    }
}
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

        return new(total, products);
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

    public async Task<Page<Order>> GetOrdersAsync(string userId)
    {
        var db = _connection.Connection;

        var items = await db.QueryAsync<Order>("""
            SELECT *
            FROM [Orders]
            WHERE [UserId] = @UserId;
        """, new { UserId = userId });

        var total = await db.QueryFirstAsync<int>("""
            SELECT COUNT(*)
            FROM [Orders]
            WHERE [UserId] = @UserId;
        """, new { UserId = userId });

        return new Page<Order>(total, items);
    }

    public async Task<Order?> GetOrderByIdAsync(string orderId, string userId)
    {
        var db = _connection.Connection;

        var order = await db.QueryFirstOrDefaultAsync<Order>("""
            SELECT *
            FROM [Orders] o
            WHERE o.[Id] = @OrderId AND o.[UserId] = @UserId;
        """, new {
                OrderId = orderId,
                UserId = userId,
                Offset = 0,
                Size = 10
            });

        if (order is not null)
        {
            var products = await db.QueryAsync<OrderProduct, Product, OrderProduct>("""
                SELECT *
                FROM [OrdersProducts] op
                LEFT JOIN [Products] p
                    ON op.[ProductId] = p.[Id]
                WHERE [OrderId] = @OrderId;
            """,
                (op, p) =>
                {
                    op.SetProduct(p);
                    return op;
                },
                new { OrderId = orderId });

            order.AddProducts(products);
        }

        return order;
    }

    public async Task<bool> OrderProductsAsync(string userId)
    {
        var db = _connection.Connection;

        var cart = await db.QueryAsync<UserProduct, Product, UserProduct>("""
            SELECT *
            FROM [UsersProducts] up
            LEFT JOIN [Products] p ON up.[ProductId] = p.[Id]
            WHERE up.[UserId] = @UserId;
        """,
            (up, p) =>
            {
                up.SetProduct(p);
                return up;
            },
            new { UserId = userId });

        if (!cart.Any())
        {
            return false;
        }

        var order = new Order(userId);
        await db.OpenAsync();
        using var tranaction = await db.BeginTransactionAsync();

        try
        {
            await db.QueryAsync("""
                DELETE FROM [UsersProducts]
                WHERE [UserId] = @UserId;
            """,
                new { UserId = userId },
                tranaction);

            await db.QueryAsync("""
                INSERT INTO [Orders] ([Id], [UserId])
                    VALUES (@Id, @UserId);
            """, order, tranaction);

            foreach (var item in cart)
            {
                if (item.Quantity > item.Product!.Quantity)
                {
                    await tranaction.RollbackAsync();
                    return false;
                }

                await db.QueryAsync("""
                    UPDATE [Products]
                    SET [Quantity] = @Quantity
                    WHERE [Id] = @Id;
                """,
                    new { Id = item.ProductId, Quantity = item.Product.Quantity - item.Quantity },
                    tranaction);

                await db.QueryAsync("""
                    INSERT INTO [OrdersProducts] ([OrderId], [ProductId], [Quantity])
                        VALUES (@OrderId, @ProductId, @Quantity);
                """,
                    new { OrderId = order.Id, item.ProductId, item.Quantity },
                    tranaction);
            }

            await tranaction.CommitAsync();
        }
        catch(Exception)
        {
            await tranaction.RollbackAsync();
            return false;
        }

        return true;
    }
}
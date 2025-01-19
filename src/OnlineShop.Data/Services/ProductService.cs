using OnlineShop.Data.Interfaces;

namespace OnlineShop.Data.Services;

internal class ProductService : IProductService
{
    private readonly IDatabaseConnection _connection;

    public ProductService(IDatabaseConnection connection)
    {
        _connection = connection;
    }
}
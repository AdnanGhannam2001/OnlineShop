using NanoidDotNet;
using OnlineShop.Data.Constants;

namespace OnlineShop.Data.Models;

public class Order
{
    #pragma warning disable CS8618
    public Order() { }
    #pragma warning restore CS8618

    private readonly List<OrderProduct> _products = [];

    public Order(string userId)
    {
        Id = Nanoid.Generate(size: DbConstants.NanoIdSize);
        UserId = userId;
        OrderedAt = DateTime.UtcNow;
    }

    public string Id { get; init; }

    public string UserId { get; init; }
    public AppUser? User { get; private set; }

    public DateTime OrderedAt { get; init; }

    public IReadOnlyCollection<OrderProduct> Products => _products;

    public void SetUser(AppUser user) => User = user;
    public void AddProducts(IEnumerable<OrderProduct> products) => _products.AddRange(products);
}

using NanoidDotNet;
using OnlineShop.Data.Constants;

namespace OnlineShop.Data.Models;

public class Category
{
    #pragma warning disable CS8618
    public Category() { }
    #pragma warning restore CS8618

    private readonly List<Product> _products;

    public Category(string label)
    {
        Id = Nanoid.Generate(size: DbConstants.NanoIdSize);
        Label = label;
        CreatedAt = DateTime.UtcNow;
        _products = [];
    }

    public string Id { get; init; }
    public string Label { get; private set; }
    public IReadOnlyCollection<Product> Products => _products;
    public DateTime CreatedAt { get; init; }
}
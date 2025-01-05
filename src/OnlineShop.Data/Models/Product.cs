using NanoidDotNet;
using OnlineShop.Data.Constants;

namespace OnlineShop.Data.Models;

public class Product
{
    #pragma warning disable CS8618
    public Product() { }
    #pragma warning restore CS8618

    public Product(string name,
        int quantity,
        decimal cost,
        short discount,
        string details,
        string categoryId,
        Category category)
    {
        Id = Nanoid.Generate(size: DbConstants.NanoIdSize);
        Name = name;
        Quantity = quantity;
        Cost = cost;
        Discount = discount;
        Details = details;
        CategoryId = categoryId;
        Category = category;
        CreatedAt = DateTime.UtcNow;
    }

    public string Id { get; init; }
    public string Name { get; private set; }
    public int Quantity { get; private set; }
    public decimal Cost { get; private set; }
    public short Discount { get; private set; }
    public string Details { get; private set; }
    public string CategoryId { get; private set; }
    public Category? Category { get; private set; }
    public DateTime CreatedAt { get; init; }

    public void SetCategory(Category? category)
    {
        Category = category;
    }
}
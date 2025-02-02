namespace OnlineShop.Data.Models;

public class UserProduct
{
    #pragma warning disable CS8618
    public UserProduct() { }
    #pragma warning restore CS8618

    public UserProduct(string userId, string productId, int quantity = 1)
    {
        UserId = userId;
        ProductId = productId;
        Quantity = quantity;
        AddedAt = DateTime.UtcNow;
    }

    public string UserId { get; init; }
    public AppUser? User { get; private set; }
    public string ProductId { get; init; }
    public Product? Product { get; private set; }

    public int Quantity { get; private set; }
    public DateTime AddedAt { get; init; }

    public void SetProduct(Product product) => Product = product;
    public void SetQuantity(int quantity) => Quantity = quantity;
}

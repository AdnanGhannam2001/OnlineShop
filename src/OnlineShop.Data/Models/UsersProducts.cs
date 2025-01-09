namespace OnlineShop.Data.Models;

public class UsersProducts
{
    #pragma warning disable CS8618
    public UsersProducts() { }
    #pragma warning restore CS8618

    public UsersProducts(string userId, string productId)
    {
        UserId = userId;
        ProductId = productId;
        AddedAt = DateTime.UtcNow;
    }

    public string UserId { get; init; }
    public AppUser User { get; private set; }
    public string ProductId { get; init; }
    public Product Product { get; private set; }
    public DateTime AddedAt { get; init; }

    public void SetProduct(Product product)
    {
        Product = product;
    }
}

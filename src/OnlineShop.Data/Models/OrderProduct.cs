namespace OnlineShop.Data.Models;

public class OrderProduct
{
    #pragma warning disable CS8618
    public OrderProduct() { }
    #pragma warning restore CS8618

    public OrderProduct(string orderId, string productId, int quantity = 1)
    {
        OrderId = orderId;
        ProductId = productId;
        Quantity = quantity;
        AddedAt = DateTime.UtcNow;
    }

    public OrderProduct(string orderId, UserProduct userProduct)
        : this(orderId, userProduct.ProductId, userProduct.Quantity)
    { }

    public string OrderId { get; init; }
    public Order? Order { get; private set; }
    public string ProductId { get; init; }
    public Product? Product { get; private set; }

    public int Quantity { get; private set; }
    public DateTime AddedAt { get; init; }

    public void SetProduct(Product product) => Product = product;
}

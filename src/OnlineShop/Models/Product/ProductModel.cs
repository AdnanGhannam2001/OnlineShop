using OnlineShop.Data.Common;

namespace OnlineShop.Models.Product;

public record ProductModel(Data.Models.Product Product, bool? InCart = null);

using OnlineShop.Data.Common;
using OnlineShop.Data.Models;

namespace OnlineShop.Models.Product;

public record OrdersModel(int PageNumber,
    Page<Order> Page);

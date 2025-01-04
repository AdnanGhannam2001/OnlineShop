using OnlineShop.Data.Common;
using OnlineShop.Data.Models;

namespace OnlineShop.Models.Home;

public record IndexModel(int PageNumber,
    Page<Product> Page,
    IEnumerable<Category> Categories,
    string? Selected,
    Range MinMax);

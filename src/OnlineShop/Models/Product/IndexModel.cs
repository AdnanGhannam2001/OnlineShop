using OnlineShop.Data.Common;
using OnlineShop.Data.Models;

namespace OnlineShop.Models.Product;

public record IndexModel(int PageNumber,
    Page<Data.Models.Product> Page,
    IEnumerable<Category> Categories,
    string? Selected,
    Range MinMax);

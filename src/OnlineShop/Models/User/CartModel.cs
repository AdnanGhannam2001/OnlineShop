using OnlineShop.Data.Common;
using OnlineShop.Data.Models;

namespace OnlineShop.Models.User;

public record CartModel(int PageNumber,
    Page<UserProduct> Page);

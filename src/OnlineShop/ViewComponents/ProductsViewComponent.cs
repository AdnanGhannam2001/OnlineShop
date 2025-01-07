using Microsoft.AspNetCore.Mvc;
using OnlineShop.Data.Models;

namespace OnlineShop.ViewComponents;

public class ProductsViewComponent : ViewComponent
{
    internal const string Name = "Products";

    public IViewComponentResult Invoke(IEnumerable<Product> products)
    {
        return View(products);
    }
}

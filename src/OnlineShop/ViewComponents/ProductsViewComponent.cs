using Microsoft.AspNetCore.Mvc;
using OnlineShop.Data.Models;

namespace OnlineShop.ViewComponents;

public class ProductsViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(IEnumerable<Product> products)
    {
        return View(products);
    }
}

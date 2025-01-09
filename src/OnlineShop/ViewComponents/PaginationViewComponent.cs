using Microsoft.AspNetCore.Mvc;
using OnlineShop.Models.Components;

namespace OnlineShop.ViewComponents;

public class PaginationViewComponent : ViewComponent
{
    internal const string Name = "Pagination";

    public IViewComponentResult Invoke(PaginationModel model)
    {
        return View(model);
    }
}
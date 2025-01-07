using Microsoft.AspNetCore.Mvc;
using OnlineShop.Models.Components;

namespace OnlineShop.ViewComponents;

public class ToastViewComponent : ViewComponent
{
    internal const string Name = "Toast";

    public IViewComponentResult Invoke(ToastModel model)
    {
        return View(model: model);
    }
}

using Microsoft.AspNetCore.Mvc;
using OnlineShop.Data.Common;
using OnlineShop.Data.Interfaces;
using OnlineShop.Models.Home;

namespace OnlineShop.Controllers;

public class ProductsController : Controller
{
    public const string Name = "Products";

    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    public async Task<IActionResult> Index([FromQuery] int pageNumber = 0,
        [FromQuery] string? categoryLabel = null,
        [FromQuery] int min = 0,
        [FromQuery] int max = 10000)
    {
        var model = new IndexModel(pageNumber,
            await _productService.GetProducts(new PageRequest(10, pageNumber), new Range(min, max), categoryLabel),
            await _productService.GetCategories(),
            categoryLabel,
            new Range(min, max));
        
        return View(model);
    }

    public IActionResult Privacy()
    {
        return View();
    }
}

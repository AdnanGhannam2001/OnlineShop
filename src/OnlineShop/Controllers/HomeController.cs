using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.Data.Common;
using OnlineShop.Data.Interfaces;
using OnlineShop.Models;
using OnlineShop.Models.Home;

namespace OnlineShop.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IProductService _productService;

    public HomeController(ILogger<HomeController> logger,
        IProductService productService)
    {
        _logger = logger;
        _productService = productService;
    }

    public async Task<IActionResult> Index([FromQuery] int pageNumber = 0,
        [FromQuery] string? categoryLabel = null,
        [FromQuery] int min = 0,
        [FromQuery] int max = 10000)
    {
        var model = new IndexModel(pageNumber,
            await _productService.GetProducts(new PageRequest(10, pageNumber), categoryLabel),
            await _productService.GetCategories(),
            categoryLabel,
            new Range(min, max));
        
        return View(model);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

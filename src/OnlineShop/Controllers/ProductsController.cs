using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.Data.Common;
using OnlineShop.Data.Interfaces;
using OnlineShop.Models.Product;

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
            await _productService.GetProductsAsync(new PageRequest(10, pageNumber), new Range(min, max), categoryLabel),
            await _productService.GetCategoriesAsync(),
            categoryLabel,
            new Range(min, max));
        
        return View(model);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Product([FromRoute] string id)
    {
        var product = await _productService.GetProductByIdAsync(id);
        bool? inCart = null;

        if (User.Identity?.IsAuthenticated == true)
        {
            inCart = await _productService.ProductInCartAsync(id, GetUserId());
        }

        return product is not null
            ? View(new ProductModel(product, inCart))
            : RedirectToAction(nameof(ErrorsController.NotFound),
                ErrorsController.Name,
                new { message = "Product was not found" });
    }

    [Authorize]
    public async Task<IActionResult> AddToCart([FromQuery] string id)
    {
        await _productService.AddToCartAsync(id, GetUserId());
        return RedirectToAction(nameof(this.Product), new { Id = id });
    }

    [Authorize]
    public async Task<IActionResult> RemoveFromCart([FromQuery] string id)
    {
        await _productService.RemoveFromCartAsync(id, GetUserId());
        return RedirectToAction(nameof(this.Product), new { Id = id });
    }

    private string GetUserId() => User.Claims.First(x => x.Type == "id").Value;
}

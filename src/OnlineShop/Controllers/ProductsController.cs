using Microsoft.AspNetCore.Mvc;
using OnlineShop.Data.Common;
using OnlineShop.Data.Interfaces;
using OnlineShop.Extensions;
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
        var productsResult = await _productService.GetProductsAsync(new PageRequest(10, pageNumber),
            new Range(min, max),
            categoryLabel);

        if (!productsResult.IsSuccess)
        {

        }

        var categoriesResult = await _productService.GetCategoriesAsync();

        if (!categoriesResult.IsSuccess)
        {

        }

        var model = new IndexModel(pageNumber,
            productsResult.Value,
            categoriesResult.Value,
            categoryLabel,
            new Range(min, max));
        
        return View(model);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Product([FromRoute] string id)
    {
        var productResult = await _productService.GetProductByIdAsync(id);
        bool? inCart = null;

        if (User.Identity?.IsAuthenticated == true)
        {
            inCart = (await _productService.ProductInCartAsync(id, this.GetUserId())).Value;
        }

        return productResult.IsSuccess
            ? View(new ProductModel(productResult.Value, inCart))
            : RedirectToAction(nameof(ErrorsController.NotFound),
                ErrorsController.Name,
                new { message = productResult.Exceptions.First().Message });
    }

    public async Task<IActionResult> Orders([FromQuery] int pageNumber = 0)
    {
        var pageResult = await _productService.GetOrdersAsync(this.GetUserId());
        var model = new OrdersModel(pageNumber, pageResult.Value);
        return View(model);
    }

    [HttpGet("[action]/{id}")]
    public async Task<IActionResult> Order([FromRoute] string id)
    {
        var model = await _productService.GetOrderByIdAsync(id, this.GetUserId());
        if (model is null)
        {
            return RedirectToAction(nameof(ErrorsController.NotFound),
                ErrorsController.Name,
                new { message = "Product was not found" });
        }
        
        return View(model);
    }

    public async Task<IActionResult> SubmitOrder()
    {
        await _productService.OrderProductsAsync(this.GetUserId()); // TODO Handle false
        return RedirectToAction(nameof(Orders));
    }
}

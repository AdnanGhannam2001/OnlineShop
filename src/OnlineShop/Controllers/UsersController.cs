using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.Data.Common;
using OnlineShop.Data.Enums;
using OnlineShop.Data.Interfaces;
using OnlineShop.Data.Models;
using OnlineShop.Extensions;
using OnlineShop.Models.User;

namespace OnlineShop.Controllers;

public sealed class UsersController : Controller
{
    public const string Name = "Users";

    private readonly IAppUserService _service;
    private readonly IProductService _productService;

    public UsersController(IAppUserService service,
        IProductService productService)
    {
        _service = service;
        _productService = productService;
    }

    [HttpGet]
    public IActionResult Register()
    {
        if (HttpContext.User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction(nameof(ProductsController.Index), ProductsController.Name);
        }

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        if (model.Password != model.ConfirmPassword)
        {
            ModelState.AddModelError(nameof(model.ConfirmPassword), "Passwords don't match");
            return View(model);
        }

        var user = new AppUser(model.Username, AppUserRole.NormalUser, model.Password);
        if (!await _service.CreateUserAsync(user))
        {
            ModelState.AddModelError(nameof(model.Username), "Username already exists");
            return View(model);
        }

        await SignInAsync(user.Id, user.Username);
        return RedirectToAction(nameof(ProductsController.Index), ProductsController.Name);
    }

    [HttpGet]
    public IActionResult Login()
    {
        if (HttpContext.User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction(nameof(ProductsController.Index), ProductsController.Name);
        }

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _service.GetUserByNameAsync(model.Username);
        if (user is null)
        {
            ModelState.AddModelError(nameof(model.Username), $"User '{model.Username}' is not found");
            return View(model);
        }

        if (!user.PasswordIsCorrect(model.Password))
        {
            ModelState.AddModelError(nameof(model.Password), "Password is wrong");
            return View(model);
        }

        await SignInAsync(user.Id, user.Username, model.Keep);
        return RedirectToAction(nameof(ProductsController.Index), ProductsController.Name);
    }

    [Authorize]
    public async Task<IActionResult> Signout()
    {
        await HttpContext.SignOutAsync();
        return RedirectToAction(nameof(ProductsController.Index), ProductsController.Name);
    }

    [Authorize]
    public async Task<IActionResult> Cart([FromQuery] int pageNumber = 0)
    {
        var page = await _service.GetUsersProductsAsync(this.GetUserId(), new PageRequest(10, pageNumber));
        var model = new CartModel(pageNumber, page);
        return View(model);
    }

    [Authorize]
    public async Task<IActionResult> AddToCart([FromForm] string id,
        [FromForm] int quantity)
    {
        var item = new UserProduct(this.GetUserId(), id, quantity);

        var product = await _productService.GetProductByIdAsync(id);

        if (product is null || quantity < 0 || product.Quantity < quantity)
        {
            return RedirectToAction(nameof(ErrorsController.Invalid), ErrorsController.Name, new { message = "Invalid Input" });
        }

        await _service.AddToCartAsync(item);
        return RedirectToAction(nameof(ProductsController.Product), ProductsController.Name, new { Id = id });
    }

    [Authorize]
    public async Task<IActionResult> RemoveFromCart([FromQuery] string id,
        [FromQuery] string redirectController = ProductsController.Name,
        [FromQuery] string redirectAction = nameof(ProductsController.Product))
    {
        await _service.RemoveFromCartAsync(id, this.GetUserId());
        return RedirectToAction(redirectAction, redirectController, new { Id = id });
    }

    private Task SignInAsync(string id, string username, bool keep = false)
    {
        var idClaim = new Claim("id", id);
        var usernameClaim = new Claim("username", username);
        var identity = new ClaimsIdentity([idClaim, usernameClaim], CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);
        return HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            new AuthenticationProperties()
            {
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(1),
                IsPersistent = keep,
                AllowRefresh = true
            });
    }
}

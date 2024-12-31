using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.Data.Enums;
using OnlineShop.Data.Interfaces;
using OnlineShop.Data.Models;
using OnlineShop.Models.User;

namespace OnlineShop.Controllers;

public sealed class UsersController : Controller
{
    private readonly IAppUserService _service;

    public UsersController(IAppUserService service)
    {
        _service = service;
    }

    [HttpGet]
    public IActionResult Register()
    {
        if (HttpContext.User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction(nameof(HomeController.Index), "Home");
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
        return RedirectToAction(nameof(HomeController.Index), "Home");
    }

    [HttpGet]
    public IActionResult Login()
    {
        if (HttpContext.User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction(nameof(HomeController.Index), "Home");
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
        return RedirectToAction(nameof(HomeController.Index), "Home");
    }

    [Authorize]
    public async Task<IActionResult> Signout()
    {
        await HttpContext.SignOutAsync();
        return RedirectToAction(nameof(HomeController.Index), "Home");
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

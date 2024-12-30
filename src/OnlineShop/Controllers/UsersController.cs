using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Dapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.Data.SqlModels;
using OnlineShop.Interfaces;
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
    public IActionResult Login()
    {
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

        if (!PasswordIsCorrect(model.Password, user.PasswordHash))
        {
            ModelState.AddModelError(nameof(model.Password), "Password is wrong");
            return View(model);
        }

        await SignInAsync(user.Id, model.Keep);
        return RedirectToAction(nameof(HomeController.Index), "Home");
    }

    private Task SignInAsync(string id, bool keep)
    {
        var claim = new Claim("id", id);
        var identity = new ClaimsIdentity([claim], CookieAuthenticationDefaults.AuthenticationScheme);
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

    private static bool PasswordIsCorrect(string password, string hash)
    {
        return hash.Equals(
            Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(password))),
            StringComparison.OrdinalIgnoreCase);
    }
}

using System.Security.Claims;
using Dapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.Interfaces;
using OnlineShop.Models.User;

namespace OnlineShop.Controllers;

public sealed class UsersController : Controller
{
    private readonly IDatabaseConnection _connection;

    public UsersController(IDatabaseConnection connection)
    {
        _connection = connection;
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

        // TODO
        await SignInAsync("...");
        return RedirectToAction(nameof(HomeController.Index), "Home");
    }

    private Task SignInAsync(string id)
    {
        var claim = new Claim("id", id);
        var identity = new ClaimsIdentity([claim], CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);
        return HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
    }
}

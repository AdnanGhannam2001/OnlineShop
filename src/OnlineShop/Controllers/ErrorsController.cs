using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.Models;

namespace OnlineShop.Controllers;

public class ErrorsController : Controller
{
    public const string Name = "Errors";

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Index()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    public IActionResult NotFound([FromQuery] string? message)
    {
        return View(model: message);
    }

    public IActionResult UnAuthorized()
    {
        return View();
    }
}
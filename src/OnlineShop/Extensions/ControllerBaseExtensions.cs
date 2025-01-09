using Microsoft.AspNetCore.Mvc;

namespace OnlineShop.Extensions;

internal static class ControllerBaseExtensions
{
    public static string GetUserId(this ControllerBase controller)
        => controller.User.Claims.First(x => x.Type == "id").Value;
}
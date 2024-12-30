using OnlineShop.Data.Enums;

namespace OnlineShop.Data.Models;

public class AppUser
{
    public string Id { get; init; }
    public string Username { get; set; }
    public AppUserRole Role { get; set; }
    public string PasswordHash { get; set; }
    public DateTime CreatedAt { get; init; }

    public override string ToString()
    {
        return $"AppUser: {Username}";
    }
}

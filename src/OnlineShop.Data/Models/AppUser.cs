using System.Security.Cryptography;
using System.Text;
using NanoidDotNet;
using OnlineShop.Data.Constants;
using OnlineShop.Data.Enums;

namespace OnlineShop.Data.Models;

public class AppUser
{
    #pragma warning disable CS8618
    public AppUser() { }
    #pragma warning restore CS8618

    public AppUser(string username, AppUserRole role, string password)
    {
        Id = Nanoid.Generate(size: DbConstants.NanoIdSize);
        Username = username;
        Role = role;
        PasswordHash = HashPassword(password);
        CreatedAt = DateTime.UtcNow;
    }

    public string Id { get; init; }
    public string Username { get; private set; }
    public AppUserRole Role { get; private set; }
    public string PasswordHash { get; private set; }
    public DateTime CreatedAt { get; init; }

    public override string ToString()
    {
        return $"AppUser: {Id} - {Username} - {Enum.GetName(Role)}";
    }

    public bool PasswordIsCorrect(string password)
    {
        return PasswordHash.Equals(
            Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(password))),
            StringComparison.OrdinalIgnoreCase);
    }

    private static string HashPassword(string password)
    {
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(password)));
    }
}

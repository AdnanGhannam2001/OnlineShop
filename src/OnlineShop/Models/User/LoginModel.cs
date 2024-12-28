using System.ComponentModel.DataAnnotations;

namespace OnlineShop.Models.User;

public record LoginModel([Required] string Username,
    [Required] string Password,
    bool Keep = false);

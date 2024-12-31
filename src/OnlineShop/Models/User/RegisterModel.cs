using System.ComponentModel.DataAnnotations;

namespace OnlineShop.Models.User;

public record RegisterModel(
    [Required,
        StringLength(50, ErrorMessage = "Must be between 5 and 50 characters", MinimumLength = 5)]
    string Username,
    [Required,
        DataType(DataType.Password),
        StringLength(255, ErrorMessage = "Must be between 5 and 255 characters", MinimumLength = 5)]
    string Password,
    [StringLength(255, ErrorMessage = "Must be between 5 and 255 characters", MinimumLength = 5)]
    string ConfirmPassword);

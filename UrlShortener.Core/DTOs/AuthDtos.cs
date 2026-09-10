using System.ComponentModel.DataAnnotations;

namespace UrlShortener.Core.DTOs;

public record LoginRequest(
    [Required(ErrorMessage = "Login is required.")]
    string Login,

    [Required(ErrorMessage = "Password is required.")]
    string Password
);

public record UserDto(
    int Id,
    string Login,
    string Role
);

using System.ComponentModel.DataAnnotations;

namespace Day1.DTOs;

// Represents the credentials submitted during login.
public class LoginRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    public string Password { get; set; } = string.Empty;
}
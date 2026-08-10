using System.ComponentModel.DataAnnotations;

namespace Day1.DTOs;

public class UpdateOrderRequest
{
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "CustomerId must be greater than 0.")]
    public int CustomerId { get; set; }
}
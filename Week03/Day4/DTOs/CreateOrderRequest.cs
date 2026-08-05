using System.ComponentModel.DataAnnotations;

namespace Day4.DTOs;

public class CreateOrderRequest
{
    [Required]
    [Range(1, int.MaxValue)]
    public int CustomerId { get; set; }
}
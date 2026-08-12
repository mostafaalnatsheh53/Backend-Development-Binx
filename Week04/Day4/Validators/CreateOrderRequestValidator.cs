using Day1.DTOs;
using FluentValidation;

namespace Day1.Validators;

public class CreateOrderRequestValidator : AbstractValidator<CreateOrderRequest>
{
    public CreateOrderRequestValidator()
    {
        RuleFor(x => x.CustomerId)
            .GreaterThan(0)
            .WithMessage("CustomerId must be greater than 0.");

        RuleFor(x => x.Total)
            .GreaterThan(0)
            .WithMessage("Order total must be greater than 0.");

        RuleFor(x => x.OrderDate)
            .LessThanOrEqualTo(DateTime.UtcNow)
            .WithMessage("Order date cannot be in the future.");
    }
}
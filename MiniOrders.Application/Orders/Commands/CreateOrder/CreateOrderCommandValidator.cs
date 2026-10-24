using FluentValidation;

namespace MiniOrders.Application.Orders.Commands.CreateOrder;
public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(x => x.CustomerId).NotEmpty().WithMessage("Customer id is required.");
        RuleFor(x => x.OrderDate).NotEmpty().WithMessage("Order date is required.");
        RuleFor(x => x.TotalAmount).GreaterThan(0).WithMessage("Order total must be greater than zero.");
    }
}

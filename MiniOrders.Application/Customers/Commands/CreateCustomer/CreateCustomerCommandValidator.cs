using FluentValidation;

namespace MiniOrders.Application.Customers.Commands.CreateCustomer;
public class CreateCustomerCommandValidator : AbstractValidator<CreateCustomerCommand>
{
    public CreateCustomerCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.").MaximumLength(100);
        RuleFor(x => x.Address)
            .NotEmpty().MaximumLength(200);
        RuleFor(x => x.PhoneNumber)
            .NotEmpty().Matches(@"^\+?[0-9\s\-]{7,15}$").WithMessage("Phone number must contain 7 to 15 digits and may start with '+'.");
    }
}

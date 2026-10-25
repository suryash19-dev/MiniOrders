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
            .NotEmpty().WithMessage("Phone number is required.").MaximumLength(20);
    }
}

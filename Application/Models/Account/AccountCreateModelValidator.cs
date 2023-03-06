using FluentValidation;

namespace Application.Models.Account;

public class CreateAccountModelValidator : AbstractValidator<AccountCreateModel>
{
    public CreateAccountModelValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty();
        RuleFor(x => x.LastName).NotEmpty();
        RuleFor(x => x.Password).NotEmpty();
        RuleFor(x => x.Email).EmailAddress();
    }
}
using FluentValidation;

namespace Application.Models.Account;

public class AccountCreateModelValidator : AbstractValidator<AccountCreateModel>
{
    public AccountCreateModelValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty();
        RuleFor(x => x.LastName).NotEmpty();
        RuleFor(x => x.Password).NotEmpty();
        RuleFor(x => x.Email).EmailAddress();
    }
}
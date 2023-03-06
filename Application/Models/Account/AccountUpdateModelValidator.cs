using FluentValidation;

namespace Application.Models.Account;

public class AccountUpdateModelValidator : AbstractValidator<AccountUpdateModel>
{
    public AccountUpdateModelValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty();
        RuleFor(x => x.LastName).NotEmpty();
        RuleFor(x => x.Email).EmailAddress();
    }
}
using Application.Entities;
using FluentValidation;

namespace Application.Models.Account;

public class AccountCreateModelValidator : AbstractValidator<AccountCreateModel>
{
    public AccountCreateModelValidator()
    {
        Include(new AccountRegistrationModelValidator());
        RuleFor(x => x.Role)
            .Must(x => x == default || Enum.IsDefined(typeof(AccountRole), x));
    }
}
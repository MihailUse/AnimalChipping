using FluentValidation;

namespace Application.Models.Account;

public class AccountSearchModelValidator : AbstractValidator<AccountSearchModel>
{
    public AccountSearchModelValidator()
    {
        RuleFor(x => x.From).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Size).GreaterThan(0);
    }
}
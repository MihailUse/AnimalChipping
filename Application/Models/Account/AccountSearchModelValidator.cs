using Application.Models.Common;
using FluentValidation;

namespace Application.Models.Account;

public class AccountSearchModelValidator : AbstractValidator<AccountSearchModel>
{
    public AccountSearchModelValidator()
    {
        Include(new ListModelValidator());
    }
}
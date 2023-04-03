using Domain.Entities;
using FluentValidation;

namespace Application.Models.Account;

public class AccountUpdateModelValidator : AbstractValidator<AccountUpdateModel>
{
    public AccountUpdateModelValidator()
    {
        Include(new AccountCreateModelValidator());
    }
}
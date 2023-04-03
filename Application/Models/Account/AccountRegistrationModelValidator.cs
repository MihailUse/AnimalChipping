using Domain.Entities;
using FluentValidation;

namespace Application.Models.Account;

public class AccountRegistrationModelValidator : AbstractValidator<AccountRegistrationModel>
{
    public AccountRegistrationModelValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty();
        RuleFor(x => x.LastName).NotEmpty();
        RuleFor(x => x.Password).NotEmpty();
        RuleFor(x => x.Email).EmailAddress();
    }
}
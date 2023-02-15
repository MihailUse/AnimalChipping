using FluentValidation;

namespace Application.Models.Account;

public class CreateAccountModelValidator : AbstractValidator<CreateAccountModel>
{
    public CreateAccountModelValidator()
    {
        RuleFor(x => x.Email).EmailAddress();
    }
}
using Application.Models.Animal;
using FluentValidation;
using AnimalGender = Domain.Entities.Animal.AnimalGender;
using AnimalLifeStatus = Domain.Entities.Animal.AnimalLifeStatus;

namespace Application.Models.Account;

public class AnimalUpdateModelValidator : AbstractValidator<AnimalUpdateModel>
{
    public AnimalUpdateModelValidator()
    {
        RuleFor(x => x.Height)
            .GreaterThan(0);
        RuleFor(x => x.Length)
            .GreaterThan(0);
        RuleFor(x => x.Weight)
            .GreaterThan(0);
        RuleFor(x => x.ChipperId)
            .GreaterThan(0);
        RuleFor(x => x.ChippingLocationId)
            .GreaterThan(0);
        RuleFor(x => x.Gender)
            .Must(x => Enum.IsDefined(typeof(AnimalGender), x));
        RuleFor(x => x.LifeStatus)
            .Must(x => Enum.IsDefined(typeof(AnimalLifeStatus), x));
    }
}
using Application.Models.Common;
using FluentValidation;
using AnimalGender = Domain.Entities.Animal.AnimalGender;
using AnimalLifeStatus = Domain.Entities.Animal.AnimalLifeStatus;

namespace Application.Models.Animal;

public class AnimalSearchModelValidator : AbstractValidator<AnimalSearchModel>
{
    public AnimalSearchModelValidator()
    {
        Include(new ListModelValidator());
        RuleFor(x => x.ChipperId)
            .GreaterThan(0);
        RuleFor(x => x.ChippingLocationId)
            .GreaterThan(0);
        RuleFor(x => x.Gender)
            .Must(x => x == default || Enum.IsDefined(typeof(AnimalGender), x));
        RuleFor(x => x.LifeStatus)
            .Must(x => x == default || Enum.IsDefined(typeof(AnimalLifeStatus), x));
    }
}
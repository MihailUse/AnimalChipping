using Application.Entities;
using FluentValidation;

namespace Application.Models.Animal;

public class AnimalCreateModelValidator : AbstractValidator<AnimalCreateModel>
{
    public AnimalCreateModelValidator()
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
        RuleFor(x => x.AnimalTypes)
            .Must(x => x.Any(typeId => typeId > 0));
        RuleFor(x => x.Gender)
            .Must(x => x != null && Enum.IsDefined(typeof(AnimalGender), x));
    }
}
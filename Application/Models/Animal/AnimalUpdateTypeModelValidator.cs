using FluentValidation;

namespace Application.Models.Animal;

public class AnimalUpdateTypeModelValidator : AbstractValidator<AnimalUpdateTypeModel>
{
    public AnimalUpdateTypeModelValidator()
    {
        RuleFor(x => x.OldTypeId)
            .GreaterThan(0);
        RuleFor(x => x.NewTypeId)
            .GreaterThan(0);
    }
}
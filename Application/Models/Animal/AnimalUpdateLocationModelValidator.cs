using FluentValidation;

namespace Application.Models.Animal;

public class AnimalUpdateLocationModelValidator : AbstractValidator<AnimalUpdateLocationModel>
{
    public AnimalUpdateLocationModelValidator()
    {
        RuleFor(x => x.LocationPointId)
            .GreaterThan(0);
        RuleFor(x => x.VisitedLocationPointId)
            .GreaterThan(0);
    }
}
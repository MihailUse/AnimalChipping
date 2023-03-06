using FluentValidation;

namespace Application.Models.AnimalType;

public class AnimalTypeUpdateModelValidator : AbstractValidator<AnimalTypeUpdateModel>
{
    public AnimalTypeUpdateModelValidator()
    {
        RuleFor(x => x.Type).NotEmpty();
    }
}
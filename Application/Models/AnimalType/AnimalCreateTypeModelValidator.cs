using FluentValidation;

namespace Application.Models.AnimalType;

public class AnimalTypeCreateModelValidator : AbstractValidator<AnimalCreateTypeModel>
{
    public AnimalTypeCreateModelValidator()
    {
        RuleFor(x => x.Type).NotEmpty();
    }
}
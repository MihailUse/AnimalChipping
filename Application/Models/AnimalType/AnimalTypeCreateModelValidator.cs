using FluentValidation;

namespace Application.Models.AnimalType;

public class AnimalTypeCreateModelValidator : AbstractValidator<AnimalTypeCreateModel>
{
    public AnimalTypeCreateModelValidator()
    {
        RuleFor(x => x.Type).NotEmpty();
    }
}
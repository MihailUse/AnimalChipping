using Application.Models.Common;
using FluentValidation;

namespace Application.Models.Animal;

public class AnimalSearchLocationModelValidator : AbstractValidator<AnimalSearchLocationModel>
{
    public AnimalSearchLocationModelValidator()
    {
        Include(new ListModelValidator());
    }
}
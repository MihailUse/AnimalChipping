using FluentValidation;

namespace Application.Models.Common;

public class ListModelValidator : AbstractValidator<ListModel>
{
    public ListModelValidator()
    {
        RuleFor(x => x.From).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Size).GreaterThan(0);
    }
}
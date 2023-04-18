using FluentValidation;

namespace Application.Models.Area;

public class AnalyticRequestModelValidator : AbstractValidator<GetAnalyticModel>
{
    public AnalyticRequestModelValidator()
    {
        RuleFor(x => x.StartDate)
            .Must((m, x) => x < m.EndDate);
    }
}
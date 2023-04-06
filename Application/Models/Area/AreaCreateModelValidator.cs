using Application.Models.Location;
using FluentValidation;

namespace Application.Models.Area;

public class AreaCreateModelValidator : AbstractValidator<AreaCreateModel>
{
    public AreaCreateModelValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.AreaPoints)
            .Must(x => x.Count >= 3);

        RuleFor(x => x.AreaPoints)
            .Must(x => x.DistinctBy(d => d.Latitude).Count() > 1 && x.DistinctBy(d => d.Longitude).Count() > 1);
        RuleForEach(x => x.AreaPoints)
            .ChildRules(point =>
            {
                point.RuleFor(x => x.Latitude)
                    .LessThanOrEqualTo(90)
                    .GreaterThanOrEqualTo(-90);

                point.RuleFor(x => x.Longitude)
                    .LessThanOrEqualTo(180)
                    .GreaterThanOrEqualTo(-180);
            });
    }
}
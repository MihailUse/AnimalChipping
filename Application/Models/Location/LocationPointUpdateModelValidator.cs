using FluentValidation;

namespace Application.Models.Location;

public class LocationPointUpdateModelValidator : AbstractValidator<LocationPointUpdateModel>
{
    public LocationPointUpdateModelValidator()
    {
        RuleFor(x => x.Latitude)
            .LessThan(90)
            .GreaterThan(-90);

        RuleFor(x => x.Longitude)
            .LessThan(180)
            .GreaterThan(-180);
    }
}
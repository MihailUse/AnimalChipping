using FluentValidation;

namespace Application.Models.Location;

public class LocationPointCreateModelValidator : AbstractValidator<LocationPointCreateModel>
{
    public LocationPointCreateModelValidator()
    {
        RuleFor(x => x.Latitude)
            .LessThan(90)
            .GreaterThan(-90);

        RuleFor(x => x.Longitude)
            .LessThan(180)
            .GreaterThan(-180);
    }
}
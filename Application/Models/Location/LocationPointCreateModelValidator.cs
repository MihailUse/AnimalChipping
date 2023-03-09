using FluentValidation;

namespace Application.Models.Location;

public class LocationPointCreateModelValidator : AbstractValidator<LocationPointCreateModel>
{
    public LocationPointCreateModelValidator()
    {
        RuleFor(x => x.Latitude)
            .LessThanOrEqualTo(90)
            .GreaterThanOrEqualTo(-90);

        RuleFor(x => x.Longitude)
            .LessThanOrEqualTo(180)
            .GreaterThanOrEqualTo(-180);
    }
}
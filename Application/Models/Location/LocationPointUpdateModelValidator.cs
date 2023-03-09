using FluentValidation;

namespace Application.Models.Location;

public class LocationPointUpdateModelValidator : AbstractValidator<LocationPointUpdateModel>
{
    public LocationPointUpdateModelValidator()
    {
        Include(new LocationPointCreateModelValidator());
    }
}
using FluentValidation;

namespace Application.Models.Area;

public class AreaUpdateModelValidator : AbstractValidator<AreaUpdateModel>
{
    public AreaUpdateModelValidator()
    {
        Include(new AreaCreateModelValidator());
    }
}
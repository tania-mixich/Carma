using Carma.Application.DTOs.Location;
using FluentValidation;

namespace Carma.Application.Validators.Location;

public class LocationValidator : AbstractValidator<LocationCreateDto>
{
    public LocationValidator()
    {
        RuleFor(l => l.Latitude).NotEmpty().WithMessage("Latitude is required")
            .GreaterThanOrEqualTo(-90).WithMessage("Latitude must be greater than or equal to -90")
            .LessThanOrEqualTo(90).WithMessage("Latitude must be less than or equal to 90");
        RuleFor(l => l.Longitude).NotEmpty().WithMessage("Longitude is required")
            .GreaterThanOrEqualTo(-180).WithMessage("Longitude must be greater than or equal to -180")
            .LessThanOrEqualTo(180).WithMessage("Longitude must be less than or equal to 180");
    }
}
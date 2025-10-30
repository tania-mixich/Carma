using Carma.Application.DTOs.Location;
using Carma.Application.DTOs.Ride;
using FluentValidation;

namespace Carma.Application.Validators.Ride;

public class RideUpdateValidator : AbstractValidator<RideUpdateDto>
{
    public RideUpdateValidator(IValidator<LocationCreateDto> locationCreateValidator)
    {
        RuleFor(r => r.PickupLocation)
            .SetValidator(locationCreateValidator);
        RuleFor(r => r.DropOffLocation)
            .SetValidator(locationCreateValidator);
        RuleFor(r => r.PickupTime)
            .GreaterThanOrEqualTo(DateTime.UtcNow).WithMessage("Pickup time must be in the future");
        RuleFor(r => r.Price)
            .GreaterThanOrEqualTo(0).WithMessage("Price must be positive");
        RuleFor(r => r.AvailableSeats)
            .GreaterThanOrEqualTo(1).WithMessage("Available seats must be greater than or equal to 1")
            .LessThanOrEqualTo(6).WithMessage("Available seats must be less than or equal to 6");
    }
}
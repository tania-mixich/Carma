using Carma.Application.DTOs.Location;
using Carma.Application.DTOs.Ride;
using FluentValidation;

namespace Carma.Application.Validators.Ride;

public class RideCreateValidator : AbstractValidator<RideCreateDto>
{
    public RideCreateValidator(IValidator<LocationCreateDto> locationCreateValidator)
    {
        RuleFor(r => r.PickupLocation).NotEmpty().WithMessage("Pickup location is required")
            .SetValidator(locationCreateValidator);
        RuleFor(r => r.DropOffLocation).NotEmpty().WithMessage("Drop off location is required")
            .SetValidator(locationCreateValidator);
        RuleFor(r => r.PickupTime).NotEmpty().WithMessage("Pickup time is required")
            .GreaterThanOrEqualTo(DateTime.UtcNow).WithMessage("Pickup time must be in the future");
        RuleFor(r => r.Price).NotEmpty().WithMessage("Price is required")
            .GreaterThanOrEqualTo(0).WithMessage("Price must be positive");
        RuleFor(r => r.AvailableSeats).NotEmpty().WithMessage("Available seats is required")
            .GreaterThanOrEqualTo(1).WithMessage("Available seats must be greater than or equal to 1")
            .LessThanOrEqualTo(6).WithMessage("Available seats must be less than or equal to 6");
    }
}
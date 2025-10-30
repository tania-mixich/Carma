using Carma.Application.DTOs.Review;
using FluentValidation;

namespace Carma.Application.Validators.Review;

public class ReviewValidator : AbstractValidator<ReviewCreateDto>
{
    public ReviewValidator()
    {
        RuleFor(r => r.RideId).NotEmpty().WithMessage("Ride id is required");
        RuleFor(r => r.Karma).NotEmpty().WithMessage("Karma is required")
            .GreaterThanOrEqualTo(0).WithMessage("Karma must be greater than or equal to 0")
            .LessThanOrEqualTo(10).WithMessage("Karma must be less than or equal to 10");
        RuleFor(r => r.Text).NotEmpty().WithMessage("Text is required")
            .MaximumLength(255).WithMessage("Text must be less than 255 characters")
            .MinimumLength(1).WithMessage("Text must be at least 1 character");
    }
}
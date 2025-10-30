using Carma.Application.DTOs.Location;
using Carma.Application.DTOs.User;
using FluentValidation;

namespace Carma.Application.Validators.User;

public class UserUpdateValidator : AbstractValidator<UserUpdateDto>
{
    public UserUpdateValidator(IValidator<LocationCreateDto?> locationValidator)
    {
        RuleFor(u => u.UserName)
            .MinimumLength(3).WithMessage("Username must be at least 3 characters long")
            .MaximumLength(20).WithMessage("Username must be at most 20 characters long")
            .When(u => !string.IsNullOrEmpty(u.UserName));
        
        RuleFor(u => u.ImageUrl)
            .MaximumLength(255).WithMessage("Image URL must be at most 255 characters long")
            .When(u => !string.IsNullOrEmpty(u.ImageUrl));
        
        RuleFor(u => u.Location)
            .SetValidator(locationValidator)
            .When(u => u.Location != null);
    }
}
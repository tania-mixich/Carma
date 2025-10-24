using Carma.Application.DTOs.Auth;
using FluentValidation;

namespace Carma.Application.Validators.Auth;

public class RegisterValidator : AbstractValidator<RegisterRequestDto>
{
    public RegisterValidator()
    {
        RuleFor(r => r.Email).NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Wrong email format");
        RuleFor(r => r.UserName).NotEmpty().WithMessage("Username is required")
            .MinimumLength(3).WithMessage("Username must be at least 3 characters long")
            .MaximumLength(20).WithMessage("Username must be at most 20 characters long");
        RuleFor(r => r.Password).NotEmpty().WithMessage("Password is required")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long");
        RuleFor(r => r.ConfirmPassword).NotEmpty().WithMessage("Confirm password is required")
            .Equal(r => r.Password).WithMessage("Passwords do not match");
    }
}
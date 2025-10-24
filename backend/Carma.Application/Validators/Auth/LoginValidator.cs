using Carma.Application.DTOs.Auth;
using FluentValidation;

namespace Carma.Application.Validators.Auth;

public class LoginValidator : AbstractValidator<LoginRequestDto>
{
    public LoginValidator()
    {
        RuleFor(r => r.Email).NotEmpty().WithMessage("Email is required");
        RuleFor(r => r.Password).NotEmpty().WithMessage("Password is required");
    }
}
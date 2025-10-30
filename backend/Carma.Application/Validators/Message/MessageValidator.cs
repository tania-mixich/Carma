using Carma.Application.DTOs.Message;
using FluentValidation;

namespace Carma.Application.Validators.Message;

public class MessageValidator : AbstractValidator<MessageCreateDto>
{
    public MessageValidator()
    {
        RuleFor(m => m.Message).NotEmpty().WithMessage("Message is required")
            .MaximumLength(255).WithMessage("Message must be less than 255 characters")
            .MinimumLength(1).WithMessage("Message must be at least 1 character");
    }
}
namespace Carma.Application.DTOs.Message;

public record MessageGetDto(
    string UserName,
    string Message,
    DateTime SentAt
    );
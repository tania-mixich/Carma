namespace Carma.Application.DTOs.Auth;

public record RegisterRequestDto(string Email, string UserName, string Password, string ConfirmPassword);
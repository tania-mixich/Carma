using Carma.Domain.Enums;

namespace Carma.Application.DTOs.RideParticipant;

public record RideParticipantGetDto(
    string ParticipantName,
    string? ParticipantImageUrl,
    double Karma,
    string RideRole
    );
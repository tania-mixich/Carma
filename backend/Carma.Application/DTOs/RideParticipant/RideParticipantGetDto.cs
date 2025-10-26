using Carma.Domain.Enums;

namespace Carma.Application.DTOs.RideParticipant;

public record RideParticipantGetDto(
    string ParticipantName,
    bool IsAccepted,
    RideRole RideRole
    );
using Carma.Domain.Enums;

namespace Carma.Application.DTOs.RideParticipant;

public record RideParticipantUpdateDto(
    DateTime AcceptedAt,
    bool IsAccepted,
    RideRole RideRole
    );
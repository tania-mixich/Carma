using Carma.Application.DTOs.RideParticipant;
using Carma.Domain.Entities;

namespace Carma.Application.Mappers;

public static class RideParticipantMapper
{
    public static RideParticipant MapToRideParticipant(int rideId)
    {
        return new RideParticipant
        {
            RideId = rideId,
        };
    }

    public static RideParticipantGetDto MapToRideParticipantGetDto(RideParticipant rideParticipant)
    {
        return new RideParticipantGetDto(
            rideParticipant.User?.UserName ?? "Unknown",
            rideParticipant.IsAccepted,
            rideParticipant.RideRole
        );
    }
}
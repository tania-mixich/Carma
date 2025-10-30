using Carma.Application.DTOs.User;
using Carma.Domain.Entities;

namespace Carma.Application.Mappers;

public static class UserMapper
{
    public static UserSummaryDto MapToUserSummaryDto(User user)
    {
        return new UserSummaryDto(user.UserName, user.ImageUrl, user.Karma);
    }

    public static UserProfileDto MapToUserProfileDto(User user)
    {
        return new UserProfileDto(user.UserName, user.ImageUrl, user.Karma, user.RideParticipants.Count);
    }

    public static UserSelfDto MapToUserSelfDto(User user)
    {
        return new UserSelfDto(user.UserName, user.Email, user.ImageUrl, user.Karma, user.RideParticipants.Count,
            user.CreatedAt);
    }
}
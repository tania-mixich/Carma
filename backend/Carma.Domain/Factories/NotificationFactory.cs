using Carma.Domain.Entities;
using Carma.Domain.Enums;

namespace Carma.Domain.Factories;

public static class NotificationFactory
{
    public static Notification CreateMessage(Guid targetId, int rideId, string senderUsername)
    {
        return new Notification
        {
            UserId = targetId,
            RideId = rideId,
            Type = NotificationType.NewMessage,
            Title = "New message",
            Message = $"{senderUsername} sent a message",
            SentAt = DateTime.UtcNow,
            IsRead = false
        };
    }
    
    public static Notification CreateReview(Guid targetId, int rideId, string reviewerUsername)
    {
        return new Notification
        {
            UserId = targetId,
            RideId = rideId,
            Type = NotificationType.NewReview,
            Title = "New review",
            Message = $"{reviewerUsername} reviewed you for the ride",
            SentAt = DateTime.UtcNow,
            IsRead = false
        };
    }
    
    public static Notification CreateJoinRequest(Guid targetId, int rideId, string senderUsername)
    {
        return new Notification
        {
            UserId = targetId,
            RideId = rideId,
            Type = NotificationType.JoinRequest,
            Title = "Ride request",
            Message = $"{senderUsername} requested to join your ride",
            SentAt = DateTime.UtcNow,
            IsRead = false
        };
    }
    
    public static Notification CreateJoinAccepted(Guid targetId, int rideId, string senderUsername)
    {
        return new Notification
        {
            UserId = targetId,
            RideId = rideId,
            Type = NotificationType.JoinAccepted,
            Title = "Ride request accepted",
            Message = $"{senderUsername} accepted your ride request",
            SentAt = DateTime.UtcNow,
            IsRead = false
        };
    }
    
    public static Notification CreateJoinRejected(Guid targetId, int rideId, string senderUsername)
    {
        return new Notification
        {
            UserId = targetId,
            RideId = rideId,
            Type = NotificationType.JoinRejected,
            Title = "Ride request rejected",
            Message = $"{senderUsername} rejected your ride request",
            SentAt = DateTime.UtcNow,
            IsRead = false
        };
    }

    public static Notification CreateLeftRide(Guid targetId, int rideId, string senderUsername)
    {
        return new Notification
        {
            UserId = targetId,
            RideId = rideId,
            Type = NotificationType.LeftRide,
            Title = "Ride member left",
            Message = $"{senderUsername} left the ride",
            SentAt = DateTime.UtcNow,
            IsRead = false
        };
    }
    
    public static Notification CreateRideCancelled(Guid targetId, int rideId, string senderUsername)
    {
        return new Notification
        {
            UserId = targetId,
            RideId = rideId,
            Type = NotificationType.RideCancelled,
            Title = "Ride cancelled",
            Message = $"{senderUsername} cancelled the ride",
            SentAt = DateTime.UtcNow,
            IsRead = false
        };
    }

    public static Notification CreateRideCompleted(Guid targetId, int rideId, string senderUsername)
    {
        return new Notification
        {
            UserId = targetId,
            RideId = rideId,
            Type = NotificationType.RideCompleted,
            Title = "Ride completed",
            Message = $"Ride organized by {senderUsername} completed",
            SentAt = DateTime.UtcNow,
            IsRead = false
        };
    }

    public static Notification CreateRideStarted(Guid targetId, int rideId, string senderUsername)
    {
        return new Notification
        {
            UserId = targetId,
            RideId = rideId,
            Type = NotificationType.RideStarted,
            Title = "Ride in progress",
            Message = $"{senderUsername} marked the ride as in progress",
            SentAt = DateTime.UtcNow,
            IsRead = false
        };
    }
}
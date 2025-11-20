using Carma.Application.Abstractions;
using Carma.Application.Abstractions.Repositories;
using Carma.Application.Common;
using Carma.Application.DTOs.Location;
using Carma.Application.DTOs.Ride;
using Carma.Application.DTOs.RideParticipant;
using Carma.Application.Mappers;
using Carma.Domain.Entities;
using Carma.Domain.Enums;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace Carma.Application.Services;

public class RideService
{
    private readonly ICarmaDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IRideRepository _rideRepository;
    private readonly IValidator<RideCreateDto> _createValidator;
    private readonly IValidator<RideUpdateDto> _updateValidator;

    public RideService(ICarmaDbContext context,ICurrentUserService currentUserService, IRideRepository rideRepository, IValidator<RideCreateDto> createValidator, IValidator<RideUpdateDto> updateValidator)
    {
        _context = context;
        _currentUserService = currentUserService;
        _rideRepository = rideRepository;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<Result<IEnumerable<RideGetDto>>> GetAllRidesAsync()
    {
        var rides = await _context.Rides
            .AsNoTracking()
            //.Include(r => r.Organizer)
            .Select(r => new
            {
                r.Id,
                OrganzierName = r.Organizer.UserName,
                PickupPoint = r.PickupLocation.Coordinate, 
                DropOffPoint = r.DropOffLocation.Coordinate,
                r.PickupTime,
                r.Price,
                r.Seats,
                r.Status, 
                AcceptedCount = r.Participants.Count(p => p.Status == ParticipantStatus.Accepted)
            })
            .OrderByDescending(r => r.PickupTime)
            .ToListAsync();

        var dtos = rides.Select(r => new RideGetDto(
                r.Id,
                r.OrganzierName,
                new LocationGetDto(r.PickupPoint.Y, r.PickupPoint.X),
                new LocationGetDto(r.DropOffPoint.Y, r.DropOffPoint.X),
                r.PickupTime,
                r.AcceptedCount > 0 ? r.Price / r.AcceptedCount : r.Price,
                r.Seats - r.AcceptedCount,
                r.Status.ToString()
            )
        );
        
        return Result<IEnumerable<RideGetDto>>.Success(dtos);
    }

    public async Task<Result<IEnumerable<RideGetDto>>> GetNearbyRidesAsync(RideQueryDto query)
    {
        var pickup = new Point(query.PickupLongitude, query.PickupLatitude) {SRID = 4326};
        IEnumerable<RideGetDto> rides;
        
        if (query.DropoffLatitude.HasValue && query.DropoffLongitude.HasValue)
        {
            var destination = new Point(query.DropoffLongitude.Value, query.DropoffLatitude.Value) {SRID = 4326};
            rides = await _rideRepository.GetNearbyRidesHeadingToTheLocationAsync(pickup, query.PickupRadius, destination, query.DropoffRadius);
            
        }
        else
        {
            rides = await _rideRepository.GetNearbyRidesAsync(pickup, query.PickupRadius);
        }

        return Result<IEnumerable<RideGetDto>>.Success(rides);
    }

    public async Task<Result<IEnumerable<RideGetDto>>> GetPreviousRides()
    {
        var userId = _currentUserService.UserId;
        var rides = await _context.Rides
            .AsNoTracking()
            .Where(r => r.Participants.Any(rp => rp.UserId == userId && rp.Status == ParticipantStatus.Accepted) && r.Status == Status.Completed)
            .OrderByDescending(r => r.PickupTime)
            .Select(r => new
                {
                    r.Id,
                    OrganzierName = r.Organizer.UserName,
                    PickupPoint = r.PickupLocation.Coordinate, 
                    DropOffPoint = r.DropOffLocation.Coordinate,
                    r.PickupTime,
                    r.Price,
                    r.Seats,
                    r.Status, 
                    AcceptedCount = r.Participants.Count(p => p.Status == ParticipantStatus.Accepted)
                })
            .ToListAsync();

        var dtos = rides.Select(r => new RideGetDto(
                r.Id,
                r.OrganzierName,
                new LocationGetDto(r.PickupPoint.Y, r.PickupPoint.X),
                new LocationGetDto(r.DropOffPoint.Y, r.DropOffPoint.X),
                r.PickupTime,
                r.AcceptedCount > 0 ? r.Price / r.AcceptedCount : r.Price,
                r.Seats - r.AcceptedCount,
                r.Status.ToString()
            )
        );
        
        return Result<IEnumerable<RideGetDto>>.Success(dtos);
    } 
    
    public async Task<Result<RideDetailsDto>> GetRideAsync(int rideId)
    {
        var ride = await _context.Rides
            //.Include(r => r.Participants)
                //.ThenInclude(p => p.User)
            .AsNoTracking()
            .Where(r => r.Id == rideId)
            .Select(r => new
            {
                r.Id,
                PickupPoint = r.PickupLocation.Coordinate,
                DropOffPoint = r.DropOffLocation.Coordinate,
                r.PickupTime,
                r.Price,
                r.Seats,
                r.Status,
                Participants = r.Participants.Where(rp => rp.Status == ParticipantStatus.Accepted)
                    .Select(rp => new
                    {
                        rp.User.UserName,
                        rp.User.ImageUrl,
                        rp.User.Karma,
                        Role = rp.Role.ToString()
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync();
        
        if (ride == null)
        {
            return Result<RideDetailsDto>.NotFound("Ride not found");
        }
        
        var acceptedCount = ride.Participants.Count;

        var rideDto = new RideDetailsDto(
            new LocationGetDto(ride.PickupPoint.Y, ride.PickupPoint.X),
            new LocationGetDto(ride.DropOffPoint.Y, ride.DropOffPoint.X),
            ride.PickupTime,
            acceptedCount > 0 ? ride.Price / acceptedCount: ride.Price,
            ride.Seats - acceptedCount,
            ride.Status.ToString(),
            ride.Participants
                .Select(rp => new RideParticipantGetDto(
                rp.UserName,
                rp.ImageUrl,
                rp.Karma,
                rp.Role.ToString()
                ))
                .ToList()
            );
        return Result<RideDetailsDto>.Success(rideDto);
    }

    public async Task<Result<RideGetDto>> CreateRideAsync(RideCreateDto rideCreateDto)
    {
        var validationResult = await _createValidator.ValidateAsync(rideCreateDto);
        if (!validationResult.IsValid)
        {
            return Result<RideGetDto>.Failure(validationResult.Errors.Select(e => e.ErrorMessage).First());       
        }
        
        var userId = _currentUserService.UserId;
        
        var ride = RideMapper.MapToRide(rideCreateDto);
        ride.OrganizerId = userId;
        
        await _context.Rides.AddAsync(ride);
        
        var rideParticipant = new RideParticipant
        {
            RequestedAt = DateTime.UtcNow,
            AcceptedAt = DateTime.UtcNow,
            Ride = ride,
            UserId = userId,
            Role = ParticipantRole.Organizer,
            Status = ParticipantStatus.Accepted
        };
        
        await _context.RideParticipants.AddAsync(rideParticipant);
        await _context.SaveChangesAsync();
        
        return Result<RideGetDto>.Success(new RideGetDto(
            ride.Id,
            _currentUserService.Username,
            new LocationGetDto(ride.PickupLocation.Coordinate.Y, ride.PickupLocation.Coordinate.X),
            new LocationGetDto(ride.DropOffLocation.Coordinate.Y, ride.DropOffLocation.Coordinate.X),
            ride.PickupTime,
            ride.Price,
            ride.Seats - 1,
            ride.Status.ToString()
            )
        );
    }

    public async Task<Result<RideGetDto>> UpdateRideAsync(int id, RideUpdateDto rideUpdateDto)
    {
        var validationResult = await _updateValidator.ValidateAsync(rideUpdateDto);
        if (!validationResult.IsValid)
        {
            return Result<RideGetDto>.Failure(validationResult.Errors.Select(e => e.ErrorMessage).First());      
        }
        
        var ride = await _context.Rides
            .Include(r => r.Organizer)
            .Include(r => r.Participants)
            .FirstOrDefaultAsync(r => r.Id == id);
        if (ride == null)
        {
            return Result<RideGetDto>.NotFound("Ride not found");
        }

        if (ride.OrganizerId != _currentUserService.UserId)
        {
            return Result<RideGetDto>.Unauthorized("You are not the organizer of this ride");
        }

        if (rideUpdateDto.PickupLocation != null)
        {
            ride.PickupLocation = LocationMapper.MapToLocation(rideUpdateDto.PickupLocation);   
        }

        if (rideUpdateDto.DropOffLocation != null)
        {
            ride.DropOffLocation = LocationMapper.MapToLocation(rideUpdateDto.DropOffLocation);
        }

        ride.PickupTime = rideUpdateDto.PickupTime ?? ride.PickupTime;
        ride.Price = rideUpdateDto.Price ?? ride.Price;
        ride.Seats = rideUpdateDto.AvailableSeats ?? ride.Seats;
        ride.UpdatedAt = DateTime.UtcNow;
        
        _context.Rides.Update(ride);
        await _context.SaveChangesAsync();
        
        var acceptedCount = ride.Participants.Count(rp => rp.Status == ParticipantStatus.Accepted);
        
        return Result<RideGetDto>.Success(new RideGetDto(
            ride.Id,
            ride.Organizer.UserName,
            new LocationGetDto(ride.PickupLocation.Coordinate.Y, ride.PickupLocation.Coordinate.X),
            new LocationGetDto(ride.DropOffLocation.Coordinate.Y, ride.DropOffLocation.Coordinate.X),
            ride.PickupTime,
            acceptedCount > 0 ? ride.Price / acceptedCount : ride.Price,
            ride.Seats - acceptedCount,
            ride.Status.ToString()
            )
        );
    }

    public async Task<Result<RideGetDto>> UpdateRideStatusAsync(int rideId, RideStatusUpdateDto dto)
    {
        var statusString = dto.Status;
        if (!Enum.TryParse<Status>(statusString, true, out var newStatus))
        {
            return Result<RideGetDto>.Failure("Invalid status");
        }
        
        var ride = await _context.Rides
            .Include(r => r.Organizer)
            .Include(r => r.Participants)
            .FirstOrDefaultAsync(r => r.Id == rideId);
        if (ride == null)
        {
            return Result<RideGetDto>.NotFound("Ride not found");
        }
        
        if (ride.OrganizerId != _currentUserService.UserId)
        {
            return Result<RideGetDto>.Unauthorized("You are not the organizer of this ride");
        }
        
        ride.Status = newStatus;
        ride.UpdatedAt = DateTime.UtcNow;

        if (newStatus == Status.Completed)
        {
            var participantUserIds = await _context.RideParticipants
                .Where(rp => rp.RideId == rideId && rp.Status == ParticipantStatus.Accepted)
                .Select(rp => rp.UserId)
                .ToListAsync();

            var users = await _context.Users
                .Where(u => participantUserIds.Contains(u.Id))
                .ToListAsync();
            
            foreach (var user in users)
            {
                user.RidesCount++;
            }
            
            var notifications = participantUserIds
                .Where(userId => userId != _currentUserService.UserId)
                .Select(userId => new Notification
                {
                    UserId = userId,
                    RideId = rideId,
                    Title = "Ride completed",
                    Message = $"Ride organized by {_currentUserService.Username} completed",
                    Type = NotificationType.RideCompleted,
                    SentAt = DateTime.UtcNow,
                    IsRead = false
                });
            
            await _context.Notifications.AddRangeAsync(notifications);
        }
        else if (newStatus == Status.Cancelled)
        {
            var participantUserIds = await _context.RideParticipants
                .Where(rp => rp.RideId == rideId && rp.Status == ParticipantStatus.Accepted)
                .Select(rp => rp.UserId)
                .ToListAsync();
            
            var notifications = participantUserIds
                .Where(userId => userId != _currentUserService.UserId)
                .Select(userId => new Notification
                    {
                        UserId = userId,
                        RideId = rideId,
                        Title = "Ride cancelled",
                        Message = $"{_currentUserService.Username} cancelled the ride",
                        Type = NotificationType.RideCancelled,
                        SentAt = DateTime.UtcNow,
                    }
                );

            ride.Organizer.Karma -= 2 / (1 + (double)ride.Organizer.ReviewsCount / 10);
            
            await _context.Notifications.AddRangeAsync(notifications);
        }
        else if (newStatus == Status.InProgress)
        {
            var participantUserIds = await _context.RideParticipants
                .Where(rp => rp.RideId == rideId && rp.Status == ParticipantStatus.Accepted)
                .Select(rp => rp.UserId)
                .ToListAsync();
            
            var notifications = participantUserIds
                .Where(userId => userId != _currentUserService.UserId)
                .Select(userId => new Notification
                {
                    UserId = userId,
                    RideId = rideId,
                    Title = "Ride in progress",
                    Message = $"{_currentUserService.Username} marked the ride as in progress",
                    Type = NotificationType.RideStarted,
                    SentAt = DateTime.UtcNow
                });
            
            await _context.Notifications.AddRangeAsync(notifications);
        }
        
        await _context.SaveChangesAsync();

        var acceptedCount = ride.Participants.Count(rp => rp.Status == ParticipantStatus.Accepted);
        
        return Result<RideGetDto>.Success(new RideGetDto(
            ride.Id,
            ride.Organizer.UserName,
            new LocationGetDto(ride.PickupLocation.Coordinate.Y, ride.PickupLocation.Coordinate.X),
            new LocationGetDto(ride.DropOffLocation.Coordinate.Y, ride.DropOffLocation.Coordinate.X),
            ride.PickupTime,
            acceptedCount > 0 ? ride.Price / acceptedCount : ride.Price,
            ride.Seats - acceptedCount,
            ride.Status.ToString()
            )
        );
    }
}
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
            .Include(r => r.Organizer)
            .ToListAsync();
        return Result<IEnumerable<RideGetDto>>.Success(
            rides.Select(r => new RideGetDto(
                r.Id,
                r.Organizer.UserName,
                new LocationGetDto(r.PickupLocation.Coordinate.Y, r.PickupLocation.Coordinate.X),
                new LocationGetDto(r.DropOffLocation.Coordinate.Y, r.DropOffLocation.Coordinate.X),
                r.PickupTime,
                r.PricePerSeat,
                r.AvailableSeats,
                r.Status.ToString())
            )
        );
    }

    public async Task<Result<IEnumerable<RideGetDto>>> GetNearbyRidesAsync(RideQueryDto query)
    {
        var pickup = new Point(query.PickupLongitude, query.PickupLatitude);
        if (query.DropoffLatitude.HasValue && query.DropoffLongitude.HasValue)
        {
            var destination = new Point(query.DropoffLongitude.Value, query.DropoffLatitude.Value);
            var rides = await _rideRepository.GetNearbyRidesHeadingToTheLocationAsync(pickup, query.PickupRadius, destination, query.DropoffRadius);
            
            return Result<IEnumerable<RideGetDto>>.Success(
                rides.Select(r => new RideGetDto(
                    r.Id,
                    r.Organizer.UserName,
                    new LocationGetDto(r.PickupLocation.Coordinate.Y, r.PickupLocation.Coordinate.X),
                    new LocationGetDto(r.DropOffLocation.Coordinate.Y, r.DropOffLocation.Coordinate.X),
                    r.PickupTime,
                    r.PricePerSeat,
                    r.AvailableSeats,
                    r.Status.ToString()
                ))
            );
        }
        else
        {
            var rides = await _rideRepository.GetNearbyRidesAsync(pickup, query.PickupRadius);
            return Result<IEnumerable<RideGetDto>>.Success(
                rides.Select(r => new RideGetDto(
                    r.Id,
                    r.Organizer.UserName,
                    new LocationGetDto(r.PickupLocation.Coordinate.Y, r.PickupLocation.Coordinate.X),
                    new LocationGetDto(r.DropOffLocation.Coordinate.Y, r.DropOffLocation.Coordinate.X),
                    r.PickupTime,
                    r.PricePerSeat,
                    r.AvailableSeats,
                    r.Status.ToString()
                ))
            );
        }
    }
    
    public async Task<Result<RideDetailsDto>> GetRideAsync(int rideId)
    {
        var ride = await _context.Rides
            .Include(r => r.Participants)
                .ThenInclude(p => p.User)
            .FirstOrDefaultAsync(r => r.Id == rideId);
        
        if (ride == null)
        {
            return Result<RideDetailsDto>.NotFound("Ride not found");
        }

        var rideDto = new RideDetailsDto(
            new LocationGetDto(ride.PickupLocation.Coordinate.Y, ride.PickupLocation.Coordinate.X),
            new LocationGetDto(ride.DropOffLocation.Coordinate.Y, ride.DropOffLocation.Coordinate.X),
            ride.PickupTime,
            ride.PricePerSeat,
            ride.AvailableSeats,
            ride.Status.ToString(),
            ride.Participants
                .Where(p => p.Status == ParticipantStatus.Accepted)
                .Select(p => new RideParticipantGetDto(
                p.User.UserName,
                p.User.ImageUrl,
                p.User.Karma,
                p.Role.ToString()
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
            ride.PricePerSeat,
            ride.AvailableSeats,
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
        ride.AvailableSeats = rideUpdateDto.AvailableSeats ?? ride.AvailableSeats;
        ride.UpdatedAt = DateTime.UtcNow;
        
        _context.Rides.Update(ride);
        await _context.SaveChangesAsync();
        
        return Result<RideGetDto>.Success(new RideGetDto(
            ride.Id,
            ride.Organizer.UserName,
            new LocationGetDto(ride.PickupLocation.Coordinate.Y, ride.PickupLocation.Coordinate.X),
            new LocationGetDto(ride.DropOffLocation.Coordinate.Y, ride.DropOffLocation.Coordinate.X),
            ride.PickupTime,
            ride.PricePerSeat,
            ride.AvailableSeats,
            ride.Status.ToString()
            )
        );
    }

    public async Task<Result<RideGetDto>> UpdateRideStatusAsync(int rideId, RideStatusUpdateDto dto)
    {
        if (!Enum.TryParse<Status>(dto.Status, out var status))
        {
            return Result<RideGetDto>.Failure("Invalid status");
        }
        
        var ride = await _context.Rides
            .Include(r => r.Organizer)
            .FirstOrDefaultAsync(r => r.Id == rideId);
        if (ride == null)
        {
            return Result<RideGetDto>.NotFound("Ride not found");
        }
        
        if (ride.OrganizerId != _currentUserService.UserId)
        {
            return Result<RideGetDto>.Unauthorized("You are not the organizer of this ride");
        }
        
        ride.Status = status;
        ride.UpdatedAt = DateTime.UtcNow;

        if (status == Status.Completed)
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
        else if (status == Status.Cancelled)
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
        else if (status == Status.InProgress)
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
        return Result<RideGetDto>.Success(new RideGetDto(
            ride.Id,
            ride.Organizer.UserName,
            new LocationGetDto(ride.PickupLocation.Coordinate.Y, ride.PickupLocation.Coordinate.X),
            new LocationGetDto(ride.DropOffLocation.Coordinate.Y, ride.DropOffLocation.Coordinate.X),
            ride.PickupTime,
            ride.PricePerSeat,
            ride.AvailableSeats,
            ride.Status.ToString()
            )
        );
    }
}
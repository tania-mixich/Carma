using Carma.Application.Abstractions;
using Carma.Application.Abstractions.Repositories;
using Carma.Application.Common;
using Carma.Application.DTOs.Location;
using Carma.Application.DTOs.Notification;
using Carma.Application.DTOs.Ride;
using Carma.Application.DTOs.RideParticipant;
using Carma.Application.Mappers;
using Carma.Domain.Entities;
using Carma.Domain.Enums;
using Carma.Domain.Factories;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using Location = Carma.Domain.ValueObjects.Location;

namespace Carma.Application.Services;

public class RideService
{
    private readonly ICarmaDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IRideRepository _rideRepository;
    private readonly IValidator<RideCreateDto> _createValidator;
    private readonly IValidator<RideUpdateDto> _updateValidator;
    private readonly IRealTimeNotifier _realTimeNotifier;
    private readonly IGeocodingService _geocodingService;

    public RideService(ICarmaDbContext context,ICurrentUserService currentUserService, IRideRepository rideRepository, IValidator<RideCreateDto> createValidator, IValidator<RideUpdateDto> updateValidator, IRealTimeNotifier realTimeNotifier, IGeocodingService geocodingService)
    {
        _context = context;
        _currentUserService = currentUserService;
        _rideRepository = rideRepository;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _realTimeNotifier = realTimeNotifier;
        _geocodingService = geocodingService;
    }

    public async Task<Result<IEnumerable<RideGetDto>>> GetAllRidesAsync()
    {
        var rides = await _context.Rides
            .AsNoTracking()
            .Select(r => new RideLookupDto
            {
                Id = r.Id,
                OrganizerName = r.Organizer.UserName ?? string.Empty,
                OrganizerKarma = r.Organizer.Karma,
                OrganizerImageUrl = r.Organizer.ImageUrl,
                PickupPoint = r.PickupLocation.Coordinate,
                PickupAddress = r.PickupLocation.Address,
                PickupCity = r.PickupLocation.City ?? string.Empty,
                PickupCountry = r.PickupLocation.Country ?? string.Empty,
                DropoffPoint = r.DropOffLocation.Coordinate,
                DropoffAddress = r.DropOffLocation.Address,
                DropoffCity = r.DropOffLocation.City ?? string.Empty,
                DropoffCountry = r.DropOffLocation.Country ?? string.Empty,
                PickupTime = r.PickupTime,
                Price = r.Price,
                Seats = r.Seats,
                Status = r.Status, 
                AcceptedCount = r.Participants.Count(p => p.Status == ParticipantStatus.Accepted),
                UserStatus = r.OrganizerId == _currentUserService.UserId ? "Organizer" : 
                    r.Participants
                        .Where(p => p.UserId == _currentUserService.UserId)
                        .Select(p => p.Status.ToString())
                        .FirstOrDefault() ?? "None",
            })
            .OrderByDescending(r => r.PickupTime)
            .ToListAsync();

        var dtos = rides.Select(RideMapper.MapToGetDto);
        
        return Result<IEnumerable<RideGetDto>>.Success(dtos);
    }

    public async Task<Result<IEnumerable<RideGetDto>>> GetNearbyRidesAsync(RideQueryDto query)
    {
        var pickup = new Point(query.PickupLongitude, query.PickupLatitude) {SRID = 4326};
        IEnumerable<RideGetDto> rides;
        
        if (query.DropoffLatitude.HasValue && query.DropoffLongitude.HasValue)
        {
            var destination = new Point(query.DropoffLongitude.Value, query.DropoffLatitude.Value) {SRID = 4326};
            rides = await _rideRepository.GetNearbyRidesHeadingToTheLocationAsync(pickup, query.PickupRadius, destination, query.DropoffRadius, _currentUserService.UserId);
            
        }
        else
        {
            rides = await _rideRepository.GetNearbyRidesAsync(pickup, query.PickupRadius, _currentUserService.UserId);
        }

        return Result<IEnumerable<RideGetDto>>.Success(rides);
    }

    public async Task<Result<IEnumerable<RideGetDto>>> GetPreviousRides()
    {
        var userId = _currentUserService.UserId;
        var rides = await _context.Rides
            .AsNoTracking()
            .Where(r => r.Participants.Any(rp => rp.UserId == userId && rp.Status == ParticipantStatus.Accepted) 
                        && (r.Status == Status.Completed || r.Status == Status.InProgress))
            .OrderByDescending(r => r.PickupTime)
            .Select(r => new RideLookupDto
            {
                Id = r.Id,
                OrganizerName = r.Organizer.UserName ?? string.Empty,
                OrganizerKarma = r.Organizer.Karma,
                OrganizerImageUrl = r.Organizer.ImageUrl,
                PickupPoint = r.PickupLocation.Coordinate, 
                PickupAddress = r.PickupLocation.Address,
                PickupCity = r.PickupLocation.City ?? string.Empty,
                PickupCountry = r.PickupLocation.Country ?? string.Empty,
                DropoffPoint = r.DropOffLocation.Coordinate,
                DropoffAddress = r.DropOffLocation.Address,
                DropoffCity = r.DropOffLocation.City ?? string.Empty,
                DropoffCountry = r.DropOffLocation.Country ?? string.Empty,
                PickupTime = r.PickupTime,
                Price = r.Price,
                Seats = r.Seats,
                Status = r.Status, 
                AcceptedCount = r.Participants.Count(p => p.Status == ParticipantStatus.Accepted),
                UserStatus = r.OrganizerId == _currentUserService.UserId ? "Organizer" : 
                    r.Participants
                        .Where(p => p.UserId == _currentUserService.UserId)
                        .Select(p => p.Status.ToString())
                        .FirstOrDefault() ?? "None",
            })
            .ToListAsync();

        var dtos = rides.Select(RideMapper.MapToGetDto);
        
        return Result<IEnumerable<RideGetDto>>.Success(dtos);
    } 
    
    public async Task<Result<RideDetailsDto>> GetRideAsync(int rideId)
    {
        var ride = await _context.Rides
            .AsNoTracking()
            .Where(r => r.Id == rideId)
            .Select(r => new
            {
                r.Id,
                r.PickupLocation,
                r.DropOffLocation,
                r.PickupTime,
                r.Price,
                r.Seats,
                r.Status,
                Participants = r.Participants.Where(rp => rp.Status == ParticipantStatus.Accepted)
                    .Select(rp => new
                    {
                        rp.UserId,
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
            new LocationGetDto(
                ride.PickupLocation.Coordinate.Y, 
                ride.PickupLocation.Coordinate.X,
                ride.PickupLocation.Address,
                ride.PickupLocation.City ?? string.Empty,
                ride.PickupLocation.Country ?? string.Empty
                ),
            new LocationGetDto(
                ride.DropOffLocation.Coordinate.Y, 
                ride.DropOffLocation.Coordinate.X,
                ride.DropOffLocation.Address,
                ride.DropOffLocation.City ?? string.Empty,
                ride.DropOffLocation.Country ?? string.Empty
                ),
            ride.PickupTime,
            acceptedCount > 0 ? ride.Price / acceptedCount: ride.Price,
            ride.Seats - acceptedCount,
            ride.Status.ToString(),
            ride.Participants
                .Select(rp => new RideParticipantGetDto(
                    rp.UserId,
                    rp.UserName ?? string.Empty,
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
            var errors = validationResult.Errors.Select(e => e.ErrorMessage);
            return Result<RideGetDto>.Failure(string.Join("; ", errors));       
        }
        
        var userId = _currentUserService.UserId;
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
        {
            return Result<RideGetDto>.NotFound("User not found");
        }

        Location pickupLocation;
        if (!string.IsNullOrEmpty(rideCreateDto.PickupLocation.Address))
        {
            pickupLocation = new Location(rideCreateDto.PickupLocation.Latitude,
                rideCreateDto.PickupLocation.Longitude,
                rideCreateDto.PickupLocation.Address,
                rideCreateDto.PickupLocation.City,
                rideCreateDto.PickupLocation.Country
                );
        }
        else
        {
            pickupLocation = await _geocodingService.GetLocationFromCoordinatesAsync(rideCreateDto.PickupLocation.Latitude, rideCreateDto.PickupLocation.Longitude);
        }
        
        Location dropOffLocation;
        if (!string.IsNullOrEmpty(rideCreateDto.DropOffLocation.Address))
        {
            dropOffLocation = new Location(rideCreateDto.DropOffLocation.Latitude,
                rideCreateDto.DropOffLocation.Longitude,
                rideCreateDto.DropOffLocation.Address,
                rideCreateDto.DropOffLocation.City,
                rideCreateDto.DropOffLocation.Country
                );
        }
        else
        {
            dropOffLocation = await _geocodingService.GetLocationFromCoordinatesAsync(rideCreateDto.DropOffLocation.Latitude, rideCreateDto.DropOffLocation.Longitude);
        }
        
        var ride = RideMapper.MapToRide(rideCreateDto, pickupLocation, dropOffLocation);
        ride.OrganizerId = userId;
        
        ride.Participants.Add(new RideParticipant
        {
            RequestedAt = DateTime.UtcNow,
            AcceptedAt = DateTime.UtcNow,
            Ride = ride,
            UserId = userId,
            Role = ParticipantRole.Organizer,
            Status = ParticipantStatus.Accepted
        });
        
        await _context.Rides.AddAsync(ride);
        await _context.SaveChangesAsync();
        
        return Result<RideGetDto>.Success(new RideGetDto(
            ride.Id,
            _currentUserService.Username,
            user.Karma,
            user.ImageUrl ?? string.Empty,
            new LocationGetDto(
                ride.PickupLocation.Coordinate.Y, 
                ride.PickupLocation.Coordinate.X,
                ride.PickupLocation.Address,
                ride.PickupLocation.City ?? string.Empty,
                ride.PickupLocation.Country ?? string.Empty
                ),
            new LocationGetDto(
                ride.DropOffLocation.Coordinate.Y, 
                ride.DropOffLocation.Coordinate.X, 
                ride.DropOffLocation.Address, 
                ride.DropOffLocation.City ?? string.Empty, 
                ride.DropOffLocation.Country ?? string.Empty),
            ride.PickupTime,
            ride.Price,
            ride.Seats,
            ride.Status.ToString(),
            "Organizer"
            )
        );
    }

    public async Task<Result<RideGetDto>> UpdateRideAsync(int id, RideUpdateDto rideUpdateDto)
    {
        var validationResult = await _updateValidator.ValidateAsync(rideUpdateDto);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage);
            return Result<RideGetDto>.Failure(string.Join("; ", errors));   
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
        
        await _context.SaveChangesAsync();
        
        var acceptedCount = ride.Participants.Count(rp => rp.Status == ParticipantStatus.Accepted);
        
        return Result<RideGetDto>.Success(new RideGetDto(
            ride.Id,
            ride.Organizer.UserName ?? string.Empty,
            ride.Organizer.Karma,
            ride.Organizer.ImageUrl ?? string.Empty,
            new LocationGetDto(
                ride.PickupLocation.Coordinate.Y, 
                ride.PickupLocation.Coordinate.X, 
                ride.PickupLocation.Address, 
                ride.PickupLocation.City ?? string.Empty, 
                ride.PickupLocation.Country ?? string.Empty
                ),
            new LocationGetDto(
                ride.DropOffLocation.Coordinate.Y, 
                ride.DropOffLocation.Coordinate.X, 
                ride.DropOffLocation.Address, 
                ride.DropOffLocation.City ?? string.Empty, 
                ride.DropOffLocation.Country ?? string.Empty
                ),
            ride.PickupTime,
            acceptedCount > 0 ? ride.Price / acceptedCount : ride.Price,
            ride.Seats - acceptedCount,
            ride.Status.ToString(),
            "Organizer"
            )
        );
    }

    public async Task<Result<RideGetDto>> UpdateRideStatusAsync(int rideId, RideStatusUpdateDto dto)
    {
        if (!Enum.TryParse<Status>(dto.Status, true, out var newStatus))
        {
            return Result<RideGetDto>.Failure("Invalid status");
        }
        
        var ride = await _context.Rides
            .Include(r => r.Organizer)
            .Include(r => r.Participants)
                .ThenInclude(p => p.User)
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

        var notificationsToSend = new List<Notification>();
        var participantsToNotify = ride.Participants.Where(rp => rp.Status == ParticipantStatus.Accepted);
        
        
        if (newStatus == Status.Completed)
        {
            foreach (var rp in ride.Participants.Where(rp => rp.Status == ParticipantStatus.Accepted))
            {
                rp.User.RidesCount++;
            }
            
            var notifications = participantsToNotify
                .Select(rp => NotificationFactory.CreateRideCompleted(rp.UserId, rideId, _currentUserService.Username));
            
            notificationsToSend.AddRange(notifications);
        }
        else if (newStatus == Status.Cancelled)
        {
            ride.Organizer.Karma -= 2 / (1 + (double)ride.Organizer.ReviewsCount / 10);
            
            var notifications = participantsToNotify
                .Select(rp => NotificationFactory.CreateRideCancelled(rp.UserId, rideId, _currentUserService.Username)
                );

            notificationsToSend.AddRange(notifications);
        }
        else if (newStatus == Status.InProgress)
        {
            var notifications = participantsToNotify
                .Select(rp => NotificationFactory.CreateRideStarted(rp.UserId, rideId, _currentUserService.Username));
            
            notificationsToSend.AddRange(notifications);
        }
        
        if (notificationsToSend.Any())
        {
            await _context.Notifications.AddRangeAsync(notificationsToSend);
        }
        
        await _context.SaveChangesAsync();

        foreach (var notification in notificationsToSend)
        {
            var notificationDto = new NotificationGetDto(
                notification.Id,
                notification.Title,
                notification.Message,
                notification.Type.ToString(),
                notification.SentAt,
                notification.RideId,
                false
                );

            await _realTimeNotifier.NotificationReceivedAsync(notification.UserId, notificationDto);
        }
        
        await _realTimeNotifier.RideStatusUpdatedAsync(rideId, newStatus.ToString());
        
        var acceptedCount = ride.Participants.Count(rp => rp.Status == ParticipantStatus.Accepted);
        
        return Result<RideGetDto>.Success(new RideGetDto(
            ride.Id,
            ride.Organizer.UserName ?? string.Empty,
            ride.Organizer.Karma,
            ride.Organizer.ImageUrl ?? string.Empty,
            new LocationGetDto(
                ride.PickupLocation.Coordinate.Y, 
                ride.PickupLocation.Coordinate.X,
                ride.PickupLocation.Address,
                ride.PickupLocation.City ?? string.Empty,
                ride.PickupLocation.Country ?? string.Empty
                ),
            new LocationGetDto(
                ride.DropOffLocation.Coordinate.Y, 
                ride.DropOffLocation.Coordinate.X,
                ride.DropOffLocation.Address,
                ride.DropOffLocation.City ?? string.Empty,
                ride.DropOffLocation.Country ?? string.Empty
                ),
            ride.PickupTime,
            acceptedCount > 0 ? ride.Price / acceptedCount : ride.Price,
            ride.Seats - acceptedCount,
            ride.Status.ToString(),
            "Organizer"
            )
        );
    }
}
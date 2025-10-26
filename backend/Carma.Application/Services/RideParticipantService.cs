using Carma.Application.Abstractions;
using Carma.Application.Common;
using Carma.Application.DTOs.RideParticipant;
using Carma.Application.Mappers;
using Carma.Domain.Enums;
using FluentValidation;

namespace Carma.Application.Services;

public class RideParticipantService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IRideParticipantRepository _rideParticipantRepository;
    private readonly IRideRepository _rideRepository;

    public RideParticipantService(IUnitOfWork unitOfWork, ICurrentUserService currentUserService,
        IRideParticipantRepository rideParticipantRepository, IRideRepository rideRepository)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _rideParticipantRepository = rideParticipantRepository;
        _rideRepository = rideRepository;
    }
    
    public async Task<Result<IEnumerable<RideParticipantGetDto>>> GetParticipantsOfRideAsync(int rideId)
    {
        var rideParticipants = await _rideParticipantRepository.GetAllByRideIdAsync(rideId);
        return Result<IEnumerable<RideParticipantGetDto>>.Success(rideParticipants.Select(RideParticipantMapper.MapToRideParticipantGetDto));
    }

    public async Task<Result<RideParticipantGetDto>> GetParticipantByRideAndUserAsync(int rideId,
        Guid rideParticipantId)
    {
        var rideParticipant = await _rideParticipantRepository.GetByRideAndUserAsync(rideId, rideParticipantId);
        if (rideParticipant == null)
        {
            return Result<RideParticipantGetDto>.Failure("Participant not found");
        }
        return Result<RideParticipantGetDto>.Success(RideParticipantMapper.MapToRideParticipantGetDto(rideParticipant));
    }

    public async Task<Result<RideParticipantGetDto>> CreateRideParticipantAsync(int rideId)
    {
        var ride = await _rideRepository.GetByIdAsync(rideId);
        if (ride == null)
        {
            return Result<RideParticipantGetDto>.Failure("Ride not found");
        }
        
        var rideParticipant = RideParticipantMapper.MapToRideParticipant(rideId);
        
        var userId = _currentUserService.UserId;
        rideParticipant.UserId = userId;
        rideParticipant.RequestedAt = DateTime.UtcNow;
        
        if (ride.OrganizerId == userId)
        {
            rideParticipant.IsAccepted = true;
            rideParticipant.RideRole = RideRole.Organizer;
        }
        else
        {
            rideParticipant.IsAccepted = false;
            rideParticipant.RideRole = RideRole.NotAssigned;
        }
        
        await _rideParticipantRepository.AddAsync(rideParticipant);
        await _unitOfWork.SaveChangesAsync();
        
        return Result<RideParticipantGetDto>.Success(RideParticipantMapper.MapToRideParticipantGetDto(rideParticipant));
    }

    public async Task<Result<RideParticipantGetDto>> UpdateRideParticipantAsync(int rideId, Guid rideParticipantId,
        RideParticipantUpdateDto rideParticipantUpdateDto)
    {
        var rideParticipant = await _rideParticipantRepository.GetByRideAndUserAsync(rideId, rideParticipantId);
        if (rideParticipant == null)
        {
            return Result<RideParticipantGetDto>.Failure("Ride participant not found");
        }
        
        rideParticipant.AcceptedAt = rideParticipantUpdateDto.AcceptedAt;
        rideParticipant.IsAccepted = rideParticipantUpdateDto.IsAccepted;
        rideParticipant.RideRole = rideParticipantUpdateDto.RideRole;
        
        _rideParticipantRepository.Update(rideParticipant);
        await _unitOfWork.SaveChangesAsync();
        
        return Result<RideParticipantGetDto>.Success(RideParticipantMapper.MapToRideParticipantGetDto(rideParticipant));
    }

    public async Task<Result> DeleteRideParticipantAsync(int rideId, Guid rideParticipantId)
    {
        var rideParticipant = await _rideParticipantRepository.GetByRideAndUserAsync(rideId, rideParticipantId);
        if (rideParticipant == null)
        {
            return Result.Failure("Ride participant not found");
        }
        
        _rideParticipantRepository.Delete(rideParticipant);
        await _unitOfWork.SaveChangesAsync();
        
        return Result.Success();
    }
}
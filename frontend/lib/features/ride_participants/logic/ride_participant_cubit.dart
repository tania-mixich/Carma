import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:frontend/features/ride_participants/data/repository/ride_participant_repository.dart';
import 'package:frontend/features/ride_participants/logic/ride_participant_state.dart';

class RideParticipantCubit extends Cubit<RideParticipantState> {
  final RideParticipantRepository _repository;

  RideParticipantCubit({RideParticipantRepository? repository})
      : _repository = repository ?? RideParticipantRepository(),
        super(RideParticipantInitial());

  Future<void> requestToJoinRide(int rideId) async {
    emit(RideParticipantLoading());
    
    try {
      await _repository.requestToJoinRide(rideId);
      emit(RideParticipantSuccess('Join request sent successfully', rideId: rideId));
    } catch (e) {
      print('Join Error: $e');
      emit(RideParticipantError(e.toString()));
    }
  }

  Future<void> acceptParticipant(int rideId, String userId) async {
    emit(RideParticipantLoading());
    
    try {
      await _repository.acceptParticipant(rideId: rideId, userId: userId);
      emit(RideParticipantSuccess('Participant accepted'));
    } catch (e) {
      emit(RideParticipantError(e.toString()));
    }
  }

  Future<void> rejectParticipant(int rideId, String userId) async {
    emit(RideParticipantLoading());
    
    try {
      await _repository.rejectParticipant(rideId: rideId, userId: userId);
      emit(RideParticipantSuccess('Participant rejected'));
    } catch (e) {
      emit(RideParticipantError(e.toString()));
    }
  }

  Future<void> leaveRide(int rideId) async {
    emit(RideParticipantLoading());
    
    try {
      await _repository.leaveRide(rideId);
      emit(RideParticipantSuccess('You left the ride'));
    } catch (e) {
      emit(RideParticipantError(e.toString()));
    }
  }

  // Future<bool> canJoinChat(int rideId) async {
  //   try {
  //     return await _repository.canJoinChat(rideId);
  //   } catch (e) {
  //     return false;
  //   }
  // }

  void reset() {
    emit(RideParticipantInitial());
  }
}
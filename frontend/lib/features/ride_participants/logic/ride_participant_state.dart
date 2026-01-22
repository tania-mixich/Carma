import 'package:equatable/equatable.dart';
import 'package:frontend/features/ride_participants/data/models/ride_participant_get.dart';

abstract class RideParticipantState extends Equatable {
  @override
  List<Object?> get props => [];
}

class RideParticipantInitial extends RideParticipantState {}

class RideParticipantLoading extends RideParticipantState {}

class RideParticipantLoaded extends RideParticipantState {
  final List<RideParticipantGet> pendingParticipants;

  RideParticipantLoaded(this.pendingParticipants);

  @override
  List<Object?> get props => [pendingParticipants];
}

class RideParticipantSuccess extends RideParticipantState {
  final String message;
  final int? rideId;
  
  RideParticipantSuccess(this.message, {this.rideId});
  
  @override
  List<Object?> get props => [message, rideId];
}

class RideParticipantError extends RideParticipantState {
  final String message;
  
  RideParticipantError(this.message);
  
  @override
  List<Object?> get props => [message];
}
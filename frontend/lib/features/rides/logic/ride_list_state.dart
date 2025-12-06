import 'package:equatable/equatable.dart';
import 'package:frontend/domain/models/ride.dart';

abstract class RideListState extends Equatable {
  @override
  List<Object?> get props => [];
}

class RideListInitial extends RideListState {}

class RideListLoading extends RideListState {}

class RideListLoaded extends RideListState {
  final List<Ride> rides;
  final int pickupRadius;
  final int dropoffRadius;

  RideListLoaded({
    required this.rides,
    required this.pickupRadius,
    required this.dropoffRadius,
  });

  @override
  List<Object?> get props => [rides, pickupRadius, dropoffRadius];
}

class RideListError extends RideListState {
  final String message;

  RideListError(this.message);

  @override
  List<Object?> get props => [message];
}
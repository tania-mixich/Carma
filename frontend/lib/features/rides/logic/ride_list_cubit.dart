import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:frontend/core/services/ride_query_storage_service.dart';
import 'package:frontend/features/rides/data/models/ride_query.dart';
import 'package:frontend/features/rides/data/repository/ride_repository.dart';
import 'package:frontend/features/rides/logic/ride_list_state.dart';

class RideListCubit extends Cubit<RideListState> {
  final RideRepository _repository;
  final RideQueryStorageService _queryStorage;

  static const double _defaultLatitude = 45.7326;
  static const double _defaultLongitude = 21.208;

  RideListCubit({
    RideRepository? repository,
    RideQueryStorageService? queryStorage,
  })  : _repository = repository ?? RideRepository(),
        _queryStorage = queryStorage ?? RideQueryStorageService(),
        super(RideListInitial());


  Future<void> initialize() async {
    await _initializeLocation();
    await loadNearbyRides();
  }

  Future<void> _initializeLocation() async {
    final location = await _queryStorage.getPickupLocation();
    if (location == null) {
      await _queryStorage.savePickupLocation(_defaultLatitude, _defaultLongitude);
    }
  }

  Future<void> loadNearbyRides() async {
    emit(RideListLoading());

    try {
      final pickupLocation = await _queryStorage.getPickupLocation();
      final pickupRadius = await _queryStorage.getPickupRadius();
      final dropoffLocation = await _queryStorage.getDropoffLocation();
      final dropoffRadius = await _queryStorage.getDropoffRadius();

      final query = RideQuery(
        pickupLatitude: pickupLocation?['latitude'] ?? _defaultLatitude,
        pickupLongitude: pickupLocation?['longitude'] ?? _defaultLongitude,
        pickupRadius: pickupRadius,
        dropoffRadius: dropoffRadius,
        dropoffLatitude: dropoffLocation?['latitude'],
        dropoffLongitude: dropoffLocation?['longitude'],
      );

      final rides = await _repository.getNearbyRides(query);

      emit(RideListLoaded(
        rides: rides,
        pickupRadius: pickupRadius,
        dropoffRadius: dropoffRadius,
      ));

      print('Rides loaded: ${rides.length}');
    } catch (e) {
      print('Error loading rides: $e');
      emit(RideListError(e.toString()));
    }
  }

  Future<void> updateRadiusFilters(int pickupRadius, int dropoffRadius) async {
    await _queryStorage.saveRadii(pickupRadius, dropoffRadius);
    await loadNearbyRides();
  }

  Future<void> updatePickupLocation(double latitude, double longitude) async {    
    await _queryStorage.savePickupLocation(latitude, longitude);
    await loadNearbyRides();
  }

  Future<void> updateDropoffLocation(double? latitude, double? longitude) async {
    if (latitude != null && longitude != null) {
      await _queryStorage.saveDropoffLocation(latitude, longitude);
    } else {
      await _queryStorage.clearDropoffLocation();
    }
    await loadNearbyRides();
  }

  Future<Map<String, int>> getCurrentRadii() async {
    return {
      'pickup': await _queryStorage.getPickupRadius(),
      'dropoff': await _queryStorage.getDropoffRadius(),
    };
  }

  void updateRideUserStatus(int rideId, String newStatus) {
    if (state is RideListLoaded) {
      final currentState = state as RideListLoaded;
      
      final updatedRides = currentState.rides.map((ride) {
        return ride.id == rideId ? ride.copyWith(userStatus: newStatus) : ride;
      }).toList();

      emit(RideListLoaded(
        rides: updatedRides,
        pickupRadius: currentState.pickupRadius,
        dropoffRadius: currentState.dropoffRadius,
      ));
    }
  }
}
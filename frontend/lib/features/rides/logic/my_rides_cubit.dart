import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:frontend/features/rides/data/repository/ride_repository.dart';
import 'package:frontend/features/rides/logic/ride_list_state.dart';

class MyRidesCubit extends Cubit<RideListState> {
  final RideRepository _repository;

  MyRidesCubit({RideRepository? repository})
      : _repository = repository ?? RideRepository(),
        super(RideListInitial());

  Future<void> loadRidesHistory() async {
    emit(RideListLoading());

    try {
      final rides = await _repository.getRidesHistory();
      emit(RideListLoaded(
        rides: rides,
        pickupRadius: 0,
        dropoffRadius: 0)
      );
    } catch (e) {
      emit(RideListError(e.toString()));
    }
  }
}
import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:frontend/features/home/presentation/widgets/error_loading_message.dart';
import 'package:frontend/features/home/presentation/widgets/no_rides_message.dart';
import 'package:frontend/features/home/presentation/widgets/radius_filters_button.dart';
import 'package:frontend/features/home/presentation/widgets/ride_card.dart';
import 'package:frontend/features/ride_participants/logic/ride_participant_cubit.dart';
import 'package:frontend/features/rides/logic/ride_list_cubit.dart';
import 'package:frontend/features/rides/logic/ride_list_state.dart';

class NearbyRidesSection extends StatelessWidget {
  final double screenWidth;
  final double screenHeight;

  const NearbyRidesSection({
    super.key,
    required this.screenWidth,
    required this.screenHeight,
  });
  
  String _formatTime(DateTime dateTime) {
    if (dateTime.year == DateTime.now().year && dateTime.month == DateTime.now().month) {
      if (dateTime.day == DateTime.now().day) {
        final hour = dateTime.hour;
        final minute = dateTime.minute.toString().padLeft(2, '0');
        return 'Today at $hour:$minute';
      } else if (dateTime.day == DateTime.now().day + 1) {
        final hour = dateTime.hour;
        final minute = dateTime.minute.toString().padLeft(2, '0');
        return 'Tomorrow at $hour:$minute';
      }
    }

    Map<int, String> weekdays = {
      1: 'Monday',
      2: 'Tuesday',
      3: 'Wednesday',
      4: 'Thursday',
      5: 'Friday',
      6: 'Saturday',
      7: 'Sunday',
    };

    final weekday = weekdays[dateTime.weekday];
    final hour = dateTime.hour;
    final minute = dateTime.minute.toString().padLeft(2, '0');
    return '$weekday at $hour:$minute';
  }

  @override
  Widget build(BuildContext context) {
    return BlocBuilder<RideListCubit, RideListState>(
      builder: (context, rideState) {
        if (rideState is RideListLoading) {
          return const Center(
            child: Padding(
              padding: EdgeInsets.all(50),
              child: CircularProgressIndicator(),
            ),
          );
        } else if (rideState is RideListError) {
          return ErrorLoadingMessage(screenWidth: screenWidth, rideState: rideState);
        } else if (rideState is RideListLoaded) {
          final rides = rideState.rides;

          return Column(
            crossAxisAlignment:
                CrossAxisAlignment.start,
            children: [
              // HEADER
              Row(
                mainAxisAlignment:
                    MainAxisAlignment.spaceBetween,
                children: [
                  Text(
                    "Nearby Rides",
                    style: TextStyle(
                      fontSize: screenWidth * 0.054,
                      fontWeight: FontWeight.bold,
                    ),
                  ),

                  RadiusFiltersButton(),
                ],
              ),
              Text(
                "${rides.length} available",
                style: TextStyle(
                  fontSize: screenWidth * 0.04,
                  color: Colors.deepOrange,
                ),
              ),
              
              SizedBox(height: screenHeight * 0.02),

              // RIDES LIST
              if (rides.isEmpty)
                NoRidesMessage(screenWidth: screenWidth)
              else
                ...rides.map((ride) {
                  return Padding(
                    padding: const EdgeInsets.only(bottom: 15),
                    child: RideCard(
                      id: ride.id,
                      name: ride.organizerName,
                      rating: ride.karma,
                      seatsLeft: ride.availableSeats,
                      from: ride.pickupLocation.address ?? "Unknown",
                      to: ride.dropOffLocation.address ?? "Unknown",
                      time: "Pickup: ${_formatTime(ride.pickupTime)}",
                      price: "\$${ride.pricePerSeat.toStringAsFixed(2)}",
                      imageUrl: ride.imageUrl ?? "https://i.imgur.com/BoN9kdC.png",
                      userStatus: ride.userStatus,
                      onJoinPressed: () {
                        context.read<RideParticipantCubit>().requestToJoinRide(ride.id);
                      },
                    ),
                  );
                }),

              const SizedBox(height: 30),
            ],
          );
        }

        return const SizedBox.shrink();
      },
    );
  }
}
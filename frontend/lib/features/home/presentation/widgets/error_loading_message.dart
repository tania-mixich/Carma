import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:frontend/features/rides/logic/ride_list_cubit.dart';
import 'package:frontend/features/rides/logic/ride_list_state.dart';

class ErrorLoadingMessage extends StatelessWidget {
  final double screenWidth;
  final RideListState rideState;

  const ErrorLoadingMessage({
    super.key,
    required this.screenWidth,
    required this.rideState,
  });

  @override
  Widget build(BuildContext context) {
    return Center(
      child: Padding(
        padding: const EdgeInsets.all(20),
        child: Column(
          children: [
            const Icon(
              Icons.error_outline,
              size: 60,
              color: Colors.red,
            ),
            const SizedBox(height: 10),
            Text(
              'Error loading rides',
              style: TextStyle(
                fontSize: screenWidth * 0.05,
                fontWeight: FontWeight.bold,
              ),
            ),
            const SizedBox(height: 10),
            Text(
              (rideState as RideListError).message,
              textAlign: TextAlign.center,
              style: const TextStyle(
                  color: Colors.grey),
            ),
            const SizedBox(height: 20),
            ElevatedButton(
              onPressed: () {
                context
                    .read<RideListCubit>()
                    .loadNearbyRides();
              },
              child: const Text('Retry'),
            ),
          ],
        ),
      ),
    );
  }
}
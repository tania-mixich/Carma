import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:frontend/features/rides/logic/ride_list_cubit.dart';

class RadiusFiltersButton extends StatelessWidget {
  const RadiusFiltersButton({super.key});

  @override
  Widget build(BuildContext context) {
    final screenWidth = MediaQuery.of(context).size.width;

    return IconButton(
      onPressed: () async {
        final rideListCubit = context.read<RideListCubit>();
        
        final currentRadii = await rideListCubit.getCurrentRadii();
        int pickupRadius = currentRadii['pickup'] ?? 1000;
        int dropoffRadius = currentRadii['dropoff'] ?? 1000;

        if (!context.mounted) return;

        showDialog(
          context: context,
          builder: (dialogContext) {
            return StatefulBuilder(
              builder: (builderContext, setState) {
                return Dialog(
                  shape: RoundedRectangleBorder(
                    borderRadius: BorderRadius.circular(20),
                  ),
                  child: Padding(
                    padding: const EdgeInsets.all(20),
                    child: Column(
                      mainAxisSize: MainAxisSize.min,
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        const Text(
                          "Radius Filters",
                          style: TextStyle(
                            fontSize: 20,
                            fontWeight: FontWeight.bold,
                          ),
                        ),
                        const SizedBox(height: 20),

                        Text(
                          "Pickup radius: $pickupRadius m",
                          style: const TextStyle(
                            fontSize: 16,
                            fontWeight: FontWeight.w600,
                          ),
                        ),
                        Slider(
                          min: 100,
                          max: 2000,
                          divisions: 19,
                          value: pickupRadius.toDouble(),
                          activeColor: Colors.deepOrange,
                          onChanged: (value) {
                            setState(() => pickupRadius = value.toInt());
                          },
                        ),
                        const SizedBox(height: 15),

                        Text(
                          "Dropoff radius: $dropoffRadius m",
                          style: const TextStyle(
                            fontSize: 16,
                            fontWeight: FontWeight.w600,
                          ),
                        ),
                        Slider(
                          min: 100,
                          max: 2000,
                          divisions: 19,
                          value: dropoffRadius.toDouble(),
                          activeColor: Colors.deepOrange,
                          onChanged: (value) {
                            setState(() => dropoffRadius = value.toInt());
                          },
                        ),
                        const SizedBox(height: 25),

                        Row(
                          mainAxisAlignment: MainAxisAlignment.end,
                          children: [
                            TextButton(
                              onPressed: () => Navigator.pop(dialogContext),
                              child: const Text("Cancel"),
                            ),
                            const SizedBox(width: 10),
                            TextButton(
                              onPressed: () {
                                Navigator.pop(dialogContext);

                                rideListCubit.updateRadiusFilters(
                                  pickupRadius,
                                  dropoffRadius,
                                );
                              },
                              child: const Text(
                                "Apply",
                                style: TextStyle(color: Colors.deepOrange),
                              ),
                            ),
                          ],
                        ),
                      ],
                    ),
                  ),
                );
              },
            );
          },
        );
      },
      style: IconButton.styleFrom(
        backgroundColor: Colors.deepOrange,
      ),
      iconSize: screenWidth * 0.07,
      padding: EdgeInsets.all(screenWidth * 0.018),
      icon: const Icon(Icons.adjust_outlined, color: Colors.white),
    );
  }
}
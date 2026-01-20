import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:flutter_gradient_app_bar_plus/flutter_gradient_app_bar_plus.dart';
import 'package:frontend/domain/models/ride.dart';
import 'package:frontend/features/ride_participants/logic/ride_participant_cubit.dart';
import 'package:frontend/features/rides/data/repository/ride_repository.dart';
import 'package:frontend/features/rides/presentation/widgets/participants_list.dart';
import 'package:frontend/features/rides/presentation/widgets/ride_specs.dart';
import 'package:frontend/features/rides/presentation/widgets/route_info.dart';
import 'package:frontend/shared/widgets/background_colors.dart';
import 'package:frontend/shared/widgets/bg_colors_gradient.dart';
import 'package:frontend/shared/widgets/join_ride_button.dart';

class RideDetailsScreen extends StatefulWidget {
  final int rideId;
  final String? userStatus;

  const RideDetailsScreen({
    super.key,
    required this.rideId,
    this.userStatus,
  });

  @override
  State<RideDetailsScreen> createState() => _RideDetailsScreenState();
}

class _RideDetailsScreenState extends State<RideDetailsScreen> {
  late Future<Ride> _rideFuture;
  final RideRepository _repository = RideRepository();

  @override
  void initState() {
    super.initState();
    _rideFuture = _repository.getRideById(widget.rideId);
  }

  @override
  Widget build(BuildContext context) {
    final h = MediaQuery.of(context).size.height;
    
    return Scaffold(
      appBar: GradientAppBar(
        elevation: 0,
        gradient: bgColorsGradient(Alignment.centerLeft, Alignment.centerRight),
        title: const Text(
          "Ride Details",
          style: TextStyle(fontWeight: FontWeight.bold),
        ),
        centerTitle: true,
      ),
      body: Stack(
        children: [
          Container(
            height: h * 0.5,
            decoration: backgroundColors(
              Alignment.centerLeft,
              Alignment.centerRight,
            ),
          ),

          Container(
            decoration: BoxDecoration(
              gradient: LinearGradient(
                begin: Alignment.topCenter,
                end: Alignment.bottomCenter,
                colors: [
                  Colors.white.withValues(alpha: 0),
                  Colors.white,
                ],
                stops: const [0.1, 0.35],
              ),
            ),
          ),

          FutureBuilder<Ride>(
            future: _rideFuture,
            builder: (context, snapshot) {
              if (snapshot.connectionState == ConnectionState.waiting) {
                return const Center(child: CircularProgressIndicator());
              } else if (snapshot.hasError) {
                return Center(child: Text("Error: ${snapshot.error}"));
              } else if (!snapshot.hasData) {
                return const Center(child: Text("Ride not found"));
              }
          
              final ride = snapshot.data!;
              return SingleChildScrollView(
                padding: const EdgeInsets.all(20),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    routeInfo(ride),
                    const Divider(height: 40),

                    rideSpecs(ride),
                    const SizedBox(height: 30),

                    Padding(
                      padding: const EdgeInsets.symmetric(horizontal: 20, vertical: 5),
                      child: Row(
                        mainAxisAlignment: MainAxisAlignment.center,
                        children: [
                          Expanded(
                            child: joinRideButton(
                              userStatus: widget.userStatus ?? "None",
                              onJoinPressed: () {
                                context.read<RideParticipantCubit>().requestToJoinRide(ride.id);
                              },
                              inDetailsScreen: true,
                            ),
                          ),
                        ],
                      ),
                    ),
                    const SizedBox(height: 30),

                    const Text("Participants", 
                      style: TextStyle(fontSize: 18, fontWeight: FontWeight.bold)),
                    const SizedBox(height: 10),
                    participantsList(ride),

                    if (widget.userStatus == "Organizer") ...[
                      const SizedBox(height: 30),
                      
                      const Text("Rider Requests", 
                        style: TextStyle(fontSize: 18, fontWeight: FontWeight.bold)),
                      const SizedBox(height: 10),
                    ],
                  ],
                ),
              );
            },
          ),
        ]
      ),
    );
  }
}
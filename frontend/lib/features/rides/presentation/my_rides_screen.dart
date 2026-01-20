import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:frontend/domain/models/ride.dart';
import 'package:frontend/features/auth/logic/auth_cubit.dart';
import 'package:frontend/features/home/presentation/widgets/home_app_bar.dart';
import 'package:frontend/features/home/presentation/widgets/ride_card.dart';
import 'package:frontend/features/rides/logic/my_rides_cubit.dart';
import 'package:frontend/features/rides/logic/ride_list_state.dart';
import 'package:frontend/shared/widgets/background_colors.dart';

class MyRidesScreen extends StatefulWidget {
  const MyRidesScreen({super.key});

  @override
  State<MyRidesScreen> createState() => _MyRidesScreenState();
}

class _MyRidesScreenState extends State<MyRidesScreen> {

  @override
  void initState() {
    super.initState();
    WidgetsBinding.instance.addPostFrameCallback((_) {
      if (mounted) {
        context.read<MyRidesCubit>().loadRidesHistory();
      }
    });
  }

  @override
  Widget build(BuildContext context) {
    final screenWidth = MediaQuery.of(context).size.width;
    final screenHeight = MediaQuery.of(context).size.height;

    return BlocBuilder<AuthCubit, AuthState>(
      builder: (context, authState) {
        final user = authState is AuthAuthenticated ? authState.user : null;
    
        return Scaffold(
          appBar: PreferredSize(
            preferredSize: Size.fromHeight(screenHeight * 0.11),
            child: HomeAppBar(user: user),
          ),

          body: RefreshIndicator(
            onRefresh: () => context.read<MyRidesCubit>().loadRidesHistory(),
            child: Stack(
              children: [
                Container(
                  height: 300,
                  decoration: backgroundColors(Alignment.centerLeft, Alignment.centerRight),
                ),
                Container(
                  height: screenHeight,
                  decoration: BoxDecoration(
                    gradient: LinearGradient(
                      begin: Alignment.topCenter,
                      end: Alignment.bottomCenter,
                      colors: [Colors.white.withAlpha(0), Colors.white],
                      stops: const [0.05, 0.2],
                    ),
                  ),
                ),
                
                BlocBuilder<MyRidesCubit, RideListState>(
                  builder: (context, state) {
                    if (state is RideListLoading) {
                      return const Center(
                        child: CircularProgressIndicator()
                      );
                    } else if (state is RideListError) {
                      return Center(
                        child: Text(state.message)
                      );
                    }
                      
                    List<Ride> rides = [];
                    if (state is RideListLoaded) {
                      rides = state.rides;
                    }
                      
                    return ListView.builder(
                      physics: const AlwaysScrollableScrollPhysics(),
                      padding: EdgeInsets.symmetric(
                        horizontal: screenWidth * 0.05,
                        vertical: 20,
                      ),
                      itemCount: rides.isEmpty ? 2 : rides.length + 1,
                      itemBuilder: (context, index) {
                        if (index == 0) {
                          return Padding(
                            padding: const EdgeInsets.only(bottom: 20),
                            child: Text(
                              "Your Rides History",
                              style: TextStyle(
                                fontSize: screenWidth * 0.06,
                                fontWeight: FontWeight.bold,
                                color: Colors.white,
                                shadows: [
                                  Shadow(
                                    offset: const Offset(0, 1),
                                    blurRadius: 0.5,
                                    color: Colors.black.withAlpha(100),
                                  ),
                                ],
                              ),
                            ),
                          );
                        }
                        
                        if (rides.isEmpty) {
                          return Center(
                            child: Padding(
                              padding: EdgeInsets.only(top: 100),
                              child: Text(
                                "You haven't posted or joined any rides yet.",
                                style: TextStyle(
                                  fontSize: 16,
                                  color: Colors.grey.shade600
                                ),
                              ),
                            ),
                          );
                        }
                        
                        final ride = rides[index - 1];
                        return Padding(
                          padding: const EdgeInsets.only(bottom: 15),
                          child: RideCard(
                            id: ride.id,
                            name: ride.organizerName,
                            rating: ride.karma,
                            seatsLeft: ride.availableSeats,
                            from: ride.pickupLocation.address ?? "Unknown",
                            to: ride.dropOffLocation.address ?? "Unknown",
                            time: "Pickup: ${ride.pickupTime.hour}:${ride.pickupTime.minute.toString().padLeft(2, '0')}",
                            price: "\$${ride.pricePerSeat.toStringAsFixed(2)}",
                            imageUrl: ride.imageUrl ?? "https://i.imgur.com/BoN9kdC.png",
                            userStatus: ride.status.displayName == "Completed" ? "Completed" : ride.userStatus,
                            showSeatsLeft: ride.status.displayName != "Completed",
                          ),
                        );
                      },
                    );
                  },
                ),
              ],
            ),
          ),
        );
      },
    );
  }
}
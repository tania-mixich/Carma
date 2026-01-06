import 'dart:ui';

import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:flutter_gradient_app_bar_plus/flutter_gradient_app_bar_plus.dart';
import 'package:frontend/features/auth/logic/auth_cubit.dart';
import 'package:frontend/features/home/presentation/widgets/post_ride_button.dart';
import 'package:frontend/features/home/presentation/widgets/ride_card.dart';
import 'package:frontend/features/home/presentation/widgets/rides_search_bar.dart';
import 'package:frontend/features/rides/logic/ride_list_cubit.dart';
import 'package:frontend/features/rides/logic/ride_list_state.dart';
import 'package:frontend/shared/widgets/background_colors.dart';
import 'package:frontend/shared/widgets/bg_colors_gradient.dart';

class HomeScreen extends StatelessWidget {
  const HomeScreen({super.key});

  String _formatTime(DateTime dateTime) {
    final hour = dateTime.hour;
    final minute = dateTime.minute.toString().padLeft(2, '0');
    final period = hour >= 12 ? 'PM' : 'AM';
    final displayHour = hour > 12 ? hour - 12 : (hour == 0 ? 12 : hour);
    return '$displayHour:$minute $period';
  }

  @override
  Widget build(BuildContext context) {
    final screenWidth = MediaQuery.of(context).size.width;
    final screenHeight = MediaQuery.of(context).size.height;

    return BlocProvider(
      create: (context) => RideListCubit()..initialize(),
      child: BlocBuilder<AuthCubit, AuthState>(
        builder: (context, state) {
          final user = state is AuthAuthenticated ? state.user : null;
      
          return Scaffold(
            backgroundColor: Colors.white,
      
            appBar: PreferredSize(
              preferredSize: Size.fromHeight(screenHeight * 0.11),
              child: GradientAppBar(
                elevation: 0,
                gradient: bgColorsGradient(Alignment.centerLeft, Alignment.centerRight),
                flexibleSpace: 
                  Column(
                    mainAxisAlignment: MainAxisAlignment.start,
                    children: [
                      Container(
                        padding: EdgeInsets.fromLTRB(
                          screenWidth * 0.06,
                          screenHeight * 0.06,
                          screenWidth * 0.06, 
                          screenHeight * 0.001
                        ),
                        child: Row(
                          mainAxisAlignment: MainAxisAlignment.spaceBetween,
                          children: [
                            Row(
                              children: [
                                Card(
                                  elevation: 4,
                                  shape: RoundedRectangleBorder(
                                    borderRadius: BorderRadius.circular(16),
                                  ),
                                  child: ClipRRect(
                                    borderRadius: BorderRadius.circular(16),
                                    child: BackdropFilter(
                                      filter: ImageFilter.blur(sigmaX: 1, sigmaY: 1),
                                      child: Padding(
                                        padding: EdgeInsets.all(screenWidth * 0.02),
                                        child: Icon(
                                          Icons.drive_eta,
                                          color: Colors.deepOrangeAccent,
                                          size: screenWidth * 0.08,
                                        ),
                                      ),
                                    ),
                                  ),
                                ),
                                SizedBox(width: screenWidth * 0.03),
      
                                Text(
                                  "Carma",
                                  style: TextStyle(
                                    color: Colors.white,
                                    fontSize: screenWidth * 0.07,
                                    fontWeight: FontWeight.bold,
                                  ),
                                )
                              ],
                            ),
      
                            Row(
                              children: [
                                Text(
                                  user?.userName ?? "Guest",
                                  style: TextStyle(
                                    color: Colors.white,
                                    fontSize: screenWidth * 0.05,
                                  ),
                                ),
                                SizedBox(width: screenWidth * 0.03),

                                TextButton(
                                  onPressed: () { 
                                    showDialog( 
                                      context: context, 
                                      builder: (dialogContext) => Dialog( 
                                        child: Column( 
                                          mainAxisSize: MainAxisSize.min, 
                                          children: [ 
                                            TextButton( 
                                              onPressed: () => Navigator.pop(dialogContext), 
                                              child: const Text('Cancel'),
                                            ), 
                                            TextButton( 
                                              onPressed: () { 
                                                Navigator.pop(dialogContext); 
                                                context.read<AuthCubit>().logout(); 
                                              }, 
                                              child: const Text( 
                                                'Logout', 
                                                style: TextStyle(color: Colors.red), 
                                              ), 
                                            ), 
                                          ], 
                                        ), 
                                      ) 
                                    ); 
                                  },
                                  child: CircleAvatar(
                                    radius: screenWidth * 0.062,
                                    backgroundImage:
                                        user?.imageUrl != null ? NetworkImage(user!.imageUrl!) : null,
                                    child: user?.imageUrl == null
                                        ? Icon(
                                          Icons.person, 
                                          color: Colors.white,
                                          size: screenWidth * 0.08,
                                        )
                                        : null,
                                  ),
                                ),
                              ],
                            )
                          ],
                        ),
                      ),
                    ],
                  ),
              ),
            ),
      
            body: RefreshIndicator(
              onRefresh: () async {
                await context.read<RideListCubit>().loadNearbyRides();
              },
              child: SingleChildScrollView(
                child: Stack(
                  children: [
                    Container(
                      height: 300,
                      decoration: backgroundColors(Alignment.centerLeft, Alignment.centerRight)
                    ),
                    
                    Container(
                      height: screenHeight,
                      decoration: BoxDecoration(
                        gradient: LinearGradient(
                          begin: Alignment.topCenter,
                          end: Alignment.bottomCenter,
                          colors: [
                            Colors.white.withValues(alpha: 0.0),
                            Colors.white,
                          ],
                          stops: [0.05, 0.2], 
                        ),
                      ),
                    ),
                
                    SafeArea(
                      child: Padding(
                        padding: EdgeInsets.symmetric(horizontal: screenWidth * 0.05),
                        child: Column(
                          crossAxisAlignment: CrossAxisAlignment.start,
                          children: [
                        
                            SizedBox(height: screenHeight * 0.025),
                                      
                            /// SEARCH BAR
                            RidesSearchBar(),
                                      
                            SizedBox(height: screenHeight * 0.035),
                                      
                            /// POST YOUR RIDE BUTTON
                            PostRideButton(),
                                      
                            SizedBox(height: screenHeight * 0.04),
                                      
                            /// ----------------------------------------------------------
                            /// NEARBY RIDES HEADER
                            
                            BlocBuilder<RideListCubit, RideListState>(
                              builder: (context, rideState) {
                                if (rideState is RideListLoading) {
                                  return const Center(
                                    child: Padding(
                                      padding: EdgeInsets.all(50),
                                      child: CircularProgressIndicator(),
                                    ),
                                  );
                                } else if (rideState is RideListError) {
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
                                            rideState.message,
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
                                } else if (rideState is RideListLoaded) {
                                  final rides = rideState.rides;

                                  return Column(
                                    crossAxisAlignment:
                                        CrossAxisAlignment.start,
                                    children: [
                                      /// HEADER
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
                                          Text(
                                            "${rides.length} available",
                                            style: TextStyle(
                                              fontSize: screenWidth * 0.04,
                                              color: Colors.deepOrange,
                                            ),
                                          ),
                                        ],
                                      ),
                                      SizedBox(height: screenHeight * 0.02),

                                      /// RIDES LIST
                                      if (rides.isEmpty)
                                        Center(
                                          child: Padding(
                                            padding: const EdgeInsets.all(40),
                                            child: Column(
                                              children: [
                                                Icon(
                                                  Icons.search_off,
                                                  size: 60,
                                                  color: Colors.grey[400],
                                                ),
                                                const SizedBox(height: 10),
                                                Text(
                                                  'No rides found nearby',
                                                  style: TextStyle(
                                                    fontSize:
                                                        screenWidth * 0.045,
                                                    color: Colors.grey[600],
                                                  ),
                                                ),
                                                const SizedBox(height: 5),
                                                Text(
                                                  'Try adjusting your radius filters',
                                                  style: TextStyle(
                                                    fontSize:
                                                        screenWidth * 0.035,
                                                    color: Colors.grey[500],
                                                  ),
                                                ),
                                              ],
                                            ),
                                          ),
                                        )
                                      else
                                        ...rides.map((ride) {
                                          return Padding(
                                            padding: const EdgeInsets.only(bottom: 15),
                                            child: RideCard(
                                              name: ride.organizerName,
                                              rating: ride.karma,
                                              seatsLeft: ride.availableSeats,
                                              from: ride.pickupLocation.address ?? "Unknown",
                                              to: ride.dropOffLocation.address ?? "Unknown",
                                              time: "Pickup: ${_formatTime(ride.pickupTime)}",
                                              price: "\$${ride.pricePerSeat.toStringAsFixed(2)}",
                                              imageUrl: ride.imageUrl ?? "https://i.imgur.com/BoN9kdC.png",
                                            ),
                                          );
                                        }),

                                      const SizedBox(height: 30),
                                    ],
                                  );
                                }

                                return const SizedBox.shrink();
                              },
                            ),
                          ],
                        ),
                      ),
                    ),
                  ]
                ),
              ),
            ),
          );
        },
      ),
    );
  }
}

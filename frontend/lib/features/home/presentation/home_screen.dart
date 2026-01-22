import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:frontend/features/auth/logic/auth_cubit.dart';
import 'package:frontend/features/home/presentation/widgets/home_app_bar.dart';
import 'package:frontend/features/home/presentation/widgets/nearby_rides_section.dart';
import 'package:frontend/features/home/presentation/widgets/post_ride_button.dart';
import 'package:frontend/features/home/presentation/widgets/rides_search_bar.dart';
import 'package:frontend/features/ride_participants/logic/ride_participant_cubit.dart';
import 'package:frontend/features/ride_participants/logic/ride_participant_state.dart';
import 'package:frontend/features/rides/logic/ride_list_cubit.dart';
import 'package:frontend/shared/widgets/background_colors.dart';

class HomeScreen extends StatelessWidget {
  const HomeScreen({super.key});


  @override
  Widget build(BuildContext context) {
    final screenWidth = MediaQuery.of(context).size.width;
    final screenHeight = MediaQuery.of(context).size.height;

    return BlocProvider(
      create: (context) => RideListCubit()..initialize(),
      child: BlocBuilder<AuthCubit, AuthState>(
        builder: (context, state) {
          final user = state is AuthAuthenticated ? state.user : null;
      
          return BlocListener<RideParticipantCubit, RideParticipantState>(
            listener: (context, participantState) {
              if (participantState is RideParticipantSuccess) {
                if (participantState.rideId != null) {
                  context.read<RideListCubit>().updateRideUserStatus(
                        participantState.rideId!,
                        "Pending",
                      );
                }
                
                context.read<RideParticipantCubit>().reset();
              } else if (participantState is RideParticipantError) {
                ScaffoldMessenger.of(context).showSnackBar(
                  SnackBar(
                    content: Text(participantState.message),
                    backgroundColor: Colors.red,
                  ),
                );
              }
            },
            child: Scaffold(      
              appBar: PreferredSize(
                preferredSize: Size.fromHeight(screenHeight * 0.11),
                child: HomeAppBar(user: user),
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
                                        
                              // SEARCH BAR
                              RidesSearchBar(),
                              SizedBox(height: screenHeight * 0.035),
                                        
                              // POST YOUR RIDE BUTTON
                              PostRideButton(),
                              SizedBox(height: screenHeight * 0.04),
                                        
                              // NEARBY RIDES HEADER
                              NearbyRidesSection(screenWidth: screenWidth, screenHeight: screenHeight),
                            ],
                          ),
                        ),
                      ),
                    ]
                  ),
                ),
              ),
            ),
          );
        },
      ),
    );
  }
}

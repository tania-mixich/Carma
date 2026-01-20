import 'package:flutter/material.dart';
import 'package:frontend/features/rides/presentation/post_ride_screen.dart';

class PostRideButton extends StatelessWidget {
  const PostRideButton({super.key});

  double adaptiveHeight(double screenHeight, double percent) {
    return (screenHeight * percent).clamp(50.0, 70.0);
  }

  @override
  Widget build(BuildContext context) {
    final screenWidth = MediaQuery.of(context).size.width;
    final screenHeight = MediaQuery.of(context).size.height;

    return Container(
      width: double.infinity,
      height: adaptiveHeight(screenHeight, 0.06),
      decoration: BoxDecoration(
        gradient: const LinearGradient(
          colors: [
            Colors.deepOrange, 
            Colors.orangeAccent,
          ],
        ),
        borderRadius: BorderRadius.circular(30),
      ),
      child: TextButton(
        onPressed: () {
          Navigator.of(context).push(
            MaterialPageRoute(
              builder: (_) => const PostRideScreen(),
            ),
          );
        },
        child: Text(
          "+ Post Your Ride",
          style: TextStyle(
            color: Colors.white,
            fontSize: screenWidth * 0.05,
            fontWeight: FontWeight.bold,
          ),
        ),
      ),
    );
  }
}
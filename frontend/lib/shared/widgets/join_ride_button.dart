import 'package:flutter/material.dart';

Widget joinRideButton({required String userStatus, VoidCallback? onJoinPressed, bool? inDetailsScreen}) {
    String label = "Join Ride";
    Color bgColor = Colors.deepOrange;
    bool isDisabled = false;

    switch (userStatus) {
      case "Organizer":
        label = "Your Ride";
        bgColor = const Color.fromARGB(255, 255, 171, 72);
        isDisabled = true;
        break;
      case "Pending":
        label = "Requested";
        bgColor = Colors.amber.shade700;
        isDisabled = true;
        break;
      case "Accepted":
        label = "Joined";
        bgColor = Colors.green;
        isDisabled = true;
        break;
      case "Completed":
        label = "Ride completed!";
        bgColor = const Color.fromARGB(255, 166, 57, 10);
        isDisabled = true;
        break;
      case "None":
      default:
        label = "Join Ride";
        bgColor = Colors.deepOrange;
        isDisabled = false;
        break;
    }

    return ElevatedButton(
      onPressed: isDisabled ? null : onJoinPressed,
      style: ElevatedButton.styleFrom(
        backgroundColor: bgColor,
        disabledBackgroundColor: bgColor.withAlpha(200),
        shape: RoundedRectangleBorder(
          borderRadius: BorderRadius.circular(30),
        ),
        minimumSize: inDetailsScreen == true ? const Size.fromHeight(50) : null,
      ),
      child: Text(
        label,
        style: TextStyle(
          color: Colors.white,
          fontSize: inDetailsScreen == true ? 18 : 16,
        ),
      )
    );
  }
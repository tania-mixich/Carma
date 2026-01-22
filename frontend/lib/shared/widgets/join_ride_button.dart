import 'package:flutter/material.dart';

Widget joinRideButton({
  required String userStatus,
  VoidCallback? onJoinPressed,
  VoidCallback? onChatPressed,
  bool? inDetailsScreen
}) {
    String label = "Join Ride";
    Color bgColor = Colors.deepOrange;
    bool isDisabled = false;
    VoidCallback? activeAction = onJoinPressed;

    switch (userStatus) {
      case "Organizer":
        label = "Ride Chat";
        bgColor = const Color.fromARGB(255, 255, 122, 99);
        isDisabled = false;
        activeAction = onChatPressed;
        break;
      case "Pending":
        label = "Requested";
        bgColor = Colors.amber.shade700;
        isDisabled = true;
        break;
      case "Accepted":
        label = "Ride Chat";
        bgColor = const Color.fromARGB(255, 255, 122, 99);
        isDisabled = false;
        activeAction = onChatPressed;
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
        activeAction = onJoinPressed;
        break;
    }

    return ElevatedButton(
      onPressed: isDisabled ? null : activeAction,
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
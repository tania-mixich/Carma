import 'package:flutter/material.dart';

Widget zoomButton(IconData icon, VoidCallback onTap) {
    return FloatingActionButton(
      shape: RoundedRectangleBorder(
        borderRadius: BorderRadius.circular(20),
      ),
      heroTag: icon.toString(),
      onPressed: onTap,
      backgroundColor: Colors.white,
      child: Icon(
        icon,
        color: Colors.black,
        size: 28,
        fontWeight: FontWeight.w500,
      ),
    );
  }
import 'dart:ui';

import 'package:flutter/material.dart';

class CarmaLogoWidget extends StatelessWidget {
  const CarmaLogoWidget({super.key});

  @override
  Widget build(BuildContext context) {
    final screenWidth = MediaQuery.of(context).size.width;
    final screenHeight = MediaQuery.of(context).size.height;

    return Column(
      children: [
        Card(
          elevation: 10,
          shape: RoundedRectangleBorder(
            borderRadius: BorderRadius.circular(16),
          ),
          child: ClipRRect(
            borderRadius: BorderRadius.circular(16),
            child: BackdropFilter(
              filter: ImageFilter.blur(sigmaX: 1, sigmaY: 1),
              child: Padding(
                padding: EdgeInsets.all(screenWidth * 0.03),
                child: Icon(
                  Icons.drive_eta,
                  color: Colors.deepOrangeAccent,
                  size: screenWidth * 0.12,
                ),
              ),
            ),
          ),
        ),
        SizedBox(height: screenHeight * 0.01),
        Text(
          'Carma',
          style: TextStyle(
            fontSize: screenWidth * 0.08,
            fontWeight: FontWeight.bold,
            color: Colors.white,
          ),
        ),
      ],
    );
  }
}
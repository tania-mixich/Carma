import 'package:flutter/material.dart';

class NoRidesMessage extends StatelessWidget {
  final double screenWidth;
  const NoRidesMessage({super.key, required this.screenWidth});

  @override
  Widget build(BuildContext context) {
    return Center(
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
    );
  }
}
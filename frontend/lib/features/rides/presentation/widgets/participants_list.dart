import 'package:flutter/material.dart';
import 'package:frontend/domain/models/ride.dart';

Widget participantsList(Ride ride) {
  if (ride.participants == null || ride.participants!.isEmpty) {
    return const Text("No participants yet.");
  }

  return ListView.builder(
    shrinkWrap: true,
    physics: const NeverScrollableScrollPhysics(),
    itemCount: ride.participants!.length,
    itemBuilder: (context, index) {
      final participant = ride.participants![index];
      final isOrganizer = participant.isOrganizer;

      final bgColor = isOrganizer
          ? const Color.fromARGB(94, 243, 227, 211)
          : const Color.fromARGB(169, 229, 223, 213);

      final roleColor = isOrganizer
          ? Colors.deepOrange
          : Colors.orange.shade700;

      return Padding(
        padding: const EdgeInsets.only(bottom: 10),
        child: Card(
          elevation: 2,
          shadowColor: Colors.black.withAlpha(60),
          shape: RoundedRectangleBorder(
            borderRadius: BorderRadius.circular(16),
          ),
          child: Container(
            padding: const EdgeInsets.symmetric(horizontal: 14, vertical: 12),
            decoration: BoxDecoration(
              color: bgColor,
              borderRadius: BorderRadius.circular(16),
            ),
            child: Row(
              children: [
                CircleAvatar(
                  radius: 24,
                  backgroundImage: NetworkImage(participant.imageUrl ?? "https://i.imgur.com/BoN9kdC.png"),
                ),

                const SizedBox(width: 12),

                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(
                        participant.name,
                        style: const TextStyle(
                          fontSize: 16,
                          fontWeight: FontWeight.w600,
                        ),
                      ),
                      const SizedBox(height: 4),
                      Container(
                        padding: const EdgeInsets.symmetric(
                          horizontal: 8,
                          vertical: 3,
                        ),
                        decoration: BoxDecoration(
                          color: roleColor.withAlpha(30),
                          borderRadius: BorderRadius.circular(10),
                        ),
                        child: Text(
                          isOrganizer ? "Organizer" : "Passenger",
                          style: TextStyle(
                            fontSize: 12,
                            fontWeight: FontWeight.w500,
                            color: roleColor,
                          ),
                        ),
                      ),
                    ],
                  ),
                ),

                Row(
                  children: [
                    const Icon(
                      Icons.star,
                      color: Color.fromARGB(255, 248, 189, 12),
                      size: 24,
                      shadows: [Shadow(
                        color: Colors.white,
                        offset: Offset(0, 0),
                        blurRadius: 20,
                      )],
                    ),
                    const SizedBox(width: 4),
                    Text(
                      participant.karma.toStringAsFixed(1),
                      style: const TextStyle(
                        fontSize: 15,
                        fontWeight: FontWeight.w500,
                      ),
                    ),
                  ],
                ),
              ],
            ),
          ),
        ),
      );
    },
  );
}
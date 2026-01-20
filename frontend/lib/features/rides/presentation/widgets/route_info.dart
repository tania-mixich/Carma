import 'package:flutter/material.dart';
import 'package:frontend/domain/models/ride.dart';
import 'package:frontend/features/rides/presentation/widgets/glass_card.dart';

Widget routeInfo(Ride ride) {
    return glassCard(
      child: Column(
        children: [
          ListTile(
            leading: const Icon(
              Icons.radio_button_checked,
              color: Colors.green
            ),
            title: Text(ride.pickupLocation.address!),
            subtitle: const Text("Pickup Location"),
          ),
          const Icon(
            Icons.arrow_downward,
            color: Colors.grey
          ),
          ListTile(
            leading: const Icon(
              Icons.place,
              color: Colors.red,
              size: 26,
            ),
            title: Text(ride.dropOffLocation.address!),
            subtitle: const Text("Drop-off Location"),
          ),
        ],
      ),
    );
  }
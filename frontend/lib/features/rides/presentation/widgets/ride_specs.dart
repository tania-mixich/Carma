import 'package:flutter/material.dart';
import 'package:frontend/domain/models/ride.dart';
import 'package:frontend/features/rides/presentation/widgets/info_tile.dart';

Widget rideSpecs(Ride ride) {
    return Row(
      mainAxisAlignment: MainAxisAlignment.spaceAround,
      children: [
        infoTile(Icons.access_time, "Time", "${ride.pickupTime.hour.toString().padLeft(2, '0')}:${ride.pickupTime.minute.toString().padLeft(2, '0')}"),
        infoTile(Icons.event, "Date", "${ride.pickupTime.month}/${ride.pickupTime.day}"),
        infoTile(Icons.attach_money, "Price", "\$${ride.pricePerSeat}"),
        infoTile(Icons.airline_seat_recline_normal, "Seats", "${ride.availableSeats}"),
      ],
    );
  }
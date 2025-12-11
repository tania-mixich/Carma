import 'package:frontend/domain/models/location.dart';

class RideCreate {
  final Location pickupLocation;
  final Location dropOffLocation;
  final DateTime pickupTime;
  final double price;
  final int availableSeats;

  RideCreate({
    required this.pickupLocation,
    required this.dropOffLocation,
    required this.pickupTime,
    required this.price,
    required this.availableSeats,
  });

  Map<String, dynamic> toJson() => {
    'pickupLocation': pickupLocation.toJson(),
    'dropOffLocation': dropOffLocation.toJson(),
    'pickupTime': pickupTime.toIso8601String(),
    'price': price,
    'availableSeats': availableSeats,
  };
}
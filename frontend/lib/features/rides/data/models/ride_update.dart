import 'package:frontend/domain/models/location.dart';

class RideUpdate {
  final Location? pickupLocation;
  final Location? dropOffLocation;
  final DateTime? pickupTime;
  final double? price;
  final int? availableSeats;

  RideUpdate({
    this.pickupLocation,
    this.dropOffLocation,
    this.pickupTime,
    this.price,
    this.availableSeats,
  });

  Map<String, dynamic> toJson() {
    final Map<String, dynamic> data = {};
    
    if (pickupLocation != null) {
      data['pickupLocation'] = pickupLocation!.toJson();
    }
    if (dropOffLocation != null) {
      data['dropOffLocation'] = dropOffLocation!.toJson();
    }
    if (pickupTime != null) {
      data['pickupTime'] = pickupTime!.toIso8601String();
    }
    if (price != null) {
      data['price'] = price;
    }
    if (availableSeats != null) {
      data['availableSeats'] = availableSeats;
    }
    
    return data;
  }
}
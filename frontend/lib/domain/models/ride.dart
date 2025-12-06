import 'package:frontend/domain/enums/status.dart';
import 'package:frontend/domain/models/location.dart';
import 'package:frontend/domain/models/ride_participant.dart';

class Ride {
  final int id;
  final String organizerName;
  final Location pickupLocation;
  final Location dropOffLocation;
  final DateTime pickupTime;
  final double pricePerSeat;
  final int availableSeats;
  final Status status;
  final List<RideParticipant>? participants;

  Ride({
    required this.id,
    required this.organizerName,
    required this.pickupLocation,
    required this.dropOffLocation,
    required this.pickupTime,
    required this.pricePerSeat,
    required this.availableSeats,
    required this.status,
    this.participants,
  });

  // For RideGetDto
  // TODO: maybe pass organizer rating from backend - also to details
  factory Ride.fromJson(Map<String, dynamic> json) {
    return Ride(
      id: json['id'],
      organizerName: json['organizerName'],
      pickupLocation: Location.fromJson(json['pickupLocation']),
      dropOffLocation: Location.fromJson(json['dropOffLocation']),
      pickupTime: DateTime.parse(json['pickupTime']),
      pricePerSeat: (json['pricePerSeat'] ?? 0.0).toDouble(),
      availableSeats: json['availableSeats'] ?? 0,
      status: Status.fromString(json['status']),
    );
  }

  // For RideDetailsDto
  // TODO: pass organizerName to details model from backend
  factory Ride.fromDetailsJson(Map<String, dynamic> json, int id, String organizerName) {
    return Ride(
      id: id,
      organizerName: organizerName,
      pickupLocation: Location.fromJson(json['pickupLocation']),
      dropOffLocation: Location.fromJson(json['dropOffLocation']),
      pickupTime: DateTime.parse(json['pickupTime']),
      pricePerSeat: (json['price'] ?? 0.0).toDouble(),
      availableSeats: json['availableSeats'] ?? 0,
      status: Status.fromString(json['status']),
      participants: (json['participants'] as List?)
          ?.map((p) => RideParticipant.fromJson(p))
          .toList(),
    );
  }

  Map<String, dynamic> toJson() => {
    'id': id,
    'organizerName': organizerName,
    'pickupLocation': pickupLocation.toJson(),
    'dropOffLocation': dropOffLocation.toJson(),
    'pickupTime': pickupTime.toIso8601String(),
    'pricePerSeat': pricePerSeat,
    'availableSeats': availableSeats,
    'status': status.toBackendString(),
    if (participants != null)
      'participants': participants!.map((p) => p.toJson()).toList(),
  };

  Ride copyWith({
    int? id,
    String? organizerName,
    Location? pickupLocation,
    Location? dropOffLocation,
    DateTime? pickupTime,
    double? pricePerSeat,
    int? availableSeats,
    Status? status,
    List<RideParticipant>? participants,
  }) {
    return Ride(
      id: id ?? this.id,
      organizerName: organizerName ?? this.organizerName,
      pickupLocation: pickupLocation ?? this.pickupLocation,
      dropOffLocation: dropOffLocation ?? this.dropOffLocation,
      pickupTime: pickupTime ?? this.pickupTime,
      pricePerSeat: pricePerSeat ?? this.pricePerSeat,
      availableSeats: availableSeats ?? this.availableSeats,
      status: status ?? this.status,
      participants: participants ?? this.participants,
    );
  }

  bool get isAvailable => status == Status.available;
  bool get isFull => status == Status.full;
  bool get isInProgress => status == Status.inProgress;
  bool get isCompleted => status == Status.completed;
  bool get isCancelled => status == Status.cancelled;
  
  bool get hasSeatsAvailable => availableSeats > 0;
    
  RideParticipant? get organizer => 
      participants?.firstWhere((p) => p.isOrganizer, orElse: () => participants!.first);
}
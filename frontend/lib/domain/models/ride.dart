import 'package:frontend/domain/enums/status.dart';
import 'package:frontend/domain/models/location.dart';
import 'package:frontend/domain/models/ride_participant.dart';

class Ride {
  final int id;
  final String organizerName;
  final double karma;
  final String? imageUrl;
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
    this.karma = 0.0,
    this.imageUrl,
    required this.pickupLocation,
    required this.dropOffLocation,
    required this.pickupTime,
    required this.pricePerSeat,
    required this.availableSeats,
    required this.status,
    this.participants,
  });

  // For RideGetDto
  // TODO: pass location address from backend, to avoid extra requests in fe - same for details
  factory Ride.fromJson(Map<String, dynamic> json) {
    return Ride(
      id: json['id'],
      organizerName: json['organizerName'],
      karma: (json['karma'] ?? 0.0).toDouble(),
      imageUrl: ((json['imageUrl'] as String).isEmpty || json['imageUrl'] == null) 
                ? "https://i.imgur.com/BoN9kdC.png" : json['imageUrl'],
      pickupLocation: Location.fromGetJson(json['pickupLocation']),
      dropOffLocation: Location.fromGetJson(json['dropOffLocation']),
      pickupTime: DateTime.parse(json['pickupTime']),
      pricePerSeat: (json['pricePerSeat'] ?? 0.0).toDouble(),
      availableSeats: json['availableSeats'] ?? 0,
      status: Status.fromString(json['status']),
    );
  }

  // For RideDetailsDto
  // TODO: pass organizerName, rating, image to get from participants
  factory Ride.fromDetailsJson(Map<String, dynamic> json, int id, String organizerName) {
    return Ride(
      id: id,
      organizerName: organizerName,
      pickupLocation: Location.fromGetJson(json['pickupLocation']),
      dropOffLocation: Location.fromGetJson(json['dropOffLocation']),
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
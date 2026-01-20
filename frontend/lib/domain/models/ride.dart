import 'package:frontend/domain/enums/status.dart';
import 'package:frontend/domain/models/location.dart';
import 'package:frontend/features/ride_participants/data/models/ride_participant_get.dart';

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
  final List<RideParticipantGet>? participants;
  final String? userStatus;

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
    this.userStatus,
  });

  // For RideGetDto
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
      userStatus: json['userStatus'],
    );
  }

  // For RideDetailsDto
  // TODO: get rating, image (for each participant) from participants list in fe
  factory Ride.fromDetailsJson(Map<String, dynamic> json, int id) {
    List<RideParticipantGet>? rideParticipants = (json['participants'] as List?)
        ?.map((p) => RideParticipantGet.fromJson(p))
        .toList();

    return Ride(
      id: id,
      organizerName: rideParticipants?.firstWhere((p) => p.isOrganizer).name ?? "Unknown",
      pickupLocation: Location.fromGetJson(json['pickupLocation']),
      dropOffLocation: Location.fromGetJson(json['dropOffLocation']),
      pickupTime: DateTime.parse(json['pickupTime']),
      pricePerSeat: (json['price'] ?? 0.0).toDouble(),
      availableSeats: json['availableSeats'] ?? 0,
      status: Status.fromString(json['status']),
      participants: rideParticipants,
    );
  }

  Ride copyWith({
    int? id,
    String? organizerName,
    Location? pickupLocation,
    Location? dropOffLocation,
    DateTime? pickupTime,
    double? pricePerSeat,
    int? availableSeats,
    Status? status,
    List<RideParticipantGet>? participants,
    String? userStatus,
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
      userStatus: userStatus ?? this.userStatus,
    );
  }

  bool get isAvailable => status == Status.available;
  bool get isFull => status == Status.full;
  bool get isInProgress => status == Status.inProgress;
  bool get isCompleted => status == Status.completed;
  bool get isCancelled => status == Status.cancelled;
  
  bool get hasSeatsAvailable => availableSeats > 0;
    
  RideParticipantGet? get organizer => 
      participants?.firstWhere((p) => p.isOrganizer);
}
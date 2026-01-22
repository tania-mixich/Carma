import 'package:frontend/domain/enums/participant_role.dart';

class RideParticipantGet {
  final String userId;
  final String name;
  final String? imageUrl;
  final double karma;
  final ParticipantRole rideRole;

  RideParticipantGet({
    required this.userId,
    required this.name,
    this.imageUrl,
    required this.karma,
    required this.rideRole,
  });

  factory RideParticipantGet.fromJson(Map<String, dynamic> json) {
    return RideParticipantGet(
      userId: json['userId'],
      name: json['participantName'],
      imageUrl: json['participantImageUrl'] ?? "https://i.imgur.com/BoN9kdC.png",
      karma: (json['karma'] ?? 0.0).toDouble(),
      rideRole: ParticipantRole.fromString(json['rideRole']),
    );
  }


  bool get isOrganizer => rideRole == ParticipantRole.organizer;
  bool get isPassenger => rideRole == ParticipantRole.passenger;
}
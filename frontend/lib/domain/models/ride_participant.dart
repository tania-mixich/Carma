import 'package:frontend/domain/enums/participant_role.dart';
import 'package:frontend/domain/enums/participant_status.dart';

class RideParticipant {
  final String userId;
  final int rideId;
  final String userName;
  final String? imageUrl;
  final double karma;
  final ParticipantRole role;
  final ParticipantStatus status;
  final DateTime requestedAt;
  final DateTime? acceptedAt;
  final DateTime? rejectedAt;
  final DateTime? leftAt;

  RideParticipant({
    required this.userId,
    required this.rideId,
    required this.userName,
    this.imageUrl,
    required this.karma,
    required this.role,
    required this.status,
    required this.requestedAt,
    this.acceptedAt,
    this.rejectedAt,
    this.leftAt,
  });

  RideParticipant copyWith({
    String? userId,
    int? rideId,
    String? userName,
    String? imageUrl,
    double? karma,
    ParticipantRole? role,
    ParticipantStatus? status,
    DateTime? requestedAt,
    DateTime? acceptedAt,
    DateTime? rejectedAt,
    DateTime? leftAt,
  }) {
    return RideParticipant(
      userId: userId ?? this.userId,
      rideId: rideId ?? this.rideId,
      userName: userName ?? this.userName,
      imageUrl: imageUrl ?? this.imageUrl,
      karma: karma ?? this.karma,
      role: role ?? this.role,
      status: status ?? this.status,
      requestedAt: requestedAt ?? this.requestedAt,
      acceptedAt: acceptedAt ?? this.acceptedAt,
      rejectedAt: rejectedAt ?? this.rejectedAt,
      leftAt: leftAt ?? this.leftAt,
    );
  }


  bool get isOrganizer => role == ParticipantRole.organizer;
  bool get isPassenger => role == ParticipantRole.passenger;
  
  bool get isPending => status.isPending;
  bool get isAccepted => status.isAccepted;
  bool get isRejected => status.isRejected;
  bool get hasLeft => status.hasLeft;
  
  bool get canChat => isAccepted;
  
  String get statusLabel => status.displayName;
  String get roleLabel => role.displayName;
}
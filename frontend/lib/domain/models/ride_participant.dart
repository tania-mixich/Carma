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

  // For RideParticipantGetDto (simplified view in ride details)
  factory RideParticipant.fromJson(Map<String, dynamic> json) {
    return RideParticipant(
      userId: '', // Not included in GetDto
      rideId: 0, // Not included in GetDto
      userName: json['participantName'] ?? json['userName'],
      imageUrl: json['participantImageUrl'] ?? json['imageUrl'],
      karma: (json['karma'] ?? 0.0).toDouble(),
      role: ParticipantRole.fromString(json['rideRole'] ?? json['role']),
      status: ParticipantStatus.accepted, // Assumed accepted in details view
      requestedAt: DateTime.now(), // Not included in GetDto
    );
  }

  // For full participant data (if you fetch it separately)
  factory RideParticipant.fromFullJson(Map<String, dynamic> json) {
    return RideParticipant(
      userId: json['userId'],
      rideId: json['rideId'],
      userName: json['userName'],
      imageUrl: json['imageUrl'],
      karma: (json['karma'] ?? 0.0).toDouble(),
      role: ParticipantRole.fromString(json['role']),
      status: ParticipantStatus.fromString(json['status']),
      requestedAt: DateTime.parse(json['requestedAt']),
      acceptedAt: json['acceptedAt'] != null 
          ? DateTime.parse(json['acceptedAt']) 
          : null,
      rejectedAt: json['rejectedAt'] != null 
          ? DateTime.parse(json['rejectedAt']) 
          : null,
      leftAt: json['leftAt'] != null 
          ? DateTime.parse(json['leftAt']) 
          : null,
    );
  }

  Map<String, dynamic> toJson() => {
    'userId': userId,
    'rideId': rideId,
    'userName': userName,
    'imageUrl': imageUrl,
    'karma': karma,
    'role': role.toBackendString(),
    'status': status.toBackendString(),
    'requestedAt': requestedAt.toIso8601String(),
    if (acceptedAt != null) 'acceptedAt': acceptedAt!.toIso8601String(),
    if (rejectedAt != null) 'rejectedAt': rejectedAt!.toIso8601String(),
    if (leftAt != null) 'leftAt': leftAt!.toIso8601String(),
  };

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

  // Helper getters
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
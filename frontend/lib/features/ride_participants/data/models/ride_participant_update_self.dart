class RideParticipantUpdateSelf {
  final String status; // "Leave"

  RideParticipantUpdateSelf.leave() : status = 'Leave';

  RideParticipantUpdateSelf({required this.status}) {
    if (status != 'Leave') {
      throw ArgumentError('Status must be "Leave"');
    }
  }

  Map<String, dynamic> toJson() => {
    'status': status,
  };
}
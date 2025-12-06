class RideParticipantUpdate {
  final String status; // "Accepted" or "Rejected"

  RideParticipantUpdate.accept() : status = 'Accepted';
  RideParticipantUpdate.reject() : status = 'Rejected';

  RideParticipantUpdate({required this.status}) {
    if (status != 'Accepted' && status != 'Rejected') {
      throw ArgumentError('Status must be either "Accepted" or "Rejected"');
    }
  }

  Map<String, dynamic> toJson() => {
    'status': status,
  };
}
enum ParticipantStatus {
  accepted,
  rejected,
  left,
  pending;

  static ParticipantStatus fromString(String status) {
    switch (status.toLowerCase()) {
      case 'accepted':
        return ParticipantStatus.accepted;
      case 'rejected':
        return ParticipantStatus.rejected;
      case 'left':
        return ParticipantStatus.left;
      case 'pending':
        return ParticipantStatus.pending;
      default:
        throw ArgumentError('Invalid participant status: $status');
    }
  }

  String toBackendString() {
    switch (this) {
      case ParticipantStatus.accepted:
        return 'Accepted';
      case ParticipantStatus.rejected:
        return 'Rejected';
      case ParticipantStatus.left:
        return 'Left';
      case ParticipantStatus.pending:
        return 'Pending';
    }
  }

  String get displayName {
    switch (this) {
      case ParticipantStatus.accepted:
        return 'Accepted';
      case ParticipantStatus.rejected:
        return 'Rejected';
      case ParticipantStatus.left:
        return 'Left';
      case ParticipantStatus.pending:
        return 'Pending';
    }
  }

  // Helper getters
  bool get isAccepted => this == ParticipantStatus.accepted;
  bool get isRejected => this == ParticipantStatus.rejected;
  bool get hasLeft => this == ParticipantStatus.left;
  bool get isPending => this == ParticipantStatus.pending;
  
}
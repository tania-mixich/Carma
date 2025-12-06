enum ParticipantRole {
  organizer,
  passenger;

  static ParticipantRole fromString(String role) {
    switch (role.toLowerCase()) {
      case 'organizer':
        return ParticipantRole.organizer;
      case 'passenger':
        return ParticipantRole.passenger;
      default:
        throw ArgumentError('Invalid participant role: $role');
    }
  }

  String toBackendString() {
    switch (this) {
      case ParticipantRole.organizer:
        return 'Organizer';
      case ParticipantRole.passenger:
        return 'Passenger';
    }
  }

  String get displayName {
    switch (this) {
      case ParticipantRole.organizer:
        return 'Organizer';
      case ParticipantRole.passenger:
        return 'Passenger';
    }
  }
}
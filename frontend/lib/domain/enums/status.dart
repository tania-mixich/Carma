enum Status {
  available,
  full,
  inProgress,
  completed,
  cancelled;

  static Status fromString(String status) {
    switch (status.toLowerCase()) {
      case 'available':
        return Status.available;
      case 'full':
        return Status.full;
      case 'inprogress':
      case 'in_progress':
        return Status.inProgress;
      case 'completed':
        return Status.completed;
      case 'cancelled':
        return Status.cancelled;
      default:
        throw ArgumentError('Invalid ride status: $status');
    }
  }

  String toBackendString() {
    switch (this) {
      case Status.available:
        return 'Available';
      case Status.full:
        return 'Full';
      case Status.inProgress:
        return 'InProgress';
      case Status.completed:
        return 'Completed';
      case Status.cancelled:
        return 'Cancelled';
    }
  }

  String get displayName {
    switch (this) {
      case Status.available:
        return 'Available';
      case Status.full:
        return 'Full';
      case Status.inProgress:
        return 'In Progress';
      case Status.completed:
        return 'Completed';
      case Status.cancelled:
        return 'Cancelled';
    }
  }
}
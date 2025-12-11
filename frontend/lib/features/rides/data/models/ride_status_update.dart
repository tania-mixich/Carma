
import 'package:frontend/domain/enums/status.dart';

class RideStatusUpdate {
  final Status status;

  RideStatusUpdate({required this.status});

  Map<String, dynamic> toJson() => {
    'status': status.toBackendString(),
  };
}
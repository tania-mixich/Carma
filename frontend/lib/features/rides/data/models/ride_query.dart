class RideQuery {
  final double pickupLatitude;
  final double pickupLongitude;
  final double? dropoffLatitude;
  final double? dropoffLongitude;
  final int pickupRadius;
  final int dropoffRadius;

  RideQuery({
    required this.pickupLatitude,
    required this.pickupLongitude,
    this.dropoffLatitude,
    this.dropoffLongitude,
    this.pickupRadius = 1000,
    this.dropoffRadius = 1000,
  });

  Map<String, dynamic> toQueryParameters() {
    final Map<String, dynamic> params = {
      'pickupLatitude': pickupLatitude,
      'pickupLongitude': pickupLongitude,
      'pickupRadius': pickupRadius,
      'dropoffRadius': dropoffRadius,
    };
    
    if (dropoffLatitude != null) {
      params['dropoffLatitude'] = dropoffLatitude;
    }
    if (dropoffLongitude != null) {
      params['dropoffLongitude'] = dropoffLongitude;
    }
    
    return params;
  }
}
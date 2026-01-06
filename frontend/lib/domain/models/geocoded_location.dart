class GeocodedLocation {
  final double latitude;
  final double longitude;
  final String address;

  GeocodedLocation({
    required this.latitude,
    required this.longitude,
    required this.address,
  });

  String get getAddress => address;
}
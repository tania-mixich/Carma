class Location {
  final double latitude;
  final double longitude;
  final String? address;
  final String? city;
  final String? country;

  Location({
    required this.latitude,
    required this.longitude,
    this.address,
    this.city,
    this.country,
  });

  factory Location.fromCreateJson(Map<String, dynamic> json) {
    return Location(
      latitude: (json['latitude'] ?? 0.0).toDouble(),
      longitude: (json['longitude'] ?? 0.0).toDouble(),
      address: (json['address'] ?? "") as String,
      city: (json['city'] ?? "") as String,
      country: (json['country'] ?? "") as String,
    );
  }

  factory Location.fromGetJson(Map<String, dynamic> json) {
    return Location(
      latitude: (json['latitude'] ?? 0.0).toDouble(),
      longitude: (json['longitude'] ?? 0.0).toDouble(),
      address: json['address'],
      city: json['city'],
      country: json['country'],
    );
  }

  Map<String, dynamic> toJson() => {
    'latitude': latitude,
    'longitude': longitude,
    'address': address ?? "",
    'city': city ?? "",
    'country': country ?? "",
  };

  Location copyWith({
    double? latitude,
    double? longitude,
  }) {
    return Location(
      latitude: latitude ?? this.latitude,
      longitude: longitude ?? this.longitude,
    );
  }

  @override
  String toString() => 'Location(lat: $latitude, long: $longitude)';

  @override
  bool operator ==(Object other) {
    if (identical(this, other)) {
      return true;
    }
    return other is Location &&
        other.latitude == latitude &&
        other.longitude == longitude;
  }

  @override
  int get hashCode => latitude.hashCode ^ longitude.hashCode;
}
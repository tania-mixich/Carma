class MapboxSuggestion {
  final String name;
  final String fullAddress;
  final String mapboxId;

  MapboxSuggestion({
    required this.name,
    required this.fullAddress,
    required this.mapboxId,
  });

  factory MapboxSuggestion.fromJson(Map<String, dynamic> json) {
    return MapboxSuggestion(
      name: json['name'],
      fullAddress: json['full_address'] ?? json['address'] ?? json['place_formatted'] ?? '',
      mapboxId: json['mapbox_id'],
    );
  }
}
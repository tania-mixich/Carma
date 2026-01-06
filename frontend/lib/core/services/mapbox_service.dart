import 'package:dio/dio.dart';
import 'package:frontend/core/utils/mapbox_config.dart';
import 'package:frontend/domain/models/geocoded_location.dart';
import 'package:frontend/features/location/data/models/mapbox_suggestion.dart';

class MapboxService {
  final Dio _dio;

  MapboxService({Dio? dio})
      : _dio = dio ??
            Dio(BaseOptions(
              connectTimeout: const Duration(seconds: 30),
              receiveTimeout: const Duration(seconds: 30),
            ));

  // SEARCH BOX AUTOCOMPLETE WITH SUGGEST
  Future<List<MapboxSuggestion>> getSuggestions({
    required String query,
    required String sessionToken,
    double? proximityLat,
    double? proximityLong,
  }) async {
    if (query.length < 3) return [];

    final response = await _dio.get(
      '${MapboxConfig.searchBaseUrl}/suggest',
      queryParameters: {
        'q': query,
        'access_token': MapboxConfig.accessToken,
        'session_token': sessionToken,
        'limit': 5,
        if (proximityLat != null && proximityLong != null)
          'proximity': '$proximityLong,$proximityLat',
      },
    );

    final suggestions = response.data['suggestions'] as List;
    return suggestions
        .map((s) => MapboxSuggestion.fromJson(s))
        .toList();
  }

  // FORWARD GEOCODING WITH RETRIEVE (address -> lat/long)
  Future<GeocodedLocation?> forwardGeocode({
    required String sessionToken,
    required String mapboxId,
  }) async {
    final response = await _dio.get(
      '${MapboxConfig.searchBaseUrl}/retrieve/$mapboxId',
      queryParameters: {
        'access_token': MapboxConfig.accessToken,
        'session_token': sessionToken,
      },
    );

    final features = response.data['features'] as List;
    if (features.isEmpty) return null;

    final feature = features.first;
    final coordinates = feature['properties']['coordinates'];

    return GeocodedLocation(
      latitude: coordinates['latitude'],
      longitude: coordinates['longitude'],
      address: feature['properties']['address'] ?? feature['properties']['name'] ?? feature['properties']['full_address'],
    );
  }

  // REVERSE GEOCODING (lat/long -> address)
  Future<GeocodedLocation?> reverseGeocode({
    required double longitude,
    required double latitude,
  }) async {
    final response = await _dio.get(
      '${MapboxConfig.geocodingBaseUrl}/reverse',
      queryParameters: {
        'longitude': longitude,
        'latitude': latitude,
        'access_token': MapboxConfig.accessToken,
      },
    );

    final features = response.data['features'] as List;
    if (features.isEmpty) return null;

    final feature = features.first;

    return GeocodedLocation(
      latitude: latitude,
      longitude: longitude,
      address: feature['properties']['address'] ?? feature['properties']['name'] ?? feature['properties']['full_address'],
    );
  }
}
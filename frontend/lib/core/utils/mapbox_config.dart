import 'package:flutter_dotenv/flutter_dotenv.dart';

class MapboxConfig {
  static String accessToken = dotenv.env['MAPBOX_ACCESS_TOKEN'] ?? '';

  static const String searchBaseUrl = 'https://api.mapbox.com/search/searchbox/v1';

  static const String geocodingBaseUrl = 'https://api.mapbox.com/search/geocode/v6';
}
import 'package:flutter_secure_storage/flutter_secure_storage.dart';

class RideQueryStorageService {
  
  static final RideQueryStorageService _instance = RideQueryStorageService._internal();
  factory RideQueryStorageService() => _instance;
  
  RideQueryStorageService._internal();

  final FlutterSecureStorage _storage = const FlutterSecureStorage(
    aOptions: AndroidOptions(
      encryptedSharedPreferences: true,
    ),
    iOptions: IOSOptions(
      accessibility: KeychainAccessibility.first_unlock,
    ),
  );


  static const String _keyPickupLatitude = 'pickup_latitude';
  static const String _keyPickupLongitude = 'pickup_longitude';
  static const String _keyPickupRadius = 'pickup_radius';
  static const String _keyDropoffLatitude = 'dropoff_latitude';
  static const String _keyDropoffLongitude = 'dropoff_longitude';
  static const String _keyDropoffRadius = 'dropoff_radius';


  Future<void> savePickupLocation(double latitude, double longitude) async {
    await _storage.write(key: _keyPickupLatitude, value: latitude.toString());
    await _storage.write(key: _keyPickupLongitude, value: longitude.toString());
  }

  Future<Map<String, double>?> getPickupLocation() async {
    final lat = await _storage.read(key: _keyPickupLatitude);
    final long = await _storage.read(key: _keyPickupLongitude);
    
    if (lat != null && long != null) {
      return {
        'latitude': double.parse(lat),
        'longitude': double.parse(long),
      };
    }
    return null;
  }

  Future<void> saveDropoffLocation(double latitude, double longitude) async {
    await _storage.write(key: _keyDropoffLatitude, value: latitude.toString());
    await _storage.write(key: _keyDropoffLongitude, value: longitude.toString());
  }

  Future<Map<String, double>?> getDropoffLocation() async {
    final lat = await _storage.read(key: _keyDropoffLatitude);
    final long = await _storage.read(key: _keyDropoffLongitude);
    
    if (lat != null && long != null) {
      return {
        'latitude': double.parse(lat),
        'longitude': double.parse(long),
      };
    }
    return null;
  }

  Future<void> clearDropoffLocation() async {
    await _storage.delete(key: _keyDropoffLatitude);
    await _storage.delete(key: _keyDropoffLongitude);
  }

  Future<void> saveRadii(int pickupRadius, int dropoffRadius) async {
    await _storage.write(key: _keyPickupRadius, value: pickupRadius.toString());
    await _storage.write(key: _keyDropoffRadius, value: dropoffRadius.toString());
  }

  Future<int> getPickupRadius() async {
    final radius = await _storage.read(key: _keyPickupRadius);
    return radius != null ? int.parse(radius) : 1000;
  }

  Future<int> getDropoffRadius() async {
    final radius = await _storage.read(key: _keyDropoffRadius);
    return radius != null ? int.parse(radius) : 1000;
  }

  Future<void> clearAll() async {
    await _storage.delete(key: _keyPickupLatitude);
    await _storage.delete(key: _keyPickupLongitude);
    await _storage.delete(key: _keyDropoffLatitude);
    await _storage.delete(key: _keyDropoffLongitude);
    await _storage.delete(key: _keyPickupRadius);
    await _storage.delete(key: _keyDropoffRadius);
  }
}
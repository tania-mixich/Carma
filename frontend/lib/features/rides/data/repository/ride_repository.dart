import 'package:dio/dio.dart';
import 'package:frontend/core/api/api_client.dart';
import 'package:frontend/domain/models/ride.dart';
import 'package:frontend/features/rides/data/models/ride_create.dart';
import 'package:frontend/features/rides/data/models/ride_query.dart';
import 'package:frontend/features/rides/data/models/ride_status_update.dart';
import 'package:frontend/features/rides/data/models/ride_update.dart';

class RideRepository {
  final ApiClient _apiClient;

  RideRepository({ApiClient? apiClient}) : _apiClient = apiClient ?? ApiClient();

  Future<List<Ride>> getAllRides() async {
    try {
      final response = await _apiClient.get('/rides');
      final List<dynamic> ridesJson = response.data;
      return ridesJson.map((json) => Ride.fromJson(json)).toList();
    } on DioException catch (e) {
      throw _handleError(e);
    }
  }

  Future<List<Ride>> getNearbyRides(RideQuery query) async {
    try {
      final response = await _apiClient.get(
        '/rides/nearby',
        queryParameters: query.toQueryParameters(),
      );
      final List<dynamic> ridesJson = response.data;
      return ridesJson.map((json) => Ride.fromJson(json)).toList();
    } on DioException catch (e) {
      throw _handleError(e);
    }
  }

  Future<Ride> getRideById(int rideId) async {
    try {
      final response = await _apiClient.get('/rides/$rideId');
      
      return Ride.fromDetailsJson(
        response.data,
        rideId,
      );
    } on DioException catch (e) {
      throw _handleError(e);
    }
  }

  Future<Ride> createRide(RideCreate request) async {
    try {
      final response = await _apiClient.post(
        '/rides',
        data: request.toJson(),
      );
      return Ride.fromJson(response.data);
    } on DioException catch (e) {
      throw _handleError(e);
    } on Exception catch (e) {
      throw 'An unexpected error occurred: $e';
    }
  }

  Future<Ride> updateRide(int rideId, RideUpdate request) async {
    try {
      final response = await _apiClient.patch(
        '/rides/$rideId',
        data: request.toJson(),
      );
      return Ride.fromJson(response.data);
    } on DioException catch (e) {
      throw _handleError(e);
    }
  }

  Future<Ride> updateRideStatus(int rideId, RideStatusUpdate request) async {
    try {
      final response = await _apiClient.patch(
        '/rides/$rideId/status',
        data: request.toJson(),
      );
      return Ride.fromJson(response.data);
    } on DioException catch (e) {
      throw _handleError(e);
    }
  }

  Future<List<Ride>> getRidesHistory() async {
    try {
      final response = await _apiClient.get(
        '/rides/history',
      );
      
      final List<dynamic> ridesJson = response.data;
      return ridesJson.map((json) => Ride.fromJson(json)).toList();
    } on DioException catch (e) {
      throw _handleError(e);
    }
  }

  String _handleError(DioException error) {
    switch (error.type) {
      case DioExceptionType.connectionTimeout:
      case DioExceptionType.sendTimeout:
      case DioExceptionType.receiveTimeout:
        return 'Connection timeout. Please check your internet connection.';

      case DioExceptionType.badResponse:
        final statusCode = error.response?.statusCode;
        final data = error.response?.data;

        if (statusCode == 400) {
          if (data is Map && data.containsKey('errors')) {
            final errors = data['errors'] as Map;
            final firstError = errors.values.first;
            if (firstError is List && firstError.isNotEmpty) {
              return firstError[0] as String;
            }
          }
          if (data is Map && data.containsKey('message')) {
            return data['message'] as String;
          }
          return 'Invalid request. Please check your input.';
        } else if (statusCode == 401) {
          return 'Unauthorized. Please login again.';
        } else if (statusCode == 404) {
          return 'Ride not found.';
        }
        return 'Server error occurred. Please try again later.';

      case DioExceptionType.cancel:
        return 'Request cancelled';

      default:
        return 'Network error. Please check your connection.';
    }
  }
}
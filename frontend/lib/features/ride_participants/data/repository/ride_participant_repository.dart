import 'package:dio/dio.dart';
import 'package:frontend/core/api/api_client.dart';
import 'package:frontend/features/ride_participants/data/models/ride_participant_get.dart';
import 'package:frontend/features/ride_participants/data/models/ride_participant_update.dart';
import 'package:frontend/features/ride_participants/data/models/ride_participant_update_self.dart';

class RideParticipantRepository {
  final ApiClient _apiClient;

  RideParticipantRepository({ApiClient? apiClient}) 
      : _apiClient = apiClient ?? ApiClient();

  Future<RideParticipantGet> requestToJoinRide(int rideId) async {
    try {
      final response = await _apiClient.post('/rides/$rideId/participants');
      return RideParticipantGet.fromJson(response.data);
    } on DioException catch (e) {
      throw _handleError(e);
    }
  }

  Future<RideParticipantGet?> handleParticipant({
    required int rideId,
    required String userId,
    required RideParticipantUpdate request,
  }) async {
    try {
      final response = await _apiClient.patch(
        '/rides/$rideId/participants/$userId',
        data: request.toJson(),
      );
      
      if (response.data == null) {
        return null;
      }
      
      return RideParticipantGet.fromJson(response.data);
    } on DioException catch (e) {
      throw _handleError(e);
    }
  }

  Future<RideParticipantGet?> acceptParticipant({
    required int rideId,
    required String userId,
  }) async {
    return handleParticipant(
      rideId: rideId,
      userId: userId,
      request: RideParticipantUpdate.accept(),
    );
  }

  Future<void> rejectParticipant({
    required int rideId,
    required String userId,
  }) async {
    await handleParticipant(
      rideId: rideId,
      userId: userId,
      request: RideParticipantUpdate.reject(),
    );
  }

  Future<void> leaveRide(int rideId) async {
    try {
      await _apiClient.patch(
        '/rides/$rideId/participants/me',
        data: RideParticipantUpdateSelf.leave().toJson(),
      );
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
          return 'Invalid request.';
        } else if (statusCode == 401) {
          return 'Unauthorized. You must be logged in.';
        } else if (statusCode == 404) {
          return 'Ride or participant not found.';
        } else if (statusCode == 409) {
          if (data is Map && data.containsKey('message')) {
            return data['message'] as String;
          }
          return 'Conflict. Cannot complete this action.';
        }
        return 'Server error occurred.';

      case DioExceptionType.cancel:
        return 'Request cancelled';

      default:
        return 'Network error. Please check your connection.';
    }
  }
}
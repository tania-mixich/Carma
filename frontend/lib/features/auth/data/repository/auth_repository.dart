import 'package:dio/dio.dart';
import 'package:frontend/core/api/api_client.dart';
import 'package:frontend/domain/models/user.dart';
import 'package:frontend/features/auth/data/models/auth_response.dart';
import 'package:frontend/features/auth/data/models/login_request.dart';
import 'package:frontend/features/auth/data/models/register_request.dart';

class AuthRepository {
  final ApiClient _apiClient;

  AuthRepository({ApiClient? apiClient}) : _apiClient = apiClient ?? ApiClient();

  Future<AuthResponse> login(LoginRequest request) async {
    try {
      final response = await _apiClient.post(
        '/auth/login',
        data: request.toJson(),
      );

      return AuthResponse.fromJson(response.data);
    } on DioException catch (e) {
      throw _handleError(e);
    }
  }

  Future<AuthResponse> register(RegisterRequest request) async {
    try {
      final response = await _apiClient.post(
        '/auth/register',
        data: request.toJson(),
      );

      return AuthResponse.fromJson(response.data);
    } on DioException catch (e) {
      throw _handleError(e);
    }
  }

  Future<User> getCurrentUser(String userId) async {
    try {
      final response = await _apiClient.get('/users/$userId');
      return User.fromSelfJson(response.data);
    } on DioException catch (e) {
      throw _handleError(e);
    }
  }

  Future<void> logout() async {
    try {
      await _apiClient.post('/auth/logout');
      _apiClient.clearAuthToken();
    } on DioException {
      _apiClient.clearAuthToken();
    }
  }

  String _handleError(DioException error) {
    switch (error.type) {
      case DioExceptionType.connectionTimeout:
      case DioExceptionType.sendTimeout:
      case DioExceptionType.receiveTimeout:
        return 'Connection timeout. Check your internet connection.';

      case DioExceptionType.badResponse:
        final statusCode = error.response?.statusCode;
        final data = error.response?.data;

        if (statusCode == 400) {
          if (data is Map) {
            if (data.containsKey('errors')) {
              final errors = data['errors'] as Map;

              final firstError = errors.values.first;
              if (firstError is List && firstError.isNotEmpty) {
                return firstError[0] as String;
              }
            }

            if (data.containsKey('message')) {
              return data['message'] as String;
            }
          }
          return 'Bad request';
        } else if (statusCode == 401) {
          return 'Invalid email or password';
        } else if (statusCode == 409) {
          return 'User already exists';
        } else if (statusCode == 404) {
          return 'Resource not found';
        }
        return 'Server error occurred';

      case DioExceptionType.cancel:
        return 'Request cancelled';

      default:
        return 'Network error. Please try again';
    }
  }

}
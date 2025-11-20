import 'package:equatable/equatable.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:frontend/core/api/api_client.dart';
import 'package:frontend/core/services/storage_service.dart';
import 'package:frontend/core/utils/jwt_helper.dart';
import 'package:frontend/domain/models/user.dart';
import 'package:frontend/features/auth/data/models/login_request.dart';
import 'package:frontend/features/auth/data/models/register_request.dart';
import 'package:frontend/features/auth/data/repository/auth_repository.dart';

abstract class AuthState extends Equatable {

  @override
  List<Object?> get props => [];
}

class AuthInitial extends AuthState {}

class AuthLoading extends AuthState {}

class AuthAuthenticated extends AuthState {
  final String token;
  final User user;

  AuthAuthenticated({
    required this.token,
    required this.user,
  });

  @override
  List<Object?> get props => [token, user];
}

class AuthUnauthenticated extends AuthState {}

class AuthError extends AuthState {
  final String message;

  AuthError(this.message);

  @override
  List<Object?> get props => [message];
}


class AuthCubit extends Cubit<AuthState> {
  final AuthRepository _repository;
  final ApiClient _apiClient;
  final StorageService _storageService;

  AuthCubit({
    AuthRepository? repository,
    ApiClient? apiClient,
    StorageService? storageService,
  }) : _repository = repository ?? AuthRepository(),
      _apiClient = apiClient ?? ApiClient(),
      _storageService = storageService ?? StorageService(),
      super(AuthInitial());

  
  Future<void> checkAuthStatus() async {
    emit(AuthLoading());

    try {
      final token = await _storageService.getAuthToken();

      if (token == null || token.isEmpty) {
        emit(AuthUnauthenticated());
        return;
      }

      if (JwtHelper.isTokenExpired(token)) {
        await _clearAuthData();
        emit(AuthUnauthenticated());
        return;
      }

      final userId = JwtHelper.getUserIdFromToken(token);
      if (userId == null) {
        await _clearAuthData();
        emit(AuthUnauthenticated());
        return;
      }

      _apiClient.setAuthToken(token);

      final user = await _repository.getCurrentUser(userId);

      emit(AuthAuthenticated(token: token, user: user));
    } catch (e) {
      await _clearAuthData();
      emit(AuthUnauthenticated());
    }

  }

  Future<void> login(String email, String password) async {
    emit(AuthLoading());

    try {
      final request = LoginRequest(email: email, password: password);
      final response = await _repository.login(request);

      await _storageService.saveAuthToken(response.token);

      final userId = JwtHelper.getUserIdFromToken(response.token);
      if (userId == null) {
        throw Exception('Invalid token received');
      }

      _apiClient.setAuthToken(response.token);

      final user = await _repository.getCurrentUser(userId);

      await _storageService.saveUserId(user.id);
      await _storageService.saveUserEmail(user.email);
      await _storageService.saveUserName(user.userName);

      emit(AuthAuthenticated(token: response.token, user: user));
    } catch (e) {
      emit(AuthError(e.toString()));
      emit(AuthUnauthenticated());
    }
  }

  Future<void> register(String email, String username, String password, String confirmPassword) async {
    emit(AuthLoading());

    try {
      final request = RegisterRequest(
        email: email, 
        username: username, 
        password: password, 
        confirmPassword: confirmPassword
      );
      final response = await _repository.register(request);

      await _storageService.saveAuthToken(response.token);

      final userId = JwtHelper.getUserIdFromToken(response.token);
      if (userId == null) {
        throw Exception('Invalid token received');
      }
      
      _apiClient.setAuthToken(response.token);

      final user = await _repository.getCurrentUser(userId);

      await _storageService.saveUserId(user.id);
      await _storageService.saveUserEmail(user.email);
      await _storageService.saveUserName(user.userName);

      emit(AuthAuthenticated(token: response.token, user: user));
    } catch (e) {
      emit(AuthError(e.toString()));
      emit(AuthUnauthenticated());
    }
  }

  Future<void> logout() async {
    emit(AuthLoading());

    try {
      await _repository.logout();
    } catch (e) {
      print('Logout API call failed: $e');
    } finally {
      await _clearAuthData();
      emit(AuthUnauthenticated());
    }
  }

  Future<void> _clearAuthData() async {
    await _storageService.clearAll();
    _apiClient.clearAuthToken();
  }

  User? get currentUser {
    final state = this.state;
    if (state is AuthAuthenticated) {
      return state.user;
    }
    return null;
  }

  bool get isAuthenticated => state is AuthAuthenticated;
}
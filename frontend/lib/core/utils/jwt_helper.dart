import 'package:jwt_decoder/jwt_decoder.dart';

class JwtHelper {

  static String? getUserIdFromToken(String token) {
    try {
      Map<String, dynamic> decodedToken = JwtDecoder.decode(token);

      return decodedToken['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'];
    } catch (e) {
      print('Error decoding JWT token: $e');
      return null;
    }
  }

  static String? getEmailFromToken(String token) {
    try {
      Map<String, dynamic> decodedToken = JwtDecoder.decode(token);

      return decodedToken['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'];
    } catch (e) {
      print('Error decoding JWT token: $e');
      return null;
    }
  }

  static String? getUsernameFromToken(String token) {
    try {
      Map<String, dynamic> decodedToken = JwtDecoder.decode(token);

      return decodedToken['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'];
    } catch (e) {
      print('Error decoding JWT token: $e');
      return null;
    }
  }

  static bool isTokenExpired(String token) {
    try {
      return JwtDecoder.isExpired(token);
    } catch (e) {
      return true;
    }
  }

  static DateTime? getExpirationDate(String token) {
    try {
      return JwtDecoder.getExpirationDate(token);
    } catch (e) {
      return null;
    }
  }
}
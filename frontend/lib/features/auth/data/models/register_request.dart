class RegisterRequest {
  final String email;
  final String username;
  final String password;
  final String confirmPassword;

  RegisterRequest({
    required this.email,
    required this.username,
    required this.password,
    required this.confirmPassword,
  });

  Map<String, dynamic> toJson() => {
    'email': email,
    'userName': username,
    'password': password,
    'confirmPassword': confirmPassword,
  };
}
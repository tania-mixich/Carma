class User {
  final String id;
  final String userName;
  final String email;
  final String? imageUrl;
  final double karma;
  final int ridesCount;
  final DateTime createdAt;

  User({
    required this.id,
    required this.userName,
    required this.email,
    this.imageUrl,
    required this.karma,
    required this.ridesCount,
    required this.createdAt,
  });

  factory User.fromSelfJson(Map<String, dynamic> json) {
    return User(
      id: json['id'],
      userName: json['userName'],
      email: json['email'],
      imageUrl: json['imageUrl'],
      karma: (json['karma'] ?? 0.0).toDouble(),
      ridesCount: json['ridesCount'] ?? 0,
      createdAt: DateTime.parse(json['createdAt']),
    );
  }

  factory User.fromProfileJson(Map<String, dynamic> json) {
    return User(
      id: json['id'],
      userName: json['userName'],
      email: '',
      imageUrl: json['imageUrl'],
      karma: (json['karma'] ?? 0.0).toDouble(),
      ridesCount: json['ridesCount'] ?? 0,
      createdAt: DateTime.now(),
    );
  }

  Map<String, dynamic> toJson() => {
    'id': id,
    'userName': userName,
    'email': email,
    'imageUrl': imageUrl,
    'karma': karma,
    'ridesCount': ridesCount,
    'createdAt': createdAt.toIso8601String(),
  };

  User copyWith({
    String? id,
    String? userName,
    String? email,
    String? imageUrl,
    double? karma,
    int? ridesCount,
    DateTime? createdAt,
  }) {
    return User(
      id: id ?? this.id,
      userName: userName ?? this.userName,
      email: email ?? this.email,
      imageUrl: imageUrl ?? this.imageUrl,
      karma: karma ?? this.karma,
      ridesCount: ridesCount ?? this.ridesCount,
      createdAt: createdAt ?? this.createdAt,
    );
  }
}
class Message {
  final String userName;
  final String text;
  final DateTime sentAt;

  Message({required this.userName, required this.text, required this.sentAt});

  factory Message.fromJson(Map<String, dynamic> json) {
    return Message(
      userName: json['userName'],
      text: json['text'],
      sentAt: DateTime.parse(json['sentAt']).toLocal(),
    );
  }
}
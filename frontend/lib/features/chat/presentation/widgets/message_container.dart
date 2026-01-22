import 'package:flutter/material.dart';
import 'package:frontend/domain/models/message.dart';

class MessageContainer extends StatelessWidget {
  final Message message;
  
  const MessageContainer({super.key, required this.message});

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.symmetric(
        horizontal: 14,
        vertical: 10,
      ),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(14),
        boxShadow: [
          BoxShadow(
            color: Colors.black.withAlpha(50),
            blurRadius: 8,
            offset: const Offset(0, 2),
          ),
        ],
      ),
      child: Text(
        message.text,
        style: const TextStyle(fontSize: 16),
      ),
    );
  }
}
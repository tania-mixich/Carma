import 'package:flutter/material.dart';
import 'package:frontend/domain/models/message.dart';
import 'package:frontend/features/chat/presentation/widgets/message_container.dart';

class MessageList extends StatelessWidget {
  final ScrollController scrollController;
  final List<Message> messages;

  const MessageList({
    super.key,
    required this.scrollController,
    required this.messages
  });

  @override
  Widget build(BuildContext context) {
    return Expanded(
      child: ListView.builder(
        controller: scrollController,
        padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 16),
        itemCount: messages.length,
        itemBuilder: (context, index) {
          final m = messages[index];

          return Padding(
            padding: const EdgeInsets.symmetric(vertical: 8, horizontal: 8),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  m.userName,
                  style: TextStyle(
                    fontSize: 16,
                    fontWeight: FontWeight.w600,
                    color: const Color.fromARGB(255, 54, 54, 54),
                  ),
                ),
                const SizedBox(height: 4),

                MessageContainer(message: m),
              ],
            ),
          );
        },
      ),
    );
  }
}
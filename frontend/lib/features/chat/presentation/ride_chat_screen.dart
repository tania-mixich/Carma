import 'dart:async';

import 'package:flutter/material.dart';
import 'package:flutter_gradient_app_bar_plus/flutter_gradient_app_bar_plus.dart';
import 'package:frontend/core/services/chat_realtime_service.dart';
import 'package:frontend/core/services/storage_service.dart';
import 'package:frontend/domain/models/message.dart';
import 'package:frontend/features/chat/data/repository/message_repository.dart';
import 'package:frontend/features/chat/presentation/widgets/message_input.dart';
import 'package:frontend/features/chat/presentation/widgets/message_list.dart';
import 'package:frontend/shared/widgets/background_colors.dart';
import 'package:frontend/shared/widgets/bg_colors_gradient.dart';

class RideChatScreen extends StatefulWidget {
  final int rideId;
  const RideChatScreen({super.key, required this.rideId});

  @override
  State<RideChatScreen> createState() => _RideChatScreenState();
}

class _RideChatScreenState extends State<RideChatScreen> {
  final MessageRepository _repo = MessageRepository();
  final TextEditingController _controller = TextEditingController();
  final ScrollController _scrollController = ScrollController();
  List<Message> _messages = [];

  late final ChatRealtimeService _realtime;
  StreamSubscription<Message>? _subscription;
  final storage = StorageService();

  @override
  void initState() {
    super.initState();

    _loadMessages();

    _realtime = ChatRealtimeService(
      baseUrl: "http://10.0.2.2:5291",
    );

    _initRealtime();
  }

  Future<void> _initRealtime() async {
    try {
      final token = await storage.getAuthToken() ?? "";

      if (!mounted) return;

      await _realtime.connect(
        accessToken: token,
        rideId: widget.rideId,
      );

      _subscription = _realtime.messagesStream.listen((message) {
        if (!mounted) return;

        setState(() {
          final alreadyExists = _messages.any((m) =>
              m.sentAt == message.sentAt &&
              m.userName == message.userName &&
              m.text == message.text);

          if (!alreadyExists) {
            _messages.add(message);
          }
        });

        _scrollToBottom();
      });
    } catch (e) {
      debugPrint("SignalR connection failed: $e");
    }
  }

  Future<void> _loadMessages() async {
    final msgs = await _repo.getMessages(widget.rideId);
    setState(() => _messages = msgs);

    Future.delayed(const Duration(milliseconds: 100), () {
      if (_scrollController.hasClients) {
        _scrollController.jumpTo(_scrollController.position.maxScrollExtent);
      }
    });
  }

  void _send() async {
    if (_controller.text.trim().isEmpty) return;

    final text = _controller.text;
    _controller.clear();

    await _repo.sendMessage(widget.rideId, text);
  }

  void _scrollToBottom() {
    WidgetsBinding.instance.addPostFrameCallback((_) {
      if (_scrollController.hasClients) {
        _scrollController.animateTo(
          _scrollController.position.maxScrollExtent,
          duration: const Duration(milliseconds: 250),
          curve: Curves.easeOut,
        );
      }
    });
  }

  @override
  void dispose() {
    _subscription?.cancel();
    _realtime.disconnect(widget.rideId);
    _controller.dispose();
    _scrollController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    final h = MediaQuery.of(context).size.height;

    return Scaffold(
      backgroundColor: Colors.orange.shade50,
      appBar: GradientAppBar(
        elevation: 0,
        gradient: bgColorsGradient(Alignment.centerLeft, Alignment.centerRight),
        title: const Text(
          "Welcome to the Ride Chat",
          style: TextStyle(fontWeight: FontWeight.w500),
        ),
        centerTitle: true,
      ),
      body: Stack(
        children: [
          Container(
            height: h * 0.5,
            decoration: backgroundColors(
              Alignment.centerLeft,
              Alignment.centerRight,
            ),
          ),
    
          Container(
            decoration: BoxDecoration(
              gradient: LinearGradient(
                begin: Alignment.topCenter,
                end: Alignment.bottomCenter,
                colors: [
                  Colors.orange.shade50.withValues(alpha: 0.2),
                  Colors.orange.shade50,
                ],
                stops: const [0.1, 0.35],
              ),
            ),
          ),

          Column(
            children: [
              if (_messages.isEmpty)
                const Expanded(
                  child: Center(
                    child: Text(
                      "No messages yet. Start the conversation!",
                      style: TextStyle(
                        fontSize: 16,
                        color: Colors.black54,
                      ),
                    ),
                  ),
                )
              else
                MessageList(scrollController: _scrollController, messages: _messages),
          
              MessageInput(messageController: _controller, onSend: _send),
            ],  
          ),
        ]
      ),
    );
  }
}
import 'dart:async';

import 'package:frontend/domain/models/message.dart';
import 'package:signalr_netcore/signalr_client.dart';

class ChatRealtimeService {
  final String baseUrl;
  HubConnection? _connection;

  final StreamController<Message> _messageController =
      StreamController.broadcast();

  Stream<Message> get messagesStream => _messageController.stream;

  ChatRealtimeService({required this.baseUrl});

  Future<void> connect({
    required String accessToken,
    required int rideId,
  }) async {
    _connection = HubConnectionBuilder()
        .withUrl(
          '$baseUrl/carmaHub',
          options: HttpConnectionOptions(
            accessTokenFactory: () async => accessToken,
          ),
        )
        .withAutomaticReconnect()
        .build();

    _connection!.on('ReceiveMessage', (args) {
      if (args == null || args.isEmpty) return;

      final json = args.first as Map<String, dynamic>;
      final message = Message.fromJson(json);

      _messageController.add(message);
    });

    await _connection!.start();

    /// Join ride group
    await _connection!.invoke(
      'JoinRideGroup',
      args: [rideId],
    );
  }

  Future<void> disconnect(int rideId) async {
    if (_connection == null) return;

    await _connection!.invoke(
      'LeaveRideGroup',
      args: [rideId],
    );

    await _connection!.stop();
    await _messageController.close();
  }
}

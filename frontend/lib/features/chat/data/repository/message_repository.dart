import 'package:frontend/core/api/api_client.dart';
import 'package:frontend/domain/models/message.dart';

class MessageRepository {
  final ApiClient _apiClient = ApiClient();

  Future<List<Message>> getMessages(int rideId) async {
    final response = await _apiClient.get('/rides/$rideId/messages');
    return (response.data as List).map((m) => Message.fromJson(m)).toList();
  }

  Future<void> sendMessage(int rideId, String text) async {
    await _apiClient.post('/rides/$rideId/messages', data: {'message': text});
  }
}
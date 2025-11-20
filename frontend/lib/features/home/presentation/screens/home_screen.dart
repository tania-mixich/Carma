import 'package:flutter/material.dart';
import 'package:frontend/core/services/storage_service.dart';

class HomeScreen extends StatelessWidget {
  const HomeScreen({super.key});

  @override
  Widget build(BuildContext context) {
    StorageService storageService = StorageService();

    return Scaffold(
      body: Center(
        child: FutureBuilder<String?>(
          future: storageService.getUserName(),
          builder: (context, snapshot) {
            if (snapshot.connectionState == ConnectionState.waiting) {
              return CircularProgressIndicator();
            }

            final userName = snapshot.data ?? 'User';

            return Text(
              "Welcome, $userName!",
              style: const TextStyle(fontSize: 24),
            );
          }
        ),
      ),
    );
  }
}
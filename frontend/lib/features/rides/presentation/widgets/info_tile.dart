import 'package:flutter/material.dart';

Widget infoTile(IconData icon, String label, String value) {
  return Column(
    children: [
      Icon(icon, color: Colors.deepOrange),
      const SizedBox(height: 4),
      Text(label, style: const TextStyle(fontSize: 14, color: Color.fromARGB(255, 104, 104, 104))),
      Text(value, style: const TextStyle(fontWeight: FontWeight.bold)),
    ],
  );
}
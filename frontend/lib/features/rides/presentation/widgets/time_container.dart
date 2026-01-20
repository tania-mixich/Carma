import 'package:flutter/material.dart';

Widget timeContainer({required String text, required IconData icon}) {
    return Container(
      padding: const EdgeInsets.symmetric(vertical: 14, horizontal: 12),
      decoration: BoxDecoration(
        borderRadius: BorderRadius.circular(14),
        border: Border.all(color: const Color.fromARGB(190, 85, 67, 64)),
      ),
      child: Row(
        children: [
          Icon(icon, color: const Color.fromARGB(225, 85, 67, 64)),
          const SizedBox(width: 18),
          Text(
            text,
            style: const TextStyle(
              fontSize: 16,
              color: Color.fromARGB(245, 85, 67, 64),
            ),
          ),
        ],
      ),
    );
  }
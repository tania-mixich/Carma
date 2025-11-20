import 'package:flutter/material.dart';

BoxDecoration backgroundColors() {
  return BoxDecoration(
    gradient: LinearGradient(
      begin: Alignment.topLeft,
      end: Alignment.bottomRight,
      colors: [
        const Color.fromRGBO(255, 179, 76, 1),
        const Color.fromRGBO(255, 107, 53, 1),
        const Color.fromRGBO(255, 140, 153, 1),
      ],
    ),
  );
}
import 'package:flutter/material.dart';

Gradient bgColorsGradient(Alignment begin, Alignment end) {
  return LinearGradient(
      begin: begin,
      end: end,
      colors: [
        const Color.fromRGBO(255, 179, 76, 1),
        const Color.fromRGBO(255, 107, 53, 1),
        const Color.fromRGBO(255, 140, 153, 1),
      ],
    );
}
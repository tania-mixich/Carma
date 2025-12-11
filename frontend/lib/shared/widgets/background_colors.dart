import 'package:flutter/material.dart';
import 'package:frontend/shared/widgets/bg_colors_gradient.dart';

BoxDecoration backgroundColors(Alignment begin, Alignment end) {
  return BoxDecoration(
    gradient: bgColorsGradient(begin, end),
  );
}
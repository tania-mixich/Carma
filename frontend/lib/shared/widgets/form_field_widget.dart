import 'package:flutter/material.dart';

class FormFieldWidget extends StatefulWidget {
  final TextEditingController controller;
  final bool isLoading;
  final TextInputType keyboardType;
  final String hintText;
  final Widget prefixIcon;
  final String? Function(String?)? validator;
  final bool obscureText;
  final bool showVisibilityToggle;

  const FormFieldWidget({
    super.key,

    required this.controller,
    required this.isLoading,
    required this.keyboardType,
    required this.hintText,
    required this.prefixIcon,
    this.validator,
    this.obscureText = false,
    this.showVisibilityToggle = false,
  });

  @override
  State<FormFieldWidget> createState() => _FormFieldWidgetState();
}

class _FormFieldWidgetState extends State<FormFieldWidget> {

  late bool _obscureText;

  @override
  void initState() {
    super.initState();
    _obscureText = widget.obscureText;
  }

  @override
  Widget build(BuildContext context) {
    final screenWidth = MediaQuery.of(context).size.width;

    final border = OutlineInputBorder(
      borderSide: const BorderSide(
        color: Colors.black38,
        width: 1,
        style: BorderStyle.solid,
      ),
      borderRadius: BorderRadius.circular(12),
    );

    return TextFormField(
      controller: widget.controller,
      enabled: !widget.isLoading,
      keyboardType: widget.keyboardType,
      obscureText: _obscureText,
      style: TextStyle(fontSize: screenWidth * 0.04),
      decoration: InputDecoration(
        hintText: widget.hintText,
        hintStyle: const TextStyle(color: Colors.black54),
        prefixIcon: widget.prefixIcon,
        suffixIcon: widget.showVisibilityToggle
            ? IconButton(
                icon: Icon(
                  _obscureText ? Icons.visibility_off : Icons.visibility,
                ),
                onPressed: () {
                  setState(() {
                    _obscureText = !_obscureText;
                  });
                },
              )
            : null,
        filled: true,
        fillColor: Colors.white24,
        focusedBorder: border,
        enabledBorder: border,
        errorBorder: border.copyWith(
          borderSide: const BorderSide(color: Colors.red),
        ),
        focusedErrorBorder: border.copyWith(
          borderSide: const BorderSide(color: Colors.red),
        ),
      ),
      validator: widget.validator,
    );
  }
}
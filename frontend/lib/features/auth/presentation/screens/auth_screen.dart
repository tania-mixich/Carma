import 'package:flutter/material.dart';
import 'package:frontend/features/auth/presentation/widgets/login_widget.dart';
import 'package:frontend/features/auth/presentation/widgets/signup_widget.dart';
import 'package:frontend/shared/widgets/background_colors.dart';

class AuthScreen extends StatefulWidget {
  const AuthScreen({super.key});

  @override
  State<AuthScreen> createState() => _AuthScreenState();
}

class _AuthScreenState extends State<AuthScreen> {
  bool _showLogin = true;

  void _toggleView() {
    setState(() {
      _showLogin = !_showLogin;
    });
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: Container(
        decoration: backgroundColors(Alignment.topLeft, Alignment.bottomRight),
        child: SafeArea(
          child: LayoutBuilder(
            builder: (context, constraints) {
              return SingleChildScrollView(
                physics: constraints.maxHeight < 800
                    ? const AlwaysScrollableScrollPhysics()
                    : const NeverScrollableScrollPhysics(),
                child: ConstrainedBox(
                  constraints: BoxConstraints(
                    minHeight: constraints.maxHeight,
                  ),
                  child: _showLogin
                      ? LoginWidget(onSignupTap: _toggleView)
                      : SignupWidget(onLoginTap: _toggleView),
                ),
              );
            },
          ),
        ),
      ),
    );
  }
}
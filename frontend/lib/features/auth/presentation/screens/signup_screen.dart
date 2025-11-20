
import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:frontend/features/auth/logic/auth_cubit.dart';
import 'package:frontend/features/auth/presentation/screens/login_screen.dart';
import 'package:frontend/shared/widgets/background_colors.dart';
import 'package:frontend/features/auth/presentation/widgets/signup_widget.dart';

class SignupScreen extends StatelessWidget {
  const SignupScreen({super.key});

  @override
  Widget build(BuildContext context) {

    return BlocProvider(
      create: (context) => AuthCubit(),
      child: Scaffold(
        body: Center(
          child: Container(
            decoration: backgroundColors(),
            child: SafeArea(
              child: LayoutBuilder(
                builder:(context, constraints) {
                  return SingleChildScrollView(
                    physics: constraints.maxHeight < 800
                      ? const AlwaysScrollableScrollPhysics()
                      : const NeverScrollableScrollPhysics(),
                    child: ConstrainedBox(
                      constraints: BoxConstraints(
                        minHeight: constraints.maxHeight,
                      ),
                      child: SignupWidget(
                        onLoginTap: () {
                          Navigator.of(context).pushReplacement(
                            MaterialPageRoute(
                              builder: (context) => const LoginScreen(),
                            ),
                          );
                        },
                      ),
                    ),
                  );
                },
              ),
            ),
          ),
        ),
      ),
    );
  }
}
import 'dart:ui';

import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:frontend/features/auth/logic/auth_cubit.dart';
import 'package:frontend/features/auth/validators/auth_validators.dart';

class LoginWidget extends StatefulWidget {
  final VoidCallback onSignupTap;

  const LoginWidget({
    super.key,
    required this.onSignupTap,
  });

  @override
  State<LoginWidget> createState() => _LoginWidgetState();
}

class _LoginWidgetState extends State<LoginWidget> {
  final _formKey = GlobalKey<FormState>();
  final _emailController = TextEditingController();
  final _passwordController = TextEditingController();
  bool _obscurePassword = true;

  @override
  void dispose() {
    _emailController.dispose();
    _passwordController.dispose();
    super.dispose();
  }

  void _handleLogin() {
    if (_formKey.currentState!.validate()) {
      context.read<AuthCubit>().login(
        _emailController.text.trim(),
        _passwordController.text,
      );
    }
  }

  @override
  Widget build(BuildContext context) {
    final screenWidth = MediaQuery.of(context).size.width;
    final screenHeight = MediaQuery.of(context).size.height;

    final border = OutlineInputBorder(
      borderSide: const BorderSide(
        color: Colors.black38,
        width: 1,
      ),
      borderRadius: BorderRadius.circular(12),
    );

    return BlocConsumer<AuthCubit, AuthState>(
      listener: (context, state) {
        if (state is AuthError) {
          ScaffoldMessenger.of(context).showSnackBar(
            SnackBar(
              content: Text(state.message),
              backgroundColor: Colors.red,
            )
          );
        }
      },

      builder: (context, state) {
        final isLoading = state is AuthLoading;

        return Form(
          key: _formKey,
          child: Column(
            mainAxisAlignment: MainAxisAlignment.center,
            children: [
          
              // Carma Logo
              Card(
                elevation: 10,
                shape: RoundedRectangleBorder(
                  borderRadius: BorderRadius.circular(16),
                ),
                child: ClipRRect(
                  borderRadius: BorderRadius.circular(16),
                  child: BackdropFilter(
                    filter: ImageFilter.blur(sigmaX: 1, sigmaY: 1),
                    child: Padding(
                      padding: EdgeInsets.all(screenWidth * 0.03),
                      child: Column(
                        children: [
                          Icon(
                            Icons.drive_eta,
                            color: Colors.deepOrangeAccent,
                            size: screenWidth * 0.12,
                          ),
                        ],
                      ),
                    ),
                  ),
                ),
              ),
              SizedBox(height: screenHeight * 0.01),
              Text(
                'Carma',
                style: TextStyle(
                  fontSize: screenWidth * 0.08,
                  fontWeight: FontWeight.bold,
                  color: Colors.white,
                ),
              ),
              SizedBox(height: screenHeight * 0.03),
          
              // Login Form
              Card(
                elevation: 10,
                margin: EdgeInsets.symmetric(
                  horizontal: screenWidth * 0.04, 
                  vertical: screenHeight * 0.02
                ),
                color: Colors.white,
                shape: RoundedRectangleBorder(
                  borderRadius: BorderRadius.circular(16),
                ),
                child: ClipRRect(
                  borderRadius: BorderRadius.circular(16),
                  child: Container(
                    padding: EdgeInsets.symmetric(
                      horizontal: screenWidth * 0.06,
                      vertical: screenHeight * 0.03,
                    ),
                    child: Column(
                      children: [
                        Text(
                          'Login',
                          style: TextStyle(
                            fontSize: screenWidth * 0.05,
                            fontWeight: FontWeight.bold,
                          ),
                        ),
          
                        Align(
                          alignment: Alignment.centerLeft,
                          child: Text(
                            'Email',
                            style: TextStyle(
                              fontSize: screenWidth * 0.04, 
                            ),
                          ),
                        ),
                        Container(
                          margin: EdgeInsets.symmetric(
                            vertical: screenHeight * 0.01,
                          ),
                          child: TextFormField(
                            controller: _emailController,
                            enabled: !isLoading,
                            keyboardType: TextInputType.emailAddress,
                            style: TextStyle(fontSize: screenWidth * 0.04),
                            decoration: InputDecoration(
                              hintText: 'Enter your email',
                              hintStyle: const TextStyle(color: Colors.black54),
                              prefixIcon: const Icon(Icons.email_outlined),
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
                            validator: AuthValidators.validateEmail,
                          ),
                        ),
                        SizedBox(height: screenHeight * 0.02),
                    
                        Align(
                          alignment: Alignment.centerLeft,
                          child: Text(
                            'Password',
                            style: TextStyle(
                              fontSize: screenWidth * 0.04, 
                            ),
                          ),
                        ),
                        Container(
                          margin: EdgeInsets.symmetric(
                            vertical: screenHeight * 0.01
                          ),
                          child: TextFormField(
                            controller: _passwordController,
                            enabled: !isLoading,
                            obscureText: _obscurePassword,
                            style: TextStyle(fontSize: screenWidth * 0.04),
                            decoration: InputDecoration(
                              hintText: 'Enter your password',
                              hintStyle: const TextStyle(color: Colors.black54),
                              prefixIcon: const Icon(Icons.lock_open),
                              suffixIcon: IconButton(
                                icon: Icon(
                                  _obscurePassword
                                      ? Icons.visibility_off
                                      : Icons.visibility,
                                ),
                                onPressed: () {
                                  setState(() {
                                    _obscurePassword = !_obscurePassword;
                                  });
                                },
                              ),
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
                            validator: AuthValidators.validatePassword,
                          ),
                        ),
                        SizedBox(height: screenHeight * 0.02),
          
                        ElevatedButton(
                          onPressed: isLoading ? null : _handleLogin,
                          style: ElevatedButton.styleFrom(
                            backgroundColor: Colors.deepOrangeAccent,
                            disabledBackgroundColor: Colors.grey,
                            padding: EdgeInsets.symmetric(
                              horizontal: screenWidth * 0.06,
                              vertical: screenHeight * 0.015,
                            ),
                            shape: RoundedRectangleBorder(
                              borderRadius: BorderRadius.circular(8),
                            ),
                            minimumSize: Size(double.infinity, 1),
                          ),
                          child: isLoading ? 
                            const SizedBox(
                              height: 20,
                              width: 20,
                              child: CircularProgressIndicator(
                                color: Colors.white,
                                strokeWidth: 2,
                              ),
                            )
                            : Text(
                              'Login',
                              style: TextStyle(
                                fontSize: screenWidth * 0.05,
                                color: Colors.white,
                                fontWeight: FontWeight.bold,
                              ),
                            ),
                        ),
          
                      ],
                    ),
                  ),
                ),
              ),
              SizedBox(height: screenHeight * 0.01),
          
              Text(
                'Don\'t have an account? Sign Up',
                style: TextStyle(
                  color: Colors.white,
                  fontSize: screenWidth * 0.04,
                ),
              ),
          
              Padding(
                padding: EdgeInsets.symmetric(
                  horizontal: screenWidth * 0.04,
                  vertical: screenHeight * 0.01
                ),
                child: ElevatedButton(
                  onPressed: isLoading ? null : widget.onSignupTap,
                  style: ElevatedButton.styleFrom(
                    backgroundColor: Colors.white,
                    elevation: 8,
                    padding: EdgeInsets.symmetric(
                      vertical: screenHeight * 0.015,
                    ),
                    shape: RoundedRectangleBorder(
                      borderRadius: BorderRadius.circular(12),
                    ),
                    minimumSize: const Size(double.infinity, 1),
                  ),
                  child: Text(
                    'Create account',
                    style: TextStyle(
                      fontSize: screenWidth * 0.043,
                      color: Colors.deepOrangeAccent,
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                ),
              ),
              
            ],
          ),
        );
      } 
    );
  }
}

import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:frontend/features/auth/logic/auth_cubit.dart';
import 'package:frontend/features/auth/validators/auth_validators.dart';
import 'package:frontend/shared/widgets/carma_logo_widget.dart';
import 'package:frontend/shared/widgets/form_field_widget.dart';
import 'package:frontend/shared/widgets/primary_button.dart';
import 'package:frontend/shared/widgets/secondary_button.dart';

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
              const CarmaLogoWidget(),
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
                          child: FormFieldWidget(
                            controller: _emailController,
                            isLoading: isLoading, 
                            keyboardType: TextInputType.emailAddress, 
                            hintText: 'Enter your email', 
                            prefixIcon: const Icon(Icons.email_outlined),
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
                          child: FormFieldWidget(
                            controller: _passwordController,
                            isLoading: isLoading,
                            keyboardType: TextInputType.visiblePassword,
                            hintText: 'Enter your password', 
                            prefixIcon: const Icon(Icons.lock_open),
                            validator: AuthValidators.validatePassword,
                            obscureText: true,
                            showVisibilityToggle: true,
                          ),
                        ),
                        SizedBox(height: screenHeight * 0.02),
          
                        PrimaryButton(
                          onPressed: _handleLogin, 
                          text: 'Login',
                          isLoading: isLoading,
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
          
              SecondaryButton(
                onPressed: widget.onSignupTap,
                text: 'Create account',
              ),
              
            ],
          ),
        );
      } 
    );
  }
}
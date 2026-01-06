import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:flutter_dotenv/flutter_dotenv.dart';
import 'package:frontend/features/auth/logic/auth_cubit.dart';
import 'package:frontend/features/auth/presentation/screens/auth_screen.dart';
import 'package:frontend/features/home/presentation/screens/home_screen.dart';
import 'package:flutter_radar/flutter_radar.dart';

void main() async {
  WidgetsFlutterBinding.ensureInitialized();
  try {
    await dotenv.load(fileName: ".env");
  } catch (e) {
    throw Exception('Error loading .env file: $e');
  }
  Radar.initialize(dotenv.env['RADAR_TEST_PUBLISHABLE_KEY']!);

  runApp(const Carma());
}

class Carma extends StatelessWidget {
  const Carma({super.key});

  @override
  Widget build(BuildContext context) {
    return BlocProvider(
      create: (context) => AuthCubit()..checkAuthStatus(),
      child: MaterialApp(
        title: 'Carma',
        theme: ThemeData(
          colorScheme: ColorScheme.fromSeed(seedColor: Colors.deepOrangeAccent),
          useMaterial3: true,
        ),
        debugShowCheckedModeBanner: false,
        home: const AuthWrapper(),
        routes: {
          '/home': (context) => const HomeScreen(),
        },
      ),
    );
  }
}


class AuthWrapper extends StatelessWidget {
  const AuthWrapper({super.key});

  @override
  Widget build(BuildContext context) {
    return BlocBuilder<AuthCubit, AuthState>(
      builder: (context, state) {
        if (state is AuthLoading || state is AuthInitial) {
          return const Scaffold(
            body: Center(
              child: CircularProgressIndicator()
            ),
          );
        } else if (state is AuthAuthenticated) {
          Radar.setUserId(state.user.id);
          return const HomeScreen();
        } else {
          return const AuthScreen();
        }
      }
    );
  }
}


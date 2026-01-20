import 'dart:ui';

import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:flutter_gradient_app_bar_plus/flutter_gradient_app_bar_plus.dart';
import 'package:frontend/domain/models/user.dart';
import 'package:frontend/features/auth/logic/auth_cubit.dart';
import 'package:frontend/shared/widgets/bg_colors_gradient.dart';

class HomeAppBar extends StatelessWidget {
  final User? user;
  const HomeAppBar({super.key, this.user});

  @override
  Widget build(BuildContext context) {
    final screenWidth = MediaQuery.of(context).size.width;
    final screenHeight = MediaQuery.of(context).size.height;
    
    return GradientAppBar(
      elevation: 0,
      gradient: bgColorsGradient(Alignment.centerLeft, Alignment.centerRight),
      flexibleSpace: 
        Column(
          mainAxisAlignment: MainAxisAlignment.start,
          children: [
            Container(
              padding: EdgeInsets.fromLTRB(
                screenWidth * 0.06,
                screenHeight * 0.06,
                screenWidth * 0.06, 
                screenHeight * 0.001
              ),
              child: Row(
                mainAxisAlignment: MainAxisAlignment.spaceBetween,
                children: [
                  Row(
                    children: [
                      Card(
                        elevation: 4,
                        shape: RoundedRectangleBorder(
                          borderRadius: BorderRadius.circular(16),
                        ),
                        child: ClipRRect(
                          borderRadius: BorderRadius.circular(16),
                          child: BackdropFilter(
                            filter: ImageFilter.blur(sigmaX: 1, sigmaY: 1),
                            child: Padding(
                              padding: EdgeInsets.all(screenWidth * 0.02),
                              child: Icon(
                                Icons.drive_eta,
                                color: Colors.deepOrangeAccent,
                                size: screenWidth * 0.08,
                              ),
                            ),
                          ),
                        ),
                      ),
                      SizedBox(width: screenWidth * 0.03),
      
                      Text(
                        "Carma",
                        style: TextStyle(
                          color: Colors.white,
                          fontSize: screenWidth * 0.07,
                          fontWeight: FontWeight.bold,
                        ),
                      )
                    ],
                  ),
      
                  Row(
                    children: [
                      Text(
                        user?.userName ?? "Guest",
                        style: TextStyle(
                          color: Colors.white,
                          fontSize: screenWidth * 0.05,
                        ),
                      ),
                      SizedBox(width: screenWidth * 0.03),

                      TextButton(
                        onPressed: () { 
                          showDialog( 
                            context: context, 
                            builder: (dialogContext) => Dialog( 
                              child: Column( 
                                mainAxisSize: MainAxisSize.min, 
                                children: [ 
                                  TextButton( 
                                    onPressed: () => Navigator.pop(dialogContext), 
                                    child: const Text('Cancel'),
                                  ), 
                                  TextButton( 
                                    onPressed: () { 
                                      Navigator.pop(dialogContext); 
                                      context.read<AuthCubit>().logout(); 
                                    }, 
                                    child: const Text( 
                                      'Logout', 
                                      style: TextStyle(color: Colors.red), 
                                    ), 
                                  ), 
                                ], 
                              ), 
                            ) 
                          ); 
                        },
                        child: CircleAvatar(
                          radius: screenWidth * 0.062,
                          backgroundImage:
                              user?.imageUrl != null ? NetworkImage(user!.imageUrl!) : null,
                          child: user?.imageUrl == null
                              ? Icon(
                                Icons.person, 
                                color: Colors.white,
                                size: screenWidth * 0.08,
                              )
                              : null,
                        ),
                      ),
                    ],
                  )
                ],
              ),
            ),
          ],
        ),
    );
  }
}
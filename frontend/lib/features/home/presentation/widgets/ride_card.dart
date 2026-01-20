import 'package:flutter/material.dart';
import 'package:frontend/features/rides/presentation/ride_details_screen.dart';
import 'package:frontend/shared/widgets/join_ride_button.dart';

class RideCard extends StatelessWidget {
  final int id;
  final String name;
  final double rating;
  final int seatsLeft;
  final String from;
  final String to;
  final String time;
  final String price;
  final String imageUrl;
  final String? userStatus;
  final VoidCallback? onJoinPressed;

  final bool showSeatsLeft;

  const RideCard({
    super.key,
    required this.id,
    required this.name,
    required this.rating,
    required this.seatsLeft,
    required this.from,
    required this.to,
    required this.time,
    required this.price,
    required this.imageUrl,
    this.userStatus,
    this.onJoinPressed,
    this.showSeatsLeft = true,
  });

  void _navigateToDetails(BuildContext context) {
    Navigator.push(
      context,
      MaterialPageRoute(
        builder: (context) => RideDetailsScreen(rideId: id, userStatus: userStatus),
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    return Card(
      elevation: 4,
      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(16)),
      child: InkWell(
        borderRadius: BorderRadius.circular(16),
        onTap: () => _navigateToDetails(context),
        child: Padding(
          padding: const EdgeInsets.all(16),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Row(
                children: [
                  CircleAvatar(
                    backgroundImage: NetworkImage(imageUrl),
                    radius: 22,
                  ),
                  const SizedBox(width: 10),
                  Expanded(
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Text(name, style: const TextStyle(fontSize: 16, fontWeight: FontWeight.bold)),
                        Row(
                          children: [
                            const Icon(Icons.star, color: Colors.amber, size: 16),
                            SizedBox(width: 4),
                            Text(rating.toString()),
                          ],
                        ),
                      ],
                    ),
                  ),

                  if (showSeatsLeft) 
                    Container(
                      padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 5),
                      decoration: BoxDecoration(
                        color: Colors.orange.shade100,
                        borderRadius: BorderRadius.circular(20),
                      ),
                      child: Text("$seatsLeft seats left", style: TextStyle(color: Colors.deepOrange)),
                    )
                ],
              ),
        
              const SizedBox(height: 15),
        
              Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Row(children: const [
                    Icon(Icons.circle, size: 10, color: Colors.green),
                    SizedBox(width: 6),
                  ]),
                  Padding(
                    padding: const EdgeInsets.only(left: 18),
                    child: Text(from),
                  ),
                  SizedBox(height: 5),
                  Row(children: const [
                    Icon(Icons.circle, size: 10, color: Colors.red),
                    SizedBox(width: 6),
                  ]),
                  Padding(
                    padding: const EdgeInsets.only(left: 18),
                    child: Text(to),
                  ),
                ],
              ),
        
              const SizedBox(height: 15),
        
              Row(
                children: [
                  Icon(Icons.access_time, size: 18, color: Colors.grey[700]),
                  SizedBox(width: 5),
                  Text(time),
        
                  Spacer(),
        
                  Text(
                    price,
                    style: const TextStyle(
                      color: Colors.green,
                      fontWeight: FontWeight.bold,
                      fontSize: 16,
                    ),
                  ),
                ],
              ),
        
              const SizedBox(height: 15),
        
              Row(
                children: [
                  Expanded(
                    child: joinRideButton(
                      userStatus: userStatus ?? "None",
                      onJoinPressed: onJoinPressed
                    ),
                  ),
                  const SizedBox(width: 10),
                  OutlinedButton(
                    onPressed: () => _navigateToDetails(context),
                    child: const Text("Details"),
                  )
                ],
              )
            ],
          ),
        ),
      ),
    );
  }
}
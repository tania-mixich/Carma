import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:frontend/features/ride_participants/data/models/ride_participant_get.dart';
import 'package:frontend/features/ride_participants/data/repository/ride_participant_repository.dart';
import 'package:frontend/features/ride_participants/logic/ride_participant_cubit.dart';

class RiderRequestsSection extends StatefulWidget {
  final int rideId;

  const RiderRequestsSection({super.key, required this.rideId});

  @override
  State<RiderRequestsSection> createState() => _RiderRequestsSectionState();
}

class _RiderRequestsSectionState extends State<RiderRequestsSection> {
  late Future<List<RideParticipantGet>> _pendingFuture;
  final RideParticipantRepository _repository = RideParticipantRepository();

  @override
  void initState() {
    super.initState();
    _loadRequests();
  }

  void _loadRequests() {
    setState(() {
      _pendingFuture = _repository.getPendingParticipants(widget.rideId);
    });
  }

  @override
  Widget build(BuildContext context) {
    return FutureBuilder<List<RideParticipantGet>>(
      future: _pendingFuture,
      builder: (context, snapshot) {
        if (snapshot.connectionState == ConnectionState.waiting) {
          return const Center(child: CircularProgressIndicator());
        }
        
        final requests = snapshot.data ?? [];
        if (requests.isEmpty) {
          return const Text("No pending requests.", style: TextStyle(color: Colors.grey));
        }

        return ListView.builder(
          shrinkWrap: true,
          physics: const NeverScrollableScrollPhysics(),
          itemCount: requests.length,
          itemBuilder: (context, index) {
            final participant = requests[index];
            return Card(
              margin: const EdgeInsets.symmetric(vertical: 8),
              shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
              child: ListTile(
                leading: CircleAvatar(
                  backgroundImage: participant.imageUrl != null &&
                      participant.imageUrl!.trim().startsWith('http')
                    ? NetworkImage(participant.imageUrl!)
                    : const NetworkImage("https://i.imgur.com/BoN9kdC.png"),
                ),
                title: Text(participant.name, style: const TextStyle(fontWeight: FontWeight.bold)),
                subtitle: Row(
                  children: [
                    const Icon(
                      Icons.star,
                      color: Color.fromARGB(255, 248, 189, 12),
                      size: 24,
                      shadows: [Shadow(
                        color: Colors.white,
                        offset: Offset(0, 0),
                        blurRadius: 20,
                      )],
                    ),
                    const SizedBox(width: 4),
                    Text(
                      participant.karma.toStringAsFixed(1),
                      style: const TextStyle(
                        fontSize: 15,
                        fontWeight: FontWeight.w500,
                      ),
                    ),
                  ],
                ),
                trailing: Row(
                  mainAxisSize: MainAxisSize.min,
                  children: [
                    IconButton(
                      icon: const Icon(
                        Icons.close_rounded,
                        color: Colors.redAccent
                      ),
                      iconSize: 34,
                      onPressed: () => _handleAction(context, participant.userId, false),
                    ),
                    IconButton(
                      icon: const Icon(
                        Icons.check,
                        color: Colors.green
                      ),
                      iconSize: 34,
                      onPressed: () => _handleAction(context, participant.userId, true),
                    ),
                  ],
                ),
              ),
            );
          },
        );
      },
    );
  }

  void _handleAction(BuildContext context, String userId, bool accept) async {
    final cubit = BlocProvider.of<RideParticipantCubit>(context);
    if (accept) {
      await cubit.acceptParticipant(widget.rideId, userId);
    } else {
      await cubit.rejectParticipant(widget.rideId, userId);
    }
    _loadRequests();
  }
}
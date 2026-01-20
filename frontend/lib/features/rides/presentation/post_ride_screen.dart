import 'package:flutter/material.dart';
import 'package:flutter_gradient_app_bar_plus/flutter_gradient_app_bar_plus.dart';
import 'package:frontend/core/services/ride_query_storage_service.dart';
import 'package:frontend/domain/models/location.dart';
import 'package:frontend/features/rides/data/models/ride_create.dart';
import 'package:frontend/features/rides/data/repository/ride_repository.dart';
import 'package:frontend/features/rides/presentation/widgets/glass_card.dart';
import 'package:frontend/features/rides/presentation/widgets/input_field.dart';
import 'package:frontend/features/rides/presentation/widgets/location_field.dart';
import 'package:frontend/features/rides/presentation/widgets/time_container.dart';
import 'package:frontend/shared/widgets/background_colors.dart';
import 'package:frontend/shared/widgets/bg_colors_gradient.dart';

class PostRideScreen extends StatefulWidget {
  const PostRideScreen({super.key});

  @override
  State<PostRideScreen> createState() => _PostRideScreenState();
}

class _PostRideScreenState extends State<PostRideScreen> {
  final _formKey = GlobalKey<FormState>();
  final RideRepository _repository = RideRepository();
  final RideQueryStorageService _storageService = RideQueryStorageService();

  Map<String, double>? currentLocation;

  final _pickupController = TextEditingController();
  final _dropoffController = TextEditingController();
  final _priceController = TextEditingController();
  final _seatsController = TextEditingController();

  DateTime? _pickupTime;

  Location pickupLocation = Location(latitude: 0, longitude: 0);
  Location dropOffLocation = Location(latitude: 0, longitude: 0);

  @override
  void initState() {
    super.initState();
    _loadCurrentLocation();
  }

  Future<void> _loadCurrentLocation() async {
    final location = await _storageService.getPickupLocation();

    if (!mounted) return;

    setState(() {
      currentLocation = location;
    });
  }

  Future<void> _selectPickupTime() async {
    final date = await showDatePicker(
      context: context,
      firstDate: DateTime.now(),
      lastDate: DateTime.now().add(const Duration(days: 10)),
      initialDate: DateTime.now(),
    );
    if (date == null) return;

    final time = await showTimePicker(
      context: context,
      initialTime: TimeOfDay.now(),
    );
    if (time == null) return;

    setState(() {
      _pickupTime = DateTime(
        date.year,
        date.month,
        date.day,
        time.hour,
        time.minute,
      ).toUtc();
    });
  }

  Future<void> _submit() async {
    if (!_formKey.currentState!.validate() || _pickupTime == null) return;

    final ride = RideCreate(
      pickupLocation: pickupLocation,
      dropOffLocation: dropOffLocation,
      pickupTime: _pickupTime!,
      price: double.parse(_priceController.text),
      availableSeats: int.parse(_seatsController.text),
    );

    await _repository.createRide(ride);

    Navigator.pop(context);
  }

  @override
  Widget build(BuildContext context) {
    final w = MediaQuery.of(context).size.width;
    final h = MediaQuery.of(context).size.height;

    return Scaffold(
      backgroundColor: Colors.white,

      appBar: PreferredSize(
        preferredSize: Size.fromHeight(h * 0.1),
        child: GradientAppBar(
          elevation: 0,
          gradient: bgColorsGradient(
            Alignment.centerLeft,
            Alignment.centerRight,
          ),
          title: Column(
            children: [
              SizedBox(height: h * 0.03),
              Text(
                "Post Your Ride",
                style: TextStyle(
                  fontSize: w * 0.055,
                  fontWeight: FontWeight.bold,
                ),
              ),
            ],
          ),
          centerTitle: true,
        ),
      ),

      body: Stack(
        children: [
          Container(
            height: h * 0.3,
            decoration: backgroundColors(
              Alignment.centerLeft,
              Alignment.centerRight,
            ),
          ),

          Container(
            decoration: BoxDecoration(
              gradient: LinearGradient(
                begin: Alignment.topCenter,
                end: Alignment.bottomCenter,
                colors: [
                  Colors.white.withValues(alpha: 0),
                  Colors.white,
                ],
                stops: const [0.1, 0.3],
              ),
            ),
          ),

          SafeArea(
            child: SingleChildScrollView(
              padding: EdgeInsets.all(w * 0.05),
              child: Form(
                key: _formKey,
                child: Column(
                  children: [
                    glassCard(
                      child: Column(
                        children: [
                          LocationField(
                            label: "Pickup Location",
                            controller: _pickupController,
                            onLocationSelected: (location) {
                              pickupLocation = location;
                            },
                            currentLocation: currentLocation ?? pickupLocation.toMap(),
                          ),
                          SizedBox(height: h * 0.02),
                          LocationField(
                            label: "Dropoff Location",
                            controller: _dropoffController,
                            onLocationSelected: (location) {
                              dropOffLocation = location;
                            },
                            currentLocation: currentLocation ?? pickupLocation.toMap(),
                          ),
                        ],
                      ),
                    ),

                    SizedBox(height: h * 0.025),

                    glassCard(
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          Padding(
                            padding: const EdgeInsets.only(bottom: 10),
                            child: Text(
                              "Pickup Time",
                              style: Theme.of(context).textTheme.titleMedium,
                            ),
                          ),
                          InkWell(
                            onTap: _selectPickupTime,
                            child: timeContainer(
                              text: _pickupTime == null
                                  ? "Select date & time"
                                  : "${_pickupTime!.toLocal()}"
                                      .split('.')
                                      .first,
                              icon: Icons.calendar_today,
                            ),
                          ),
                        ],
                      ),
                    ),

                    SizedBox(height: h * 0.025),

                    glassCard(
                      child: Column(
                        children: [
                          inputField(
                            controller: _priceController,
                            label: "Price of ride",
                            icon: Icons.attach_money,
                          ),
                          SizedBox(height: h * 0.02),
                          inputField(
                            controller: _seatsController,
                            label: "Available seats",
                            icon: Icons.event_seat_rounded,
                          ),
                        ],
                      ),
                    ),

                    SizedBox(height: h * 0.04),

                    SizedBox(
                      width: double.infinity,
                      height: h * 0.065,
                      child: ElevatedButton(
                        style: ElevatedButton.styleFrom(
                          backgroundColor: Colors.deepOrange,
                          shape: RoundedRectangleBorder(
                            borderRadius: BorderRadius.circular(30),
                          ),
                        ),
                        onPressed: _submit,
                        child: Text(
                          "Post Ride",
                          style: TextStyle(
                            fontSize: w * 0.05,
                            color: Colors.white,
                            fontWeight: FontWeight.bold,
                          ),
                        ),
                      ),
                    ),
                  ],
                ),
              ),
            ),
          ),
        ],
      ),
    );
  }
}

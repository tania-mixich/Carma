import 'dart:async';

import 'package:flutter/material.dart';
import 'package:flutter_gradient_app_bar_plus/flutter_gradient_app_bar_plus.dart';
import 'package:flutter_radar/flutter_radar.dart';
import 'package:frontend/domain/models/location.dart';
import 'package:frontend/features/location/presentation/widgets/build_location_card.dart';
import 'package:frontend/features/location/presentation/widgets/zoom_button.dart';
import 'package:frontend/shared/widgets/bg_colors_gradient.dart';
import 'package:maplibre_gl/maplibre_gl.dart';
import 'package:flutter_dotenv/flutter_dotenv.dart';


class RadarMapScreen extends StatefulWidget {
  final double latitude;
  final double longitude;
  final String titleText;

  const RadarMapScreen({
    super.key,
    required this.latitude,
    required this.longitude,
    required this.titleText,
  });

  @override
  State<RadarMapScreen> createState() => _RadarMapScreenState();
}

class _RadarMapScreenState extends State<RadarMapScreen> {
  late MapLibreMapController _mapController;
  Location? _selectedLocation;
  bool _isGeocoding = false;
  Timer? _debounce;
  
  CameraPosition? _currentCameraPosition;

  static const String _style = "radar-default-v1";

  @override
  void initState() {
    super.initState();

    _currentCameraPosition = CameraPosition(
      target: LatLng(widget.latitude, widget.longitude),
      zoom: 15,
    );
  }
  
  Future<void> _onCameraIdle() async {
    _debounce?.cancel();

    _debounce = Timer(const Duration(milliseconds: 2000), () async {
      final LatLng center = _mapController.cameraPosition?.target ?? _currentCameraPosition!.target;

      setState(() => _isGeocoding = true);

      try {
        final res = await Radar.reverseGeocode(
          location: {
            'latitude': center.latitude,
            'longitude': center.longitude
          },
        );

        if (res != null && res['addresses'] != null && (res['addresses'] as List).isNotEmpty) {
          final addr = res['addresses'][0];
          String place = addr['placeLabel'] ?? '';
          if (mounted) {
            setState(() {
              _selectedLocation = Location(
                latitude: center.latitude,
                longitude: center.longitude,
                address: place.isNotEmpty ? '$place, ${addr['formattedAddress']}' : addr['formattedAddress'],
                city: addr['city'],
                country: addr['country'],
              );
            });
          }
        }
      } catch (e) {
        debugPrint("Radar geocode error: $e");
      } finally {
        if (mounted) setState(() => _isGeocoding = false);
      }
    });
  }

  void _onZoom(bool zoomIn) {
    _mapController.animateCamera(
      zoomIn ? CameraUpdate.zoomIn() : CameraUpdate.zoomOut(),
    );
  }

  @override
  void dispose() {
    _debounce?.cancel();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: GradientAppBar(
        gradient: bgColorsGradient(
            Alignment.centerLeft,
            Alignment.centerRight,
        ),
        title: Text(
          widget.titleText,
          style: TextStyle(fontWeight: FontWeight.bold),
        ),
        elevation: 0,
      ),
      body: Stack(
        children: [
          MapLibreMap(
            styleString: 'https://api.radar.io/maps/styles/$_style?publishableKey=${dotenv.env['RADAR_TEST_PUBLISHABLE_KEY']}',
            initialCameraPosition: CameraPosition(
              target: LatLng(widget.latitude, widget.longitude),
              zoom: 15,
            ),
            myLocationEnabled: true,
            myLocationTrackingMode: MyLocationTrackingMode.none,
            trackCameraPosition: true,
            onCameraMove: (CameraPosition position) {
              _currentCameraPosition = position;
            },
            onCameraIdle: _onCameraIdle,
            onMapCreated: (controller) {
              _mapController = controller;
              _onCameraIdle();
            },
          ),

          IgnorePointer(
            child: Center(
              child: Padding(
                padding: const EdgeInsets.only(bottom: 35),
                child: Icon(
                  Icons.location_on,
                  size: 45,
                  color: Colors.deepOrange,
                ),
              ),
            ),
          ),

          Positioned(
            right: 16,
            top: 40,
            child: Column(
              children: [
                zoomButton(Icons.add, () => _onZoom(true)),
                const SizedBox(height: 12),
                zoomButton(Icons.remove, () => _onZoom(false)),
              ],
            ),
          ),

          Positioned(
            bottom: 30,
            left: 16,
            right: 16,
            child: buildLocationCard(_selectedLocation, _isGeocoding, context),
          ),
        ],
      ),
    );
  }
}
import 'dart:async';

import 'package:flutter/material.dart';
import 'package:frontend/domain/models/location.dart';
import 'package:flutter_radar/flutter_radar.dart';
import 'package:frontend/features/location/presentation/radar_map_screen.dart';

class LocationField extends StatefulWidget {
  final String label;
  final TextEditingController controller;
  final void Function(Location location) onLocationSelected;
  final Map<String, double> currentLocation;

  const LocationField({
    super.key,
    required this.label,
    required this.controller,
    required this.onLocationSelected,
    required this.currentLocation,
  });

  @override
  State<LocationField> createState() => _LocationFieldState();
}

class _LocationFieldState extends State<LocationField> {
  List<dynamic> _suggestions = [];
  bool _isLoading = false;
  Timer? _debounce;

  Future<void> _search(String query) async {
    _debounce?.cancel();

    if (query.isEmpty) {
      setState(() => _suggestions = []);
      return;
    }

    _debounce = Timer(const Duration(milliseconds: 2000), () async {
      if (query.trim().length < 3) {
        setState(() => _suggestions = []);
        return;
      }

      setState(() => _isLoading = true);

      try {
        final res = await Radar.autocomplete(
          query: query,
          near: {
            'latitude': widget.currentLocation['latitude'],
            'longitude': widget.currentLocation['longitude']
          },
          limit: 5,
          layers: ['place', 'address']
        );

        print('Radar autocomplete response: $res');

        if (res != null && res.isNotEmpty) {
          setState(() {
            _suggestions = res['addresses'] ?? [];
          });
        }
      } catch (e) {
        debugPrint("Radar autocomplete error: $e");
      } finally {
        setState(() => _isLoading = false);
      }
    });
  }

  void _selectAddress(Map<Object?, Object?> address) {
    if (address['placeLabel'] != null && address['placeLabel'] != '') {
      widget.controller.text = address['placeLabel'] as String;
      widget.controller.text += ', ${address['formattedAddress']}';
    } else {
      widget.controller.text = address['formattedAddress'] as String? ?? '';
    }

    final location = Location(
      latitude: address['latitude'] as double,
      longitude: address['longitude'] as double,
      address: widget.controller.text,
      city: address['city'] as String?,
      country: address['country'] as String?,
    );
    
    widget.onLocationSelected(location);

    setState(() => _suggestions = []);
    FocusScope.of(context).unfocus();
  }

  @override
  Widget build(BuildContext context) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          widget.label,
          style: Theme.of(context).textTheme.titleMedium,
        ),
        const SizedBox(height: 8),

        Stack(
          children: [
            TextFormField(
              controller: widget.controller,
              minLines: 1,
              maxLines: 2,
              decoration: InputDecoration(
                hintText: "Enter address",
                prefixIcon: const Icon(Icons.location_on_outlined),
                contentPadding: const EdgeInsets.only(
                  left: 12,
                  right: 56,
                  top: 16,
                  bottom: 16,
                ),
                border: OutlineInputBorder(
                  borderRadius: BorderRadius.circular(14),
                ),
              ),
              validator: (v) =>
                  v == null || v.isEmpty ? "Required" : null,
              onChanged: _search,
            ),

            if (_isLoading)
              const Positioned(
                right: 48,
                top: 18,
                child: SizedBox(
                  width: 18,
                  height: 18,
                  child: CircularProgressIndicator(strokeWidth: 2),
                ),
              ),

            Positioned(
              right: 8,
              top: 4,
              bottom: 4,
              child: IconButton(
                icon: const Icon(Icons.map_outlined),
                onPressed: () async {
                  final result = await Navigator.push<Location>(
                    context,
                    MaterialPageRoute(
                      builder: (_) => RadarMapScreen(
                        latitude: widget.currentLocation['latitude']!,
                        longitude: widget.currentLocation['longitude']!,
                        titleText: 'Select ${widget.label}',
                      ),
                    ),
                  );
              
                  if (result != null) {
                    widget.controller.text = result.address ?? '';
                    widget.onLocationSelected(result);
                  }
                },
              ),
            ),
          ],
        ),

        if (_suggestions.isNotEmpty)
          Container(
            margin: const EdgeInsets.only(top: 6),
            decoration: BoxDecoration(
              color: Colors.white,
              borderRadius: BorderRadius.circular(14),
              boxShadow: [
                BoxShadow(
                  color: Colors.black.withAlpha(30),
                  blurRadius: 12,
                ),
              ],
            ),
            child: Column(
              children: _suggestions.map((address) {
                return ListTile(
                  leading: const Icon(
                    Icons.place,
                    color: Colors.deepOrange,
                  ),
                  title: Text(address['formattedAddress'] ?? ''),
                  subtitle: Text(
                    [
                      address['placeLabel'],
                      address['city'],
                      address['country'],
                    ].where((e) => e != null && e.isNotEmpty).join(', '),
                  ),
                  onTap: () => _selectAddress(address),
                );
              }).toList(),
            ),
          ),
      ],
    );
  }
}

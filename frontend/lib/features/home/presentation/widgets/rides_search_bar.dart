
import 'dart:async';

import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:frontend/core/services/mapbox_service.dart';
import 'package:frontend/features/auth/logic/auth_cubit.dart';
import 'package:frontend/features/home/presentation/widgets/radius_filters_button.dart';
import 'package:frontend/features/location/data/models/mapbox_suggestion.dart';
import 'package:frontend/features/rides/logic/ride_list_cubit.dart';

class RidesSearchBar extends StatefulWidget {
  const RidesSearchBar({super.key});

  @override
  State<RidesSearchBar> createState() => _RidesSearchBarState();
}

class _RidesSearchBarState extends State<RidesSearchBar> {
  final MapboxService _mapboxService = MapboxService();
  final _addressController = TextEditingController();
  Timer? _debounce;

  List<MapboxSuggestion> _suggestions = [];
  bool _loading = false;

  String get _sessionToken =>
      context.read<AuthCubit>().state is AuthAuthenticated
          ? (context.read<AuthCubit>().state as AuthAuthenticated).user.id
          : 'guest-session';

  Future<void> _onTextChanged(String searchQuery) async {
    _debounce?.cancel();

    if (searchQuery.isEmpty) {
      context.read<RideListCubit>().updateDropoffLocation(null, null);
      setState(() => _suggestions = []);
      return;
    }

    _debounce = Timer(const Duration(milliseconds: 2000), () async {
      if (searchQuery.length < 3) {
        setState(() => _suggestions = []);
        return;
      }

      setState(() => _loading = true);

      try {
        final results = await _mapboxService.getSuggestions(
          query: searchQuery,
          sessionToken: _sessionToken,
        );

        if (!mounted) return;

        setState(() {
          _suggestions = results;
          _loading = false;
        });
      } catch (_) {
        if (!mounted) return;

        setState(() => _loading = false);
      }
    });
  }

  Future<void> _selectSuggestion(MapboxSuggestion suggestion) async {
    final location = await _mapboxService.forwardGeocode(
      sessionToken: _sessionToken,
      mapboxId: suggestion.mapboxId
    );

    if (location != null && mounted) {
      context.read<RideListCubit>().updateDropoffLocation(
        location.latitude,
        location.longitude,
      );

      _addressController.text = suggestion.name;
    }

    setState(() => _suggestions = []);
    FocusScope.of(context).unfocus();
  }

  double adaptiveHeight(double screenHeight, double percent) {
    return (screenHeight * percent).clamp(50.0, 70.0);
  }

  @override
  void dispose() {
    _debounce?.cancel();
    _addressController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    final screenWidth = MediaQuery.of(context).size.width;
    final screenHeight = MediaQuery.of(context).size.height;

    return Column(
      children: [
        Container(
          padding: EdgeInsets.symmetric(horizontal: screenWidth * 0.04),
          height: adaptiveHeight(screenHeight, 0.06),
          decoration: BoxDecoration(
            color: Colors.white,
            borderRadius: BorderRadius.circular(30),
            boxShadow: const [
              BoxShadow(
                color: Colors.black12,
                blurRadius: 6,
                offset: Offset(0, 2),
              )
            ],
          ),
          child: Row(
            children: [
              Icon(Icons.search, size: screenWidth * 0.07, color: Colors.grey),
              SizedBox(width: screenWidth * 0.02),

              Expanded(
                child: TextField(
                  controller: _addressController,
                  keyboardType: TextInputType.streetAddress,
                  decoration: const InputDecoration(
                    hintText: "Where are you going?",
                    border: InputBorder.none,
                  ),
                  onChanged: _onTextChanged,
                ),
              ),

              RadiusFiltersButton(),
            ],
          ),
        ),

        if (_loading)
          const Padding(
            padding: EdgeInsets.all(8),
            child: LinearProgressIndicator(minHeight: 2),
          ),

        if (_suggestions.isNotEmpty)
          Container(
            margin: const EdgeInsets.only(top: 6),
            decoration: BoxDecoration(
              color: Colors.white,
              borderRadius: BorderRadius.circular(16),
              boxShadow: const [
                BoxShadow(
                  color: Colors.black12,
                  blurRadius: 8,
                  offset: Offset(0, 4),
                ),
              ],
            ),
            child: ListView.separated(
              shrinkWrap: true,
              physics: const NeverScrollableScrollPhysics(),
              itemCount: _suggestions.length,
              separatorBuilder: (_, __) => const Divider(height: 1),
              itemBuilder: (context, index) {
                final suggestion = _suggestions[index];

                return ListTile(
                  leading: const Icon(Icons.location_on_outlined),
                  title: Text(suggestion.name),
                  subtitle: Text(
                    suggestion.fullAddress,
                    maxLines: 1,
                    overflow: TextOverflow.ellipsis,
                  ),
                  onTap: () => _selectSuggestion(suggestion),
                );
              },
            ),
          ),
      ],
    );
  }
}
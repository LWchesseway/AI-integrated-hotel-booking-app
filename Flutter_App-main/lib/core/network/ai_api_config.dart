import 'package:flutter/foundation.dart';

class AiApiConfig {
  static String get baseUrl {
    const configuredBaseUrl = String.fromEnvironment('AI_API_BASE_URL');
    if (configuredBaseUrl.isNotEmpty) {
      return configuredBaseUrl;
    }

    if (kIsWeb) {
      return 'http://localhost:8000';
    }

    if (defaultTargetPlatform == TargetPlatform.android) {
      return 'http://10.0.2.2:8000';
    }

    return 'http://localhost:8000';
  }
}

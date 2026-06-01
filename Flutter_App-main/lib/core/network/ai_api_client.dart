import 'dart:convert';
import 'package:http/http.dart' as http;
import 'ai_api_config.dart';

class AiApiException implements Exception {
  final String message;
  final int statusCode;

  const AiApiException(this.message, this.statusCode);

  @override
  String toString() => 'AiApiException($statusCode): $message';
}

class AiApiClient {
  AiApiClient({http.Client? client}) : _client = client ?? http.Client();

  final http.Client _client;

  Future<Map<String, dynamic>> get(
    String path, {
    Map<String, dynamic>? query,
    String? accessToken,
  }) async {
    final uri = _buildUri(path, query);
    final response = await _client.get(uri, headers: _headers(accessToken));
    return _handleResponse(response);
  }

  Future<Map<String, dynamic>> post(
    String path, {
    Map<String, dynamic>? body,
    Map<String, dynamic>? query,
    String? accessToken,
  }) async {
    final uri = _buildUri(path, query);
    final response = await _client.post(
      uri,
      headers: _headers(accessToken),
      body: jsonEncode(body ?? <String, dynamic>{}),
    );
    return _handleResponse(response);
  }

  Uri _buildUri(String path, Map<String, dynamic>? query) {
    final raw = '${AiApiConfig.baseUrl}$path';
    final uri = Uri.parse(raw);
    if (query == null || query.isEmpty) return uri;

    final queryParams = <String, String>{};
    for (final entry in query.entries) {
      if (entry.value != null) {
        queryParams[entry.key] = entry.value.toString();
      }
    }
    return uri.replace(queryParameters: queryParams);
  }

  Map<String, String> _headers(String? accessToken) {
    final headers = <String, String>{
      'Content-Type': 'application/json',
      'Accept': 'application/json',
    };
    if (accessToken != null && accessToken.isNotEmpty) {
      headers['Authorization'] = 'Bearer $accessToken';
    }
    return headers;
  }

  Map<String, dynamic> _handleResponse(http.Response response) {
    final decoded = _tryDecode(response.body);
    if (response.statusCode >= 200 && response.statusCode < 300) {
      if (decoded is Map<String, dynamic>) return decoded;
      return {
        'success': true,
        'statusCode': response.statusCode,
        'data': decoded,
      };
    }
    final message = _extractMessage(decoded) ?? 'Request failed';
    throw AiApiException(message, response.statusCode);
  }

  dynamic _tryDecode(String body) {
    if (body.isEmpty) return null;
    try {
      return jsonDecode(body);
    } catch (_) {
      return null;
    }
  }

  String? _extractMessage(dynamic decoded) {
    if (decoded is Map<String, dynamic>) {
      final detail = decoded['detail'];
      if (detail is String && detail.trim().isNotEmpty) return detail;
      final message = decoded['message'];
      if (message is String && message.trim().isNotEmpty) return message;
      final title = decoded['title'];
      if (title is String && title.trim().isNotEmpty) return title;
      final errors = decoded['errors'];
      if (errors is List && errors.isNotEmpty) return errors.join(', ');
      if (errors is Map && errors.isNotEmpty) {
        return errors.values
            .expand((value) => value is List ? value : [value])
            .whereType<Object>()
            .join(', ');
      }
    }
    return null;
  }
}

import '../../../../../core/network/api_client.dart';
import '../../../../../core/storage/auth_storage.dart';
import '../../../domain/entities/notification_entity.dart';

class NotificationRemoteDataSource {
  final ApiClient _client;
  final AuthStorage _authStorage = AuthStorage();

  NotificationRemoteDataSource({ApiClient? client}) : _client = client ?? ApiClient();

  Future<List<NotificationEntity>> getNotifications({
    int pageIndex = 1,
    int pageSize = 20,
  }) async {
    final token = await _authStorage.getAccessToken();
    final session = await _authStorage.getSession();
    final currentUserId = session?.userId;
    final response = await _client.get(
      '/api/notifications',
      query: {'pageIndex': pageIndex, 'pageSize': pageSize},
      accessToken: token,
    );

    // Check if the structure contains "data" array or it's directly an array
    final data = response['data'] ?? response;
    if (data is! List) return <NotificationEntity>[];

    final items = data
        .whereType<Map<String, dynamic>>()
        .map(NotificationEntity.fromJson)
        .toList();

    if (currentUserId == null) return items;
    return items.where((n) => n.userId == currentUserId).toList();
  }

  Future<void> markAsRead(int notificationId) async {
    final token = await _authStorage.getAccessToken();
    final detail = await _client.get(
      '/api/notifications/$notificationId',
      accessToken: token,
    );
    final data = detail['data'];
    if (data is! Map<String, dynamic>) return;

    final current = NotificationEntity.fromJson(data);
    await _client.put(
      '/api/notifications/$notificationId',
      accessToken: token,
      body: {
        'title': current.title,
        'message': current.message,
        'type': current.type,
        'relatedTable': current.relatedTable,
        'relatedId': current.relatedId,
        'isRead': true,
        'readAt': DateTime.now().toIso8601String(),
      },
    );
  }
}

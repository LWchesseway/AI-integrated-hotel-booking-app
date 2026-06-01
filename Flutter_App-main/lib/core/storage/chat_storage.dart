import 'package:shared_preferences/shared_preferences.dart';

class ChatStorage {
  static const _threadKeyPrefix = 'chat.thread.';

  Future<void> saveLastThreadId(int userId, String threadId) async {
    final prefs = await SharedPreferences.getInstance();
    await prefs.setString('$_threadKeyPrefix$userId', threadId);
  }

  Future<String?> getLastThreadId(int userId) async {
    final prefs = await SharedPreferences.getInstance();
    return prefs.getString('$_threadKeyPrefix$userId');
  }

  Future<void> clearLastThreadId(int userId) async {
    final prefs = await SharedPreferences.getInstance();
    await prefs.remove('$_threadKeyPrefix$userId');
  }
}

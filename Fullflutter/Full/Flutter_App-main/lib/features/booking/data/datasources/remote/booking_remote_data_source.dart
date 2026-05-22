import '../../../../../core/network/api_client.dart';
import '../../../../../core/storage/auth_storage.dart';
import '../../../domain/entities/booking_entity.dart';
import 'package:intl/intl.dart';

import '../../models/booking_model.dart';
import '../../models/room_model.dart';
import '../../models/time_slot_model.dart';

class BookingRemoteDataSource {
  BookingRemoteDataSource({ApiClient? client})
    : _client = client ?? ApiClient();

  final ApiClient _client;
  final AuthStorage _authStorage = AuthStorage();

  Future<BookingEntity> createRequest({
    required int roomId,
    required DateTime checkInDate,
    required DateTime checkOutDate,
    required int guestCount,
    required double paidAmount,
    String? paymentMethod,
    String? transactionCode,
    String? paymentNote,
    String? note,
  }) async {
    final token = await _authStorage.getAccessToken();
    final response = await _client.post(
      '/api/bookings/request',
      accessToken: token,
      body: {
        'roomId': roomId,
        'checkInDate': DateFormat("yyyy-MM-dd'T'HH:mm:ss").format(checkInDate),
        'checkOutDate': DateFormat("yyyy-MM-dd'T'HH:mm:ss").format(checkOutDate),
        'guestCount': guestCount,
        'paidAmount': paidAmount.toInt(),
        if (paymentMethod != null) 'paymentMethod': paymentMethod,
        if (transactionCode != null && transactionCode.trim().isNotEmpty)
          'transactionCode': transactionCode.trim(),
        if (paymentNote != null && paymentNote.trim().isNotEmpty)
          'paymentNote': paymentNote.trim(),
        if (note != null) 'note': note,
      },
    );
    return BookingModel.fromJson(response['data'] as Map<String, dynamic>);
  }

  Future<(List<BookingEntity>, int)> getMyBookings({
    int pageIndex = 1,
    int pageSize = 20,
  }) async {
    final token = await _authStorage.getAccessToken();
    try {
      final response = await _client.get(
        '/api/bookings/my-bookings',
        query: {'pageIndex': pageIndex, 'pageSize': pageSize},
        accessToken: token,
      );
      final data = response['data'];
      if (data is! List) return (<BookingEntity>[], 0);
      final total = (response['totalRecords'] as num?)?.toInt() ??
          (response['totalCount'] as num?)?.toInt() ??
          0;
      final items = data
          .whereType<Map<String, dynamic>>()
          .map(BookingModel.fromJson)
          .toList();
      return (items, total);
    } catch (e) {
      // Fallback to empty list instead of throwing exception to show empty state
      return (<BookingEntity>[], 0);
    }
  }

  Future<(List<BookingEntity>, int)> getAllBookings({
    int pageIndex = 1,
    int pageSize = 20,
  }) async {
    final token = await _authStorage.getAccessToken();
    final response = await _client.get(
      '/api/bookings',
      query: {'pageIndex': pageIndex, 'pageSize': pageSize},
      accessToken: token,
    );
    final data = response['data'];
    if (data is! List) return (<BookingEntity>[], 0);

    final total = (response['totalRecords'] as num?)?.toInt() ??
        (response['totalCount'] as num?)?.toInt() ??
        0;
    final items = data
        .whereType<Map<String, dynamic>>()
        .map(BookingModel.fromJson)
        .toList();
    return (items, total);
  }

  Future<BookingEntity?> cancelBooking(int id, String reason) async {
    final token = await _authStorage.getAccessToken();
    final response = await _client.post(
      '/api/bookings/$id/cancel',
      accessToken: token,
      body: {'reason': reason},
    );
    final data = response['data'];
    if (data is! Map<String, dynamic>) return null;
    return BookingModel.fromJson(data);
  }

  Future<BookingEntity> updateBookingStatus({
    required BookingEntity booking,
    required int status,
    required double paidAmount,
    int? cancelledBy,
    String? cancelReason,
    DateTime? cancelledAt,
  }) async {
    final token = await _authStorage.getAccessToken();
    final response = await _client.put(
      '/api/bookings/${booking.id}',
      accessToken: token,
      body: {
        'roomId': booking.roomId,
        'customerId': booking.customerId,
        'checkInDate': DateFormat("yyyy-MM-dd'T'HH:mm:ss").format(booking.checkInDate),
        'checkOutDate': DateFormat("yyyy-MM-dd'T'HH:mm:ss").format(booking.checkOutDate),
        'guestCount': booking.guestCount,
        'paidAmount': paidAmount.toInt(),
        'note': booking.note,
        'status': status,
        'cancelledBy': cancelledBy,
        'cancelReason': cancelReason,
        'cancelledAt': cancelledAt?.toIso8601String(),
      },
    );
    return BookingModel.fromJson(response['data'] as Map<String, dynamic>);
  }

  Future<List<RoomModel>> getRoomsByHotel(
    int hotelId, {
    required DateTime checkInDate,
    required DateTime checkOutDate,
  }) async {
    final response = await _client.get(
      '/api/rooms/by-hotel/available',
      query: {
        'hotelId': hotelId,
        'checkInDate': DateFormat('yyyy-MM-dd').format(checkInDate),
        'checkOutDate': DateFormat('yyyy-MM-dd').format(checkOutDate),
      },
    );
    final data = response['data'] ?? response;
    if (data is! List) return [];
    return data
        .whereType<Map<String, dynamic>>()
        .map(RoomModel.fromJson)
        .toList();
  }

  Future<List<RoomModel>> getRoomsCatalogByHotel(int hotelId) async {
    final response = await _client.get(
      '/api/rooms/by-hotel',
      query: {'hotelId': hotelId},
    );
    final data = response['data'] ?? response;
    if (data is! List) return [];
    return data
        .whereType<Map<String, dynamic>>()
        .map(RoomModel.fromJson)
        .toList();
  }

  Future<List<DateTime>> getFullyBookedDatesByHotel(
    int hotelId, {
    required DateTime fromDate,
    required DateTime toDate,
  }) async {
    final response = await _client.get(
      '/api/rooms/by-hotel/fully-booked-dates',
      query: {
        'hotelId': hotelId,
        'fromDate': DateFormat('yyyy-MM-dd').format(fromDate),
        'toDate': DateFormat('yyyy-MM-dd').format(toDate),
      },
    );
    final data = response['data'] ?? response;
    if (data is! List) return [];

    return data
        .map((item) => DateTime.tryParse(item.toString()))
        .whereType<DateTime>()
        .map((date) => DateTime(date.year, date.month, date.day))
        .toList();
  }

  Future<List<TimeSlotModel>> getTimeSlotsByRoom(int roomId) async {
    final token = await _authStorage.getAccessToken();
    final response = await _client.get(
      '/api/time-slots/room/$roomId',
      accessToken: token,
    );
    final data = response['data'] ?? response;
    if (data is! List) return [];
    return data
        .whereType<Map<String, dynamic>>()
        .map(TimeSlotModel.fromJson)
        .toList();
  }
}

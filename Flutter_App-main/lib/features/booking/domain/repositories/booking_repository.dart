import '../entities/booking_entity.dart';
import '../entities/room_entity.dart';
import '../entities/time_slot_entity.dart';

abstract class BookingRepository {
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
  });

  Future<(List<BookingEntity>, int)> getMyBookings({
    int pageIndex = 1,
    int pageSize = 20,
  });

  Future<(List<BookingEntity>, int)> getAllBookings({
    int pageIndex = 1,
    int pageSize = 20,
  });

  Future<BookingEntity?> cancelBooking(int id, String reason);
  Future<BookingEntity> updateBookingStatus({
    required BookingEntity booking,
    required int status,
    required double paidAmount,
    int? cancelledBy,
    String? cancelReason,
    DateTime? cancelledAt,
  });
  Future<List<RoomEntity>> getRoomsByHotel(
    int hotelId, {
    required DateTime checkInDate,
    required DateTime checkOutDate,
  });
  Future<List<RoomEntity>> getRoomsCatalogByHotel(int hotelId);
  Future<List<DateTime>> getFullyBookedDatesByHotel(
    int hotelId, {
    required DateTime fromDate,
    required DateTime toDate,
  });
  Future<List<TimeSlotEntity>> getTimeSlotsByRoom(int roomId);
}

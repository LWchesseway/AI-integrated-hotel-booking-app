import '../entities/room_entity.dart';
import '../repositories/booking_repository.dart';

class GetRoomsByHotelIdUseCase {
  final BookingRepository repository;

  GetRoomsByHotelIdUseCase(this.repository);

  Future<List<RoomEntity>> execute(
    int hotelId, {
    required DateTime checkInDate,
    required DateTime checkOutDate,
  }) {
    return repository.getRoomsByHotel(
      hotelId,
      checkInDate: checkInDate,
      checkOutDate: checkOutDate,
    );
  }
}

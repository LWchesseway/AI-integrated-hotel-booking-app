import '../repositories/booking_repository.dart';

class GetHotelFullyBookedDatesUseCase {
  final BookingRepository _repository;

  const GetHotelFullyBookedDatesUseCase(this._repository);

  Future<List<DateTime>> execute(
    int hotelId, {
    required DateTime fromDate,
    required DateTime toDate,
  }) {
    return _repository.getFullyBookedDatesByHotel(
      hotelId,
      fromDate: fromDate,
      toDate: toDate,
    );
  }
}

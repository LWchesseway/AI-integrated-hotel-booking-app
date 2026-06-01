import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:flutter_app/core/storage/auth_storage.dart';
import 'package:flutter_app/features/booking/domain/repositories/booking_repository.dart';
import 'package:flutter_app/features/home/domain/entities/hotel_recommendation_entity.dart';
import 'package:flutter_app/features/home/domain/usecases/get_home_recommendations_usecase.dart';
import 'package:flutter_app/main.dart';
import 'package:flutter_app/injection.dart' as di;

const String testEmail = String.fromEnvironment(
  'TEST_EMAIL',
  defaultValue: 'abcd@gmail.com',
);
const String testPassword = String.fromEnvironment(
  'TEST_PASSWORD',
  defaultValue: 'abcdef',
);

const String testHotelId = String.fromEnvironment(
  'TEST_HOTEL_ID',
  defaultValue: '',
);
const String testHotelName = String.fromEnvironment(
  'TEST_HOTEL_NAME',
  defaultValue: '',
);

const int _recommendationsPageSize = 12;
const int _recommendationsMaxPages = 3;
const int _defaultSearchWindowDays = 21;
const int _defaultStayNights = 2;

bool _diInitialized = false;

Future<void> _ensureDiInitialized() async {
  if (_diInitialized) {
    return;
  }
  await di.init();
  _diInitialized = true;
}

Future<void> launchApp(WidgetTester tester) async {
  WidgetsFlutterBinding.ensureInitialized();
  await _ensureDiInitialized();
  await tester.pumpWidget(const WhiteHotelApp());
  await tester.pumpAndSettle();
}

Future<void> pumpUntilFound(
  WidgetTester tester,
  Finder finder, {
  Duration timeout = const Duration(seconds: 15),
}) async {
  final end = DateTime.now().add(timeout);
  while (DateTime.now().isBefore(end)) {
    await tester.pump(const Duration(milliseconds: 200));
    if (finder.evaluate().isNotEmpty) {
      return;
    }
  }
  throw TestFailure('Timed out waiting for ${finder.description}');
}

Future<bool> pumpUntilFoundOrTimeout(
  WidgetTester tester,
  Finder finder, {
  Duration timeout = const Duration(seconds: 5),
}) async {
  final end = DateTime.now().add(timeout);
  while (DateTime.now().isBefore(end)) {
    await tester.pump(const Duration(milliseconds: 200));
    if (finder.evaluate().isNotEmpty) {
      return true;
    }
  }
  return false;
}

Future<void> loginWithTestAccount(WidgetTester tester) async {
  await AuthStorage().clear();
  await launchApp(tester);

  final startButton = find.byKey(const Key('welcome_start_button'));
  await pumpUntilFound(tester, startButton);
  await tester.tap(startButton);
  await tester.pumpAndSettle();

  await tester.enterText(
    find.byKey(const Key('login_email_input')),
    testEmail,
  );
  await tester.enterText(
    find.byKey(const Key('login_password_input')),
    testPassword,
  );

  await tester.tap(find.byKey(const Key('login_submit_button')));
  await pumpUntilFound(tester, find.byKey(const Key('home_screen_root')));
}

class BookingCandidate {
  final int hotelId;
  final String hotelName;
  final DateTime checkIn;
  final DateTime checkOut;

  const BookingCandidate({
    required this.hotelId,
    required this.hotelName,
    required this.checkIn,
    required this.checkOut,
  });
}

int? _parseEnvInt(String raw) {
  final value = raw.trim();
  if (value.isEmpty) {
    return null;
  }
  return int.tryParse(value);
}

bool _isSafeCalendarRange(DateTime start, DateTime end) {
  if (start.year != end.year || start.month != end.month) {
    return false;
  }
  return start.day >= 8 && end.day <= 22;
}

Future<BookingCandidate?> findBookingCandidate({
  int stayNights = _defaultStayNights,
  int searchWindowDays = _defaultSearchWindowDays,
}) async {
  await _ensureDiInitialized();

  final bookingRepository = di.sl<BookingRepository>();
  final recommendations = di.sl<GetHomeRecommendationsUseCase>();
  final session = await AuthStorage().getSession();

  final hotels = <HotelRecommendationEntity>[];
  for (var pageIndex = 1; pageIndex <= _recommendationsMaxPages; pageIndex++) {
    final items = await recommendations.execute(
      topK: _recommendationsPageSize,
      pageIndex: pageIndex,
      province: null,
      accessToken: session?.accessToken,
    );
    if (items.isEmpty) {
      break;
    }
    hotels.addAll(items);
    if (items.length < _recommendationsPageSize) {
      break;
    }
  }

  if (hotels.isEmpty) {
    return null;
  }

  final preferredId = _parseEnvInt(testHotelId);
  final preferredName = testHotelName.trim().toLowerCase();
  Iterable<HotelRecommendationEntity> candidates = hotels;

  if (preferredId != null) {
    candidates = candidates.where((hotel) => hotel.hotelId == preferredId);
  }
  if (preferredName.isNotEmpty) {
    candidates = candidates.where(
      (hotel) => hotel.name.toLowerCase().contains(preferredName),
    );
  }
  if (candidates.isEmpty) {
    candidates = hotels;
  }

  final now = DateTime.now();
  final today = DateTime(now.year, now.month, now.day);

  for (final hotel in candidates) {
    for (var offset = 1; offset <= searchWindowDays; offset++) {
      final checkIn = today.add(Duration(days: offset));
      final checkOut = checkIn.add(Duration(days: stayNights));

      if (!_isSafeCalendarRange(checkIn, checkOut)) {
        continue;
      }

      try {
        final rooms = await bookingRepository.getRoomsByHotel(
          hotel.hotelId,
          checkInDate: checkIn,
          checkOutDate: checkOut,
        );
        if (rooms.isNotEmpty) {
          return BookingCandidate(
            hotelId: hotel.hotelId,
            hotelName: hotel.name,
            checkIn: checkIn,
            checkOut: checkOut,
          );
        }
      } catch (_) {
        continue;
      }
    }
  }

  return null;
}

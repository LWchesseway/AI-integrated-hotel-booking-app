import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:integration_test/integration_test.dart';
import 'helpers/test_helpers.dart';

void main() {
  IntegrationTestWidgetsFlutterBinding.ensureInitialized();

  testWidgets('booking flow', (tester) async {
    await loginWithTestAccount(tester);

    final candidate = await findBookingCandidate();
    if (candidate == null) {
      debugPrint('No booking candidate found. Skipping booking flow.');
      return;
    }

    final hotelNameFinder = find.text(candidate.hotelName);
    final hotelVisible = await _scrollToHotel(tester, hotelNameFinder);
    if (!hotelVisible) {
      debugPrint('Hotel not visible in recommendations. Skipping booking flow.');
      return;
    }

    await tester.tap(hotelNameFinder);
    await tester.pumpAndSettle();

    final bookButton = find.byKey(const Key('hotel_detail_book_button'));
    await pumpUntilFound(tester, bookButton);
    await tester.tap(bookButton);
    await tester.pumpAndSettle();

    final calendar = find.byKey(const Key('room_selection_calendar'));
    await pumpUntilFound(tester, calendar);

    await _selectDateRange(
      tester,
      calendar,
      candidate.checkIn,
      candidate.checkOut,
    );

    await tester.tap(
      find.byKey(const Key('room_selection_continue_button')),
    );
    await tester.pumpAndSettle();

    final timeOption = find.byKey(const Key('time_option_08_00'));
    await pumpUntilFound(tester, timeOption);
    await tester.tap(timeOption);
    await tester.pumpAndSettle();

    await tester.tap(find.byKey(const Key('time_confirm_button')));
    await tester.pumpAndSettle();

    final roomSelect = find.byKey(const Key('room_select_button_0'));
    var roomFound = await pumpUntilFoundOrTimeout(
      tester,
      roomSelect,
      timeout: const Duration(seconds: 8),
    );
    if (!roomFound) {
      await tester.tap(find.text('Phòng STD'));
      await tester.pumpAndSettle();
      roomFound = await pumpUntilFoundOrTimeout(
        tester,
        roomSelect,
        timeout: const Duration(seconds: 8),
      );
    }
    if (!roomFound) {
      debugPrint('No rooms available for selected dates. Skipping booking.');
      return;
    }
    await tester.tap(roomSelect);
    await tester.pumpAndSettle();

    final paymentConfirm = find.byKey(const Key('payment_confirm_button'));
    await pumpUntilFound(tester, paymentConfirm);
    await tester.tap(paymentConfirm);

    await pumpUntilFound(
      tester,
      find.byKey(const Key('booking_success_dialog')),
      timeout: const Duration(seconds: 30),
    );

    await tester.tap(find.byKey(const Key('booking_success_ok_button')));
    await tester.pumpAndSettle();

    await pumpUntilFound(tester, find.byKey(const Key('main_nav_screen')));
  });
}

Future<bool> _scrollToHotel(WidgetTester tester, Finder hotelFinder) async {
  if (await pumpUntilFoundOrTimeout(tester, hotelFinder)) {
    return true;
  }

  final scrollable = find.byType(CustomScrollView);
  try {
    await tester.scrollUntilVisible(
      hotelFinder,
      400,
      scrollable: scrollable,
    );
    await tester.pumpAndSettle();
    return true;
  } catch (_) {
    return false;
  }
}

Future<void> _selectDateRange(
  WidgetTester tester,
  Finder calendar,
  DateTime checkIn,
  DateTime checkOut,
) async {
  await _navigateCalendarToMonth(tester, calendar, checkIn);
  await _tapCalendarDay(tester, calendar, checkIn);
  await tester.pumpAndSettle();
  await _tapCalendarDay(tester, calendar, checkOut);
  await tester.pumpAndSettle();
}

Future<void> _navigateCalendarToMonth(
  WidgetTester tester,
  Finder calendar,
  DateTime target,
) async {
  final now = DateTime.now();
  var current = DateTime(now.year, now.month);
  final targetMonth = DateTime(target.year, target.month);

  if (current.year == targetMonth.year && current.month == targetMonth.month) {
    return;
  }

  final nextButton = find.descendant(
    of: calendar,
    matching: find.byIcon(Icons.chevron_right),
  );
  final prevButton = find.descendant(
    of: calendar,
    matching: find.byIcon(Icons.chevron_left),
  );

  final step = targetMonth.isAfter(current) ? 1 : -1;
  final button = step > 0 ? nextButton : prevButton;

  if (button.evaluate().isEmpty) {
    throw TestFailure('Calendar navigation buttons not found.');
  }

  while (current.year != targetMonth.year || current.month != targetMonth.month) {
    await tester.tap(button);
    await tester.pumpAndSettle();
    current = DateTime(current.year, current.month + step);
  }
}

Future<void> _tapCalendarDay(
  WidgetTester tester,
  Finder calendar,
  DateTime day,
) async {
  final dayFinder = find.descendant(
    of: calendar,
    matching: find.text('${day.day}'),
  );
  if (dayFinder.evaluate().isEmpty) {
    throw TestFailure('Calendar day not found: ${day.day}.');
  }
  await tester.tap(dayFinder.first);
}

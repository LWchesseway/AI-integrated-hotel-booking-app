import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:integration_test/integration_test.dart';
import 'helpers/test_helpers.dart';

void main() {
  IntegrationTestWidgetsFlutterBinding.ensureInitialized();

  testWidgets('booking flow', (tester) async {
    await loginWithTestAccount(tester);

    final firstHotel = find.byKey(const Key('hotel_card_0'));
    await pumpUntilFound(tester, firstHotel);
    await tester.tap(firstHotel);
    await tester.pumpAndSettle();

    final bookButton = find.byKey(const Key('hotel_detail_book_button'));
    await pumpUntilFound(tester, bookButton);
    await tester.tap(bookButton);
    await tester.pumpAndSettle();

    final calendar = find.byKey(const Key('room_selection_calendar'));
    await pumpUntilFound(tester, calendar);

    final startDay = DateTime.now().add(const Duration(days: 3));
    final endDay = startDay.add(const Duration(days: 2));

    final startFinder = find.descendant(
      of: calendar,
      matching: find.text('${startDay.day}'),
    ).first;
    final endFinder = find.descendant(
      of: calendar,
      matching: find.text('${endDay.day}'),
    ).first;

    await tester.tap(startFinder);
    await tester.pumpAndSettle();
    await tester.tap(endFinder);
    await tester.pumpAndSettle();

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
    await pumpUntilFound(tester, roomSelect);
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

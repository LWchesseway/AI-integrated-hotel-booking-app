import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:flutter_app/core/storage/auth_storage.dart';
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

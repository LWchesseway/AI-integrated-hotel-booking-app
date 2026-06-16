import 'package:flutter/material.dart';
import 'package:google_fonts/google_fonts.dart';

import '../../../../core/constants/app_colors.dart';
import '../../../../core/storage/auth_storage.dart';
import '../../../../core/widgets/fade_indexed_stack.dart';
import '../../../booking/presentation/screens/my_bookings_screen.dart';
import '../../../booking/presentation/screens/owner_booking_approvals_screen.dart';
import '../../../favorite/presentation/screens/favorites_screen.dart';
import '../../../home/presentation/screens/home_screen.dart';
import '../../../profile/presentation/screens/profile_screen.dart';

class MainNavScreen extends StatefulWidget {
  final int initialIndex;
  final int initialBookingTab;

  const MainNavScreen({
    super.key,
    this.initialIndex = 0,
    this.initialBookingTab = 0,
  });

  @override
  State<MainNavScreen> createState() => _MainNavScreenState();
}

class _MainNavScreenState extends State<MainNavScreen> {
  late int _currentIndex;
  bool _isOwner = false;
  bool _isSessionLoaded = false;

  Key _favoritesRefreshKey = UniqueKey();
  Key _bookingsRefreshKey = UniqueKey();
  Key _approvalsRefreshKey = UniqueKey();

  @override
  void initState() {
    super.initState();
    _currentIndex = widget.initialIndex;
    _loadSession();
  }

  Future<void> _loadSession() async {
    final session = await AuthStorage().getSession();
    if (!mounted) return;
    setState(() {
      _isOwner = session?.isOwner ?? false;
      _isSessionLoaded = true;
    });
  }

  @override
  Widget build(BuildContext context) {
    final screens = _isOwner
        ? <Widget>[
            const HomeScreen(),
            OwnerBookingApprovalsScreen(key: _approvalsRefreshKey),
            const ProfileScreen(),
          ]
        : <Widget>[
            const HomeScreen(),
            MyBookingsScreen(
              key: _bookingsRefreshKey,
              initialTabIndex: widget.initialBookingTab,
            ),
            FavoritesScreen(key: _favoritesRefreshKey),
            const ProfileScreen(),
          ];

    final items = _isOwner
        ? const <BottomNavigationBarItem>[
            BottomNavigationBarItem(
              icon: Icon(Icons.home_outlined),
              activeIcon: Icon(Icons.home),
              label: 'Trang chủ',
            ),
            BottomNavigationBarItem(
              icon: Icon(Icons.approval_outlined),
              activeIcon: Icon(Icons.approval),
              label: 'Duyệt đơn',
            ),
            BottomNavigationBarItem(
              icon: Icon(Icons.person_outline),
              activeIcon: Icon(Icons.person),
              label: 'Hồ sơ',
            ),
          ]
        : const <BottomNavigationBarItem>[
            BottomNavigationBarItem(
              icon: Icon(Icons.home_outlined),
              activeIcon: Icon(Icons.home),
              label: 'Trang chủ',
            ),
            BottomNavigationBarItem(
              icon: Icon(Icons.receipt_long_outlined),
              activeIcon: Icon(Icons.receipt_long),
              label: 'Đặt phòng',
            ),
            BottomNavigationBarItem(
              icon: Icon(Icons.favorite_border),
              activeIcon: Icon(Icons.favorite),
              label: 'Yêu thích',
            ),
            BottomNavigationBarItem(
              icon: Icon(Icons.person_outline),
              activeIcon: Icon(Icons.person),
              label: 'Hồ sơ',
            ),
          ];

    final safeIndex = _currentIndex.clamp(0, screens.length - 1);

    return Scaffold(
      key: const Key('main_nav_screen'),
      body: !_isSessionLoaded
          ? const Center(
              child: CircularProgressIndicator(color: AppColors.primary),
            )
          : FadeIndexedStack(index: safeIndex, children: screens),
      bottomNavigationBar: Container(
        decoration: const BoxDecoration(
          boxShadow: [
            BoxShadow(
              color: Colors.black12,
              blurRadius: 12,
              offset: Offset(0, -3),
            ),
          ],
        ),
        child: BottomNavigationBar(
          currentIndex: safeIndex,
          onTap: (index) {
            setState(() {
              _currentIndex = index;
              if (!_isOwner) {
                if (index == 1) {
                  _bookingsRefreshKey = UniqueKey();
                } else if (index == 2) {
                  _favoritesRefreshKey = UniqueKey();
                }
              } else {
                if (index == 1) {
                  _approvalsRefreshKey = UniqueKey();
                }
              }
            });
          },
          type: BottomNavigationBarType.fixed,
          backgroundColor: Colors.white,
          selectedItemColor: AppColors.primary,
          unselectedItemColor: AppColors.textHint,
          selectedLabelStyle: GoogleFonts.poppins(
            fontSize: 11,
            fontWeight: FontWeight.w600,
          ),
          unselectedLabelStyle: GoogleFonts.poppins(fontSize: 11),
          items: items,
        ),
      ),
    );
  }
}

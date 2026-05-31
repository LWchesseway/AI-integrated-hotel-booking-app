import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:google_fonts/google_fonts.dart';

import '../../../../../injection.dart' as di;
import '../../../../core/constants/app_colors.dart';
import '../../../shell/presentation/screens/main_nav_screen.dart';
import '../../domain/entities/booking_entity.dart';
import '../bloc/booking_bloc.dart';
import '../widgets/booking_item_card.dart';
import 'booking_detail_screen.dart';

class MyBookingsScreen extends StatelessWidget {
  final int initialTabIndex;

  const MyBookingsScreen({super.key, this.initialTabIndex = 0});

  @override
  Widget build(BuildContext context) {
    return BlocProvider(
      create: (_) => di.sl<BookingBloc>()..add(LoadMyBookingsEvent()),
      child: _MyBookingsScreenView(initialTabIndex: initialTabIndex),
    );
  }
}

class _MyBookingsScreenView extends StatefulWidget {
  final int initialTabIndex;

  const _MyBookingsScreenView({required this.initialTabIndex});

  @override
  State<_MyBookingsScreenView> createState() => _MyBookingsScreenViewState();
}

class _MyBookingsScreenViewState extends State<_MyBookingsScreenView>
    with SingleTickerProviderStateMixin {
  final _tabs = const [
    'Tất cả',
    'Chờ duyệt',
    'Đã xác nhận',
    'Hoàn thành',
    'Đã hủy',
  ];

  late final TabController _tabController;

  @override
  void initState() {
    super.initState();
    final initialIndex = widget.initialTabIndex.clamp(0, _tabs.length - 1);
    _tabController = TabController(
      length: _tabs.length,
      vsync: this,
      initialIndex: initialIndex,
    );
    _tabController.addListener(() {
      if (_tabController.indexIsChanging) {
        context.read<BookingBloc>().add(LoadMyBookingsEvent());
      }
    });
  }

  @override
  void dispose() {
    _tabController.dispose();
    super.dispose();
  }

  List<BookingEntity> _filter(int tab, List<BookingEntity> all) {
    switch (tab) {
      case 1:
        return all.where((booking) => booking.status == 0).toList();
      case 2:
        return all.where((booking) => booking.status == 1).toList();
      case 3:
        return all.where((booking) => booking.status == 3).toList();
      case 4:
        return all.where((booking) => booking.status == 2).toList();
      default:
        return all;
    }
  }

  Widget _buildEmptyState(BuildContext context) {
    return Center(
      child: Column(
        mainAxisAlignment: MainAxisAlignment.center,
        children: [
          Container(
            padding: const EdgeInsets.all(24),
            decoration: BoxDecoration(
              color: Colors.green.withOpacity(0.05),
              shape: BoxShape.circle,
            ),
            child: Icon(
              Icons.calendar_today_outlined,
              size: 64,
              color: AppColors.greenPrimary.withOpacity(0.2),
            ),
          ),
          const SizedBox(height: 24),
          Text(
            'Hiện tại bạn chưa có phòng nào',
            style: GoogleFonts.playfairDisplay(
              fontSize: 20,
              fontWeight: FontWeight.bold,
            ),
          ),
          const SizedBox(height: 8),
          Text(
            'Quay lại trang chủ để đặt phòng nhé',
            style: GoogleFonts.dmSans(color: Colors.grey),
          ),
          const SizedBox(height: 24),
          ElevatedButton(
            onPressed: () {
              Navigator.pushAndRemoveUntil(
                context,
                MaterialPageRoute(
                  builder: (_) => const MainNavScreen(initialIndex: 0),
                ),
                (route) => false,
              );
            },
            style: ElevatedButton.styleFrom(
              backgroundColor: AppColors.greenPrimary,
              shape: RoundedRectangleBorder(
                borderRadius: BorderRadius.circular(12),
              ),
              padding: const EdgeInsets.symmetric(horizontal: 24, vertical: 12),
            ),
            child: Text(
              'Quay lại trang chủ',
              style: GoogleFonts.dmSans(
                color: Colors.white,
                fontWeight: FontWeight.bold,
              ),
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildErrorState(String message) {
    return Center(
      child: Padding(
        padding: const EdgeInsets.all(32),
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            const Icon(
              Icons.cloud_off_rounded,
              size: 80,
              color: Colors.redAccent,
            ),
            const SizedBox(height: 16),
            Text(
              'Kết nối thất bại',
              style: GoogleFonts.playfairDisplay(
                fontSize: 22,
                fontWeight: FontWeight.bold,
              ),
            ),
            const SizedBox(height: 8),
            Text(
              message,
              textAlign: TextAlign.center,
              style: GoogleFonts.dmSans(color: Colors.grey),
            ),
            const SizedBox(height: 24),
            ElevatedButton(
              style: ElevatedButton.styleFrom(
                backgroundColor: AppColors.greenPrimary,
                padding: const EdgeInsets.symmetric(horizontal: 40, vertical: 12),
                shape: RoundedRectangleBorder(
                  borderRadius: BorderRadius.circular(12),
                ),
              ),
              onPressed: () => context.read<BookingBloc>().add(LoadMyBookingsEvent()),
              child: const Text(
                'Thử lại',
                style: TextStyle(color: Colors.white),
              ),
            ),
          ],
        ),
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: const Color(0xFFF8FAFB),
      appBar: AppBar(
        systemOverlayStyle: SystemUiOverlayStyle.light,
        flexibleSpace: Container(
          decoration: const BoxDecoration(
            gradient: LinearGradient(
              colors: [AppColors.greenPrimary, AppColors.greenMedium],
              begin: Alignment.topLeft,
              end: Alignment.bottomRight,
            ),
          ),
        ),
        title: Text(
          'Chuyến đi của tôi',
          style: GoogleFonts.playfairDisplay(
            color: Colors.white,
            fontSize: 22,
            fontWeight: FontWeight.w700,
          ),
        ),
        elevation: 0,
        actions: [
          IconButton(
            icon: const Icon(Icons.refresh_rounded, color: Colors.white),
            onPressed: () => context.read<BookingBloc>().add(LoadMyBookingsEvent()),
          ),
        ],
        bottom: TabBar(
          controller: _tabController,
          isScrollable: true,
          indicatorColor: Colors.amber,
          indicatorWeight: 3,
          labelColor: Colors.white,
          unselectedLabelColor: Colors.white70,
          labelStyle: GoogleFonts.dmSans(
            fontWeight: FontWeight.bold,
            fontSize: 13,
          ),
          tabs: _tabs.map((tab) => Tab(text: tab)).toList(),
        ),
      ),
      body: BlocBuilder<BookingBloc, BookingState>(
        builder: (context, state) {
          if (state is BookingLoading || state is BookingInitial) {
            return const Center(
              child: CircularProgressIndicator(color: AppColors.greenPrimary),
            );
          }

          if (state is BookingError) {
            return _buildErrorState(state.message);
          }

          if (state is MyBookingsLoaded) {
            return TabBarView(
              controller: _tabController,
              children: List.generate(_tabs.length, (index) {
                final filteredList = _filter(index, state.bookings);
                if (filteredList.isEmpty) {
                  return _buildEmptyState(context);
                }

                return ListView.builder(
                  padding: const EdgeInsets.all(16),
                  itemCount: filteredList.length,
                  itemBuilder: (context, itemIndex) => BookingItemCard(
                    booking: filteredList[itemIndex],
                    onTap: () => Navigator.push(
                      context,
                      MaterialPageRoute(
                        builder: (_) => BookingDetailScreen(
                          booking: filteredList[itemIndex],
                        ),
                      ),
                    ),
                  ),
                );
              }),
            );
          }

          return const SizedBox.shrink();
        },
      ),
    );
  }
}

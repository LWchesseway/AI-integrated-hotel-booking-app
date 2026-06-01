import 'package:flutter/material.dart';
import 'package:google_fonts/google_fonts.dart';

import '../../../../core/constants/app_colors.dart';
import '../../../../core/storage/auth_storage.dart';
import '../../../../injection.dart' as di;
import '../../../hotel/domain/entities/hotel_entity.dart';
import '../../../hotel/domain/repositories/hotel_repository.dart';
import '../../domain/entities/booking_entity.dart';
import '../../domain/repositories/booking_repository.dart';
import '../widgets/booking_item_card.dart';

class OwnerBookingApprovalsScreen extends StatefulWidget {
  const OwnerBookingApprovalsScreen({super.key});

  @override
  State<OwnerBookingApprovalsScreen> createState() =>
      _OwnerBookingApprovalsScreenState();
}

class _OwnerBookingApprovalsScreenState
    extends State<OwnerBookingApprovalsScreen>
    with SingleTickerProviderStateMixin {
  final BookingRepository _bookingRepository = di.sl<BookingRepository>();
  final HotelRepository _hotelRepository = di.sl<HotelRepository>();
  final AuthStorage _authStorage = AuthStorage();

  final List<String> _tabs = const [
    'Tất cả',
    'Chờ duyệt',
    'Đã xác nhận',
    'Hoàn thành',
    'Đã hủy',
  ];

  late final TabController _tabController;

  bool _isLoading = true;
  bool _isSubmitting = false;
  String? _error;
  int? _ownerId;
  List<BookingEntity> _bookings = const [];
  Map<int, String> _hotelNamesByRoomId = const {};

  @override
  void initState() {
    super.initState();
    _tabController = TabController(length: _tabs.length, vsync: this);
    _loadOwnerBookings();
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

  Future<List<HotelEntity>> _loadOwnerHotels(int ownerId) async {
    const pageSize = 100;
    var pageIndex = 1;
    var totalCount = 0;
    final ownerHotels = <HotelEntity>[];

    do {
      final (items, total) = await _hotelRepository.getAll(
        pageIndex: pageIndex,
        pageSize: pageSize,
      );
      totalCount = total;
      ownerHotels.addAll(items.where((hotel) => hotel.ownerId == ownerId));
      pageIndex++;
    } while ((pageIndex - 1) * pageSize < totalCount);

    return ownerHotels;
  }

  Future<List<BookingEntity>> _loadAllBookings() async {
    const pageSize = 100;
    var pageIndex = 1;
    var totalCount = 0;
    final bookings = <BookingEntity>[];

    do {
      final (items, total) = await _bookingRepository.getAllBookings(
        pageIndex: pageIndex,
        pageSize: pageSize,
      );
      totalCount = total;
      bookings.addAll(items);
      pageIndex++;
    } while ((pageIndex - 1) * pageSize < totalCount);

    return bookings;
  }

  Future<void> _loadOwnerBookings() async {
    setState(() {
      _isLoading = true;
      _error = null;
    });

    try {
      final session = await _authStorage.getSession();
      if (session == null) {
        throw Exception('Không tìm thấy phiên đăng nhập.');
      }

      final ownerId = session.userId;
      final ownerHotels = await _loadOwnerHotels(ownerId);

      if (ownerHotels.isEmpty) {
        if (!mounted) return;
        setState(() {
          _ownerId = ownerId;
          _bookings = const [];
          _hotelNamesByRoomId = const {};
          _isLoading = false;
        });
        return;
      }

      final roomResults = await Future.wait(
        ownerHotels.map((hotel) => _bookingRepository.getRoomsCatalogByHotel(hotel.id)),
      );

      final ownerRoomIds = <int>{};
      final roomHotelNames = <int, String>{};
      for (var i = 0; i < ownerHotels.length; i++) {
        final hotel = ownerHotels[i];
        for (final room in roomResults[i]) {
          ownerRoomIds.add(room.id);
          roomHotelNames[room.id] = hotel.name;
        }
      }

      final bookings = await _loadAllBookings();
      final ownerBookings = bookings
          .where((booking) => ownerRoomIds.contains(booking.roomId))
          .toList()
        ..sort((a, b) => b.createdAt.compareTo(a.createdAt));

      if (!mounted) return;
      setState(() {
        _ownerId = ownerId;
        _bookings = ownerBookings;
        _hotelNamesByRoomId = roomHotelNames;
        _isLoading = false;
      });
    } catch (error) {
      if (!mounted) return;
      setState(() {
        _error = error.toString();
        _isLoading = false;
      });
    }
  }

  Future<void> _updateBookingStatus(BookingEntity booking, int status) async {
    if (_isSubmitting) return;

    setState(() {
      _isSubmitting = true;
    });

    try {
      await _bookingRepository.updateBookingStatus(
        booking: booking,
        status: status,
        paidAmount: status == 1 ? booking.totalAmount : 0,
        cancelledBy: status == 2 ? _ownerId : null,
        cancelReason: status == 2 ? 'Owner từ chối đơn đặt phòng' : null,
        cancelledAt: status == 2 ? DateTime.now() : null,
      );

      if (!mounted) return;
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text(
            status == 1 ? 'Đã xác nhận đơn đặt phòng.' : 'Đã từ chối đơn đặt phòng.',
          ),
          backgroundColor: status == 1 ? Colors.green : Colors.red,
        ),
      );
      await _loadOwnerBookings();
    } catch (error) {
      if (!mounted) return;
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text(error.toString()),
          backgroundColor: Colors.redAccent,
        ),
      );
    } finally {
      if (mounted) {
        setState(() {
          _isSubmitting = false;
        });
      }
    }
  }

  Widget _buildActionButton({
    required String label,
    required Color color,
    required IconData icon,
    required VoidCallback onPressed,
  }) {
    return SizedBox(
      height: 44,
      child: ElevatedButton.icon(
        onPressed: _isSubmitting ? null : onPressed,
        icon: Icon(icon, color: Colors.white, size: 16),
        label: Text(
          label,
          style: GoogleFonts.dmSans(
            color: Colors.white,
            fontWeight: FontWeight.bold,
            fontSize: 13,
          ),
        ),
        style: ElevatedButton.styleFrom(
          backgroundColor: color,
          disabledBackgroundColor: color.withOpacity(0.45),
          shape: RoundedRectangleBorder(
            borderRadius: BorderRadius.circular(12),
          ),
          elevation: 0,
        ),
      ),
    );
  }

  Widget _buildEmptyState() {
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
              Icons.approval_outlined,
              size: 64,
              color: AppColors.greenPrimary.withOpacity(0.2),
            ),
          ),
          const SizedBox(height: 24),
          Text(
            'Chưa có đơn cần xử lý',
            style: GoogleFonts.playfairDisplay(
              fontSize: 20,
              fontWeight: FontWeight.bold,
            ),
          ),
          const SizedBox(height: 8),
          Text(
            'Các booking thuộc khách sạn của bạn sẽ hiện ở đây.',
            style: GoogleFonts.dmSans(color: Colors.grey),
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
              'Không tải được danh sách đơn',
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
              onPressed: _loadOwnerBookings,
              style: ElevatedButton.styleFrom(
                backgroundColor: AppColors.greenPrimary,
                padding: const EdgeInsets.symmetric(horizontal: 40, vertical: 12),
                shape: RoundedRectangleBorder(
                  borderRadius: BorderRadius.circular(12),
                ),
              ),
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
      backgroundColor: const Color(0xFFF9F5F0),
      appBar: AppBar(
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
          'Duyệt đơn đặt phòng',
          style: GoogleFonts.playfairDisplay(
            color: Colors.white,
            fontSize: 22,
            fontWeight: FontWeight.bold,
          ),
        ),
        elevation: 2,
        shadowColor: AppColors.greenPrimary.withOpacity(0.2),
        actions: [
          IconButton(
            icon: const Icon(Icons.refresh_rounded, color: Colors.white),
            onPressed: _loadOwnerBookings,
          ),
        ],
        bottom: PreferredSize(
          preferredSize: const Size.fromHeight(48),
          child: Container(
            padding: const EdgeInsets.only(bottom: 8),
            child: TabBar(
              controller: _tabController,
              isScrollable: true,
              indicatorSize: TabBarIndicatorSize.tab,
              padding: const EdgeInsets.symmetric(horizontal: 12),
              indicator: BoxDecoration(
                borderRadius: BorderRadius.circular(20),
                color: Colors.amber,
                boxShadow: [
                  BoxShadow(
                    color: Colors.black12,
                    blurRadius: 4,
                    offset: const Offset(0, 2),
                  ),
                ],
              ),
              labelColor: AppColors.brownDark,
              unselectedLabelColor: Colors.white.withOpacity(0.85),
              labelStyle: GoogleFonts.dmSans(
                fontWeight: FontWeight.bold,
                fontSize: 13,
              ),
              unselectedLabelStyle: GoogleFonts.dmSans(
                fontWeight: FontWeight.w600,
                fontSize: 13,
              ),
              tabs: _tabs.map((tab) => Tab(
                child: Container(
                  padding: const EdgeInsets.symmetric(horizontal: 12),
                  alignment: Alignment.center,
                  child: Text(tab),
                ),
              )).toList(),
            ),
          ),
        ),
      ),
      body: _isLoading
          ? const Center(
              child: CircularProgressIndicator(color: AppColors.greenPrimary),
            )
          : _error != null
              ? _buildErrorState(_error!)
              : TabBarView(
                  controller: _tabController,
                  children: List.generate(_tabs.length, (index) {
                    final filteredList = _filter(index, _bookings);
                    if (filteredList.isEmpty) {
                      return _buildEmptyState();
                    }

                    return RefreshIndicator(
                      onRefresh: _loadOwnerBookings,
                      child: ListView.builder(
                        padding: const EdgeInsets.all(16),
                        itemCount: filteredList.length,
                        itemBuilder: (context, itemIndex) {
                          final booking = filteredList[itemIndex];
                          return BookingItemCard(
                            booking: booking,
                            hotelLabel: _hotelNamesByRoomId[booking.roomId],
                            actions: booking.isPending
                                ? [
                                    _buildActionButton(
                                      label: 'Xác nhận',
                                      color: Colors.green,
                                      icon: Icons.check_circle_outline_rounded,
                                      onPressed: () => _updateBookingStatus(booking, 1),
                                    ),
                                    _buildActionButton(
                                      label: 'Từ chối',
                                      color: Colors.red,
                                      icon: Icons.cancel_outlined,
                                      onPressed: () => _updateBookingStatus(booking, 2),
                                    ),
                                  ]
                                : const [],
                          );
                        },
                      ),
                    );
                  }),
                ),
    );
  }
}

import 'package:flutter/material.dart';
import 'package:google_fonts/google_fonts.dart';
import 'package:intl/intl.dart';
import 'package:table_calendar/table_calendar.dart';

import '../../../../core/constants/app_colors.dart';
import '../../../../injection.dart' as di;
import '../../domain/repositories/booking_repository.dart';
import '../../domain/entities/room_entity.dart';
import 'payment_screen.dart';

const _kGreen = AppColors.primary;
const _kSurface = Color(0xFFF8FAFC);
const _kTextPrimary = Color(0xFF1E293B);
const _kRed = Color(0xFFEF4444);

class RoomSelectionScreen extends StatefulWidget {
  final int hotelId;
  final String hotelName;

  const RoomSelectionScreen({
    super.key,
    required this.hotelId,
    required this.hotelName,
  });

  @override
  State<RoomSelectionScreen> createState() => _RoomSelectionScreenState();
}

class _RoomSelectionScreenState extends State<RoomSelectionScreen>
    with SingleTickerProviderStateMixin {
  final BookingRepository _bookingRepository = di.sl<BookingRepository>();
  late final TabController _tabController;

  List<RoomEntity> _rooms = [];
  bool _isLoading = true;
  String? _errorMessage;

  @override
  void initState() {
    super.initState();
    _tabController = TabController(length: 2, vsync: this);
    _loadRooms();
  }

  @override
  void dispose() {
    _tabController.dispose();
    super.dispose();
  }

  Future<void> _loadRooms() async {
    try {
      final rooms = await _bookingRepository.getRoomsCatalogByHotel(widget.hotelId);
      if (!mounted) return;
      setState(() {
        _rooms = rooms;
        _isLoading = false;
      });
    } catch (_) {
      if (!mounted) return;
      setState(() {
        _errorMessage = 'Không thể tải danh sách phòng. Vui lòng thử lại!';
        _isLoading = false;
      });
    }
  }

  String _roomTypeKey(RoomEntity room) {
    return (room.roomTypeName ?? '').trim().toUpperCase();
  }

  bool _isVipRoom(RoomEntity room) {
    final key = _roomTypeKey(room);
    return key.contains('VIP') || key.contains('DELUXE');
  }

  bool _isStdRoom(RoomEntity room) {
    final key = _roomTypeKey(room);
    return key.contains('STD') || key.contains('STANDARD') || (!key.contains('VIP') && !key.contains('DELUXE'));
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: _kSurface,
      appBar: AppBar(
        elevation: 0,
        backgroundColor: Colors.white,
        leading: IconButton(
          icon: const Icon(
            Icons.arrow_back_ios_new,
            color: _kTextPrimary,
            size: 20,
          ),
          onPressed: () => Navigator.pop(context),
        ),
        title: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(
              'Chọn hạng phòng',
              style: GoogleFonts.dmSans(
                color: _kTextPrimary,
                fontWeight: FontWeight.bold,
                fontSize: 18,
              ),
            ),
            Text(
              widget.hotelName,
              style: GoogleFonts.dmSans(
                color: Colors.grey,
                fontSize: 12,
              ),
            ),
          ],
        ),
        bottom: PreferredSize(
          preferredSize: const Size.fromHeight(60),
          child: Container(
            margin: const EdgeInsets.symmetric(horizontal: 20, vertical: 8),
            decoration: BoxDecoration(
              color: Colors.grey.shade100,
              borderRadius: BorderRadius.circular(16),
            ),
            child: TabBar(
              controller: _tabController,
              indicator: BoxDecoration(
                color: _kGreen,
                borderRadius: BorderRadius.circular(12),
                boxShadow: [
                  BoxShadow(
                    color: _kGreen.withOpacity(0.2),
                    blurRadius: 8,
                    offset: const Offset(0, 4),
                  ),
                ],
              ),
              labelColor: Colors.white,
              unselectedLabelColor: Colors.grey.shade600,
              labelStyle: GoogleFonts.dmSans(
                fontWeight: FontWeight.bold,
                fontSize: 14,
              ),
              tabs: const [
                Tab(text: 'Phòng VIP'),
                Tab(text: 'Phòng STD'),
              ],
            ),
          ),
        ),
      ),
      body: _isLoading
          ? const Center(
              child: CircularProgressIndicator(color: _kGreen),
            )
          : _errorMessage != null
              ? Center(
                  child: Padding(
                    padding: const EdgeInsets.all(24),
                    child: Text(
                      _errorMessage!,
                      textAlign: TextAlign.center,
                      style: GoogleFonts.dmSans(color: Colors.redAccent),
                    ),
                  ),
                )
              : TabBarView(
                  controller: _tabController,
                  children: [
                    _buildRoomList(_rooms.where(_isVipRoom).toList()),
                    _buildRoomList(_rooms.where(_isStdRoom).toList()),
                  ],
                ),
    );
  }

  Widget _buildRoomList(List<RoomEntity> rooms) {
    if (rooms.isEmpty) {
      return Center(
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Icon(Icons.bed_rounded, size: 60, color: Colors.grey.shade300),
            const SizedBox(height: 16),
            Text(
              'Không tìm thấy phòng thuộc hạng này',
              style: GoogleFonts.dmSans(color: Colors.grey, fontSize: 16),
            ),
          ],
        ),
      );
    }

    return ListView.builder(
      padding: const EdgeInsets.all(20),
      itemCount: rooms.length,
      itemBuilder: (context, index) {
        final room = rooms[index];
        final isVip = _isVipRoom(room);

        return Container(
          margin: const EdgeInsets.only(bottom: 16),
          padding: const EdgeInsets.all(16),
          decoration: BoxDecoration(
            color: Colors.white,
            borderRadius: BorderRadius.circular(20),
            border: Border.all(
              color: isVip ? Colors.amber.shade300 : Colors.grey.withOpacity(0.1),
              width: isVip ? 2 : 1,
            ),
            boxShadow: [
              BoxShadow(color: Colors.black.withOpacity(0.02), blurRadius: 10),
            ],
          ),
          child: Row(
            children: [
              Container(
                width: 80,
                height: 80,
                decoration: BoxDecoration(
                  color: isVip ? Colors.amber.shade50 : _kGreen.withOpacity(0.1),
                  borderRadius: BorderRadius.circular(16),
                ),
                child: Icon(
                  Icons.king_bed_rounded,
                  color: isVip ? Colors.amber.shade700 : _kGreen,
                  size: 30,
                ),
              ),
              const SizedBox(width: 16),
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Row(
                      children: [
                        Flexible(
                          child: Text(
                            'Phòng ${room.roomNumber ?? room.id}',
                            style: GoogleFonts.dmSans(
                              fontWeight: FontWeight.bold,
                              fontSize: 16,
                            ),
                          ),
                        ),
                        if (isVip) ...[
                          const SizedBox(width: 8),
                          Container(
                            padding: const EdgeInsets.symmetric(
                              horizontal: 6,
                              vertical: 2,
                            ),
                            decoration: BoxDecoration(
                              color: Colors.amber,
                              borderRadius: BorderRadius.circular(4),
                            ),
                            child: const Text(
                              'VIP',
                              style: TextStyle(
                                color: Colors.white,
                                fontSize: 10,
                                fontWeight: FontWeight.bold,
                              ),
                            ),
                          ),
                        ],
                      ],
                    ),
                    const SizedBox(height: 4),
                    Text(
                      '${room.capacity} khách',
                      style: GoogleFonts.dmSans(
                        color: Colors.grey.shade600,
                        fontSize: 13,
                      ),
                    ),
                    const SizedBox(height: 4),
                    Text(
                      '${NumberFormat.currency(locale: 'vi_VN', symbol: '₫').format(room.price)}/đêm',
                      style: GoogleFonts.dmSans(
                        color: _kGreen,
                        fontWeight: FontWeight.bold,
                      ),
                    ),
                  ],
                ),
              ),
              ElevatedButton(
                key: Key('room_select_button_$index'),
                onPressed: () {
                  Navigator.push(
                    context,
                    MaterialPageRoute(
                      builder: (_) => RoomBookingCalendarScreen(
                        room: room,
                        hotelName: widget.hotelName,
                      ),
                    ),
                  );
                },
                style: ElevatedButton.styleFrom(
                  backgroundColor: _kGreen,
                  shape: RoundedRectangleBorder(
                    borderRadius: BorderRadius.circular(10),
                  ),
                  elevation: 0,
                  padding: const EdgeInsets.symmetric(horizontal: 16),
                ),
                child: const Text(
                  'Đặt phòng',
                  style: TextStyle(color: Colors.white, fontWeight: FontWeight.bold),
                ),
              ),
            ],
          ),
        );
      },
    );
  }
}

class RoomBookingCalendarScreen extends StatefulWidget {
  final RoomEntity room;
  final String hotelName;

  const RoomBookingCalendarScreen({
    super.key,
    required this.room,
    required this.hotelName,
  });

  @override
  State<RoomBookingCalendarScreen> createState() => _RoomBookingCalendarScreenState();
}

class _RoomBookingCalendarScreenState extends State<RoomBookingCalendarScreen> {
  final BookingRepository _bookingRepository = di.sl<BookingRepository>();

  DateTime? _rangeStart;
  DateTime? _rangeEnd;
  List<DateTime> _bookedDates = [];
  bool _isLoadingCalendar = true;

  @override
  void initState() {
    super.initState();
    _fetchBookedDates();
  }

  Future<void> _fetchBookedDates() async {
    try {
      final now = DateTime.now();
      final bookedDates = await _bookingRepository.getBookedDatesByRoom(
        widget.room.id,
        fromDate: now,
        toDate: now.add(const Duration(days: 365)),
      );

      if (!mounted) return;
      setState(() {
        _bookedDates = bookedDates;
        _isLoadingCalendar = false;
      });
    } catch (_) {
      if (!mounted) return;
      setState(() {
        _bookedDates = [];
        _isLoadingCalendar = false;
      });
    }
  }

  bool _isBooked(DateTime day) {
    return _bookedDates.any((date) => isSameDay(date, day));
  }

  List<DateTime> _getOccupiedDays(DateTime start, DateTime end) {
    final days = <DateTime>[];
    for (var i = 0; i < end.difference(start).inDays; i++) {
      days.add(
        DateTime(start.year, start.month, start.day).add(Duration(days: i)),
      );
    }
    return days;
  }

  void _showBookedMessage(String message) {
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(
        content: Text(message, style: const TextStyle(color: Colors.white)),
        backgroundColor: Colors.red,
      ),
    );
  }

  void _showTimeSelectionSheet() {
    showModalBottomSheet(
      context: context,
      backgroundColor: Colors.transparent,
      isScrollControlled: true,
      builder: (context) => _TimeSelectionSheet(
        onTimeSelected: (time) {
          Navigator.pop(context);
          Navigator.push(
            context,
            MaterialPageRoute(
              builder: (_) => PaymentScreen(
                roomId: widget.room.id,
                hotelName: widget.hotelName,
                roomNumber: widget.room.roomNumber ?? widget.room.id.toString(),
                roomType: widget.room.roomTypeName ?? 'Phòng',
                price: widget.room.price.toInt(),
                checkInDate: _rangeStart!,
                checkOutDate: _rangeEnd!,
              ),
            ),
          );
        },
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    final canContinue = _rangeStart != null && _rangeEnd != null;

    return Scaffold(
      backgroundColor: _kSurface,
      appBar: AppBar(
        elevation: 0,
        backgroundColor: Colors.white,
        leading: IconButton(
          icon: const Icon(
            Icons.arrow_back_ios_new,
            color: _kTextPrimary,
            size: 20,
          ),
          onPressed: () => Navigator.pop(context),
        ),
        title: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(
              'Chọn ngày đặt phòng',
              style: GoogleFonts.dmSans(
                color: _kTextPrimary,
                fontWeight: FontWeight.bold,
                fontSize: 18,
              ),
            ),
            Text(
              'Phòng ${widget.room.roomNumber ?? widget.room.id} • ${widget.hotelName}',
              style: GoogleFonts.dmSans(color: Colors.grey, fontSize: 12),
            ),
          ],
        ),
      ),
      body: _isLoadingCalendar
          ? const Center(
              child: CircularProgressIndicator(color: _kGreen),
            )
          : Column(
              children: [
                Expanded(
                  child: SingleChildScrollView(
                    padding: const EdgeInsets.all(20),
                    child: Container(
                      padding: const EdgeInsets.all(12),
                      decoration: BoxDecoration(
                        color: Colors.white,
                        borderRadius: BorderRadius.circular(24),
                        boxShadow: [
                          BoxShadow(
                            color: Colors.black.withOpacity(0.03),
                            blurRadius: 20,
                          ),
                        ],
                      ),
                      child: TableCalendar(
                        key: const Key('room_selection_calendar'),
                        firstDay: DateTime.now(),
                        lastDay: DateTime.now().add(const Duration(days: 365)),
                        focusedDay: _rangeStart ?? DateTime.now(),
                        rangeStartDay: _rangeStart,
                        rangeEndDay: _rangeEnd,
                        rangeSelectionMode: RangeSelectionMode.toggledOn,
                        onRangeSelected: (start, end, focusedDay) {
                          if (start != null && _isBooked(start)) {
                            _showBookedMessage(
                              'Phòng này đã được đặt kín vào ngày bắt đầu.',
                            );
                            return;
                          }

                          if (start != null && end != null) {
                            final occupiedDays = _getOccupiedDays(start, end);
                            if (occupiedDays.any(_isBooked)) {
                              _showBookedMessage(
                                'Phòng này đã được đặt kín trong khoảng thời gian này.',
                              );
                              return;
                            }
                          }

                          setState(() {
                            _rangeStart = start;
                            _rangeEnd = end;
                          });
                        },
                        calendarBuilders: CalendarBuilders(
                          defaultBuilder: (context, day, focusedDay) {
                            if (_isBooked(day)) {
                              return Container(
                                margin: const EdgeInsets.all(6),
                                alignment: Alignment.center,
                                decoration: const BoxDecoration(
                                  color: Color(0xFFFFEBEE),
                                  shape: BoxShape.circle,
                                ),
                                child: Text(
                                  '${day.day}',
                                  style: const TextStyle(
                                    color: _kRed,
                                    fontWeight: FontWeight.bold,
                                  ),
                                ),
                              );
                            }
                            return null;
                          },
                        ),
                        calendarStyle: const CalendarStyle(
                          rangeHighlightColor: Color(0xFFE8F5EE),
                          rangeStartDecoration: BoxDecoration(
                            color: _kGreen,
                            shape: BoxShape.circle,
                          ),
                          rangeEndDecoration: BoxDecoration(
                            color: _kGreen,
                            shape: BoxShape.circle,
                          ),
                          todayDecoration: BoxDecoration(
                            color: Colors.orangeAccent,
                            shape: BoxShape.circle,
                          ),
                        ),
                        headerStyle: const HeaderStyle(
                          formatButtonVisible: false,
                          titleCentered: true,
                        ),
                      ),
                    ),
                  ),
                ),
                Container(
                  padding: const EdgeInsets.all(24),
                  decoration: BoxDecoration(
                    color: Colors.white,
                    boxShadow: [
                      BoxShadow(
                        color: Colors.black.withOpacity(0.05),
                        blurRadius: 20,
                        offset: const Offset(0, -5),
                      ),
                    ],
                  ),
                  child: SizedBox(
                    width: double.infinity,
                    height: 56,
                    child: ElevatedButton(
                      key: const Key('room_selection_continue_button'),
                      onPressed: canContinue ? _showTimeSelectionSheet : null,
                      style: ElevatedButton.styleFrom(
                        backgroundColor: _kGreen,
                        disabledBackgroundColor: Colors.grey.shade300,
                        shape: RoundedRectangleBorder(
                          borderRadius: BorderRadius.circular(16),
                        ),
                      ),
                      child: Text(
                        'Tiếp tục',
                        style: GoogleFonts.dmSans(
                          fontSize: 16,
                          fontWeight: FontWeight.bold,
                          color: canContinue ? Colors.white : Colors.grey.shade500,
                        ),
                      ),
                    ),
                  ),
                ),
              ],
            ),
    );
  }
}

class _TimeSelectionSheet extends StatefulWidget {
  final void Function(String) onTimeSelected;

  const _TimeSelectionSheet({required this.onTimeSelected});

  @override
  State<_TimeSelectionSheet> createState() => _TimeSelectionSheetState();
}

class _TimeSelectionSheetState extends State<_TimeSelectionSheet> {
  final List<String> _times = const [
    '08:00',
    '10:00',
    '12:00',
    '14:00',
    '16:00',
    '18:00',
    '20:00',
  ];

  String? _selectedTime;

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.all(24),
      decoration: const BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.vertical(top: Radius.circular(24)),
      ),
      child: SafeArea(
        child: Column(
          mainAxisSize: MainAxisSize.min,
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Center(
              child: Container(
                width: 40,
                height: 5,
                decoration: BoxDecoration(
                  color: Colors.grey.shade300,
                  borderRadius: BorderRadius.circular(10),
                ),
              ),
            ),
            const SizedBox(height: 24),
            Text(
              'Chọn giờ nhận phòng',
              style: GoogleFonts.dmSans(
                fontSize: 20,
                fontWeight: FontWeight.bold,
                color: _kTextPrimary,
              ),
            ),
            const SizedBox(height: 16),
            Wrap(
              spacing: 12,
              runSpacing: 12,
              children: _times.map((time) {
                final isSelected = _selectedTime == time;
                return GestureDetector(
                  onTap: () => setState(() => _selectedTime = time),
                  child: AnimatedContainer(
                    key: Key('time_option_${time.replaceAll(':', '_')}'),
                    duration: const Duration(milliseconds: 200),
                    padding: const EdgeInsets.symmetric(
                      horizontal: 20,
                      vertical: 12,
                    ),
                    decoration: BoxDecoration(
                      color: isSelected ? _kGreen : Colors.white,
                      border: Border.all(
                        color: isSelected ? _kGreen : Colors.grey.shade300,
                      ),
                      borderRadius: BorderRadius.circular(12),
                    ),
                    child: Text(
                      time,
                      style: GoogleFonts.dmSans(
                        color: isSelected ? Colors.white : _kTextPrimary,
                        fontWeight: FontWeight.bold,
                      ),
                    ),
                  ),
                );
              }).toList(),
            ),
            const SizedBox(height: 32),
            SizedBox(
              width: double.infinity,
              height: 56,
              child: ElevatedButton(
                key: const Key('time_confirm_button'),
                onPressed: _selectedTime == null
                    ? null
                    : () => widget.onTimeSelected(_selectedTime!),
                style: ElevatedButton.styleFrom(
                  backgroundColor: _kGreen,
                  disabledBackgroundColor: Colors.grey.shade300,
                  shape: RoundedRectangleBorder(
                    borderRadius: BorderRadius.circular(16),
                  ),
                ),
                child: Text(
                  'Xác nhận',
                  style: GoogleFonts.dmSans(
                    fontSize: 16,
                    fontWeight: FontWeight.bold,
                    color: _selectedTime != null
                        ? Colors.white
                        : Colors.grey.shade500,
                  ),
                ),
              ),
            ),
          ],
        ),
      ),
    );
  }
}

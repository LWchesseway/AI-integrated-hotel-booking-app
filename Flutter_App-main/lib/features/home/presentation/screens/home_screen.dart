import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:google_fonts/google_fonts.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'dart:async';

import '../../../../../injection.dart' as di;
import '../../../../core/storage/auth_storage.dart';
import '../../../../core/storage/chat_storage.dart';
import '../../../../core/network/ai_api_client.dart';
import '../bloc/home_bloc.dart';
import '../../../search/presentation/screens/search_screen.dart';
import '../../../notification/presentation/screens/notifications_screen.dart';
import '../../../../core/constants/app_colors.dart';
import '../../domain/entities/hotel_recommendation_entity.dart';
import '../../domain/entities/province_entity.dart';
import '../../../booking/domain/repositories/booking_repository.dart';
import '../../../hotel/domain/entities/hotel_entity.dart';
import '../../../hotel/domain/repositories/hotel_repository.dart';
import '../../../hotel/presentation/screens/hotel_detail_screen.dart';

const _kGreen = AppColors.greenPrimary;
const _kGreenMedium = AppColors.greenMedium;
const _kSurface =
    AppColors.scaffoldBg; // Hoặc Color(0xFFF5F7FA) nếu muốn nền sáng mượt hơn
const _kTextPrimary = Color(0xFF172B24);
const _kTextSec = Color(0xFF6B7B75);

// --- HÀM FORMAT TIỀN TỆ ---
String _formatCurrency(double amount) {
  return amount
      .toStringAsFixed(0)
      .replaceAllMapped(
        RegExp(r'(\d{1,3})(?=(\d{3})+(?!\d))'),
        (m) => '${m[1]}.',
      );
}

class _ChatMessage {
  final String role;
  final String content;

  const _ChatMessage({required this.role, required this.content});
}

class HomeScreen extends StatelessWidget {
  const HomeScreen({super.key});

  @override
  Widget build(BuildContext context) {
    return BlocProvider(
      create: (_) => di.sl<HomeBloc>()..add(LoadHomeDataEvent()),
      child: const _HomeScreenView(),
    );
  }
}

class _HomeScreenView extends StatefulWidget {
  const _HomeScreenView();

  @override
  State<_HomeScreenView> createState() => _HomeScreenViewState();
}

class _HomeScreenViewState extends State<_HomeScreenView> {
  // Controller cho Banner cuộn tự động (Tùy chọn)
  final PageController _bannerController = PageController();
  final ScrollController _scrollController = ScrollController();
  int _currentBannerIndex = 0;

  final BookingRepository _bookingRepository = di.sl<BookingRepository>();
  final HotelRepository _hotelRepository = di.sl<HotelRepository>();
  final AuthStorage _authStorage = AuthStorage();
  final ChatStorage _chatStorage = ChatStorage();
  static final AiApiClient _aiClient = AiApiClient();
  TextEditingController? _chatInputController;
  ScrollController? _chatScrollController;
  List<_ChatMessage>? _chatMessages;
  Offset? _chatButtonOffset;
  bool _isChatOpen = false;
  bool _isSendingMessage = false;
  bool _isHistoryLoading = false;
  bool _isHistoryLoaded = false;
  String? _threadId;

  bool _isOwner = false;
  Future<_OwnerStats>? _ownerStatsFuture;

  final List<String> _promoBanners = [
    'Assets/images/splash_bg.png',
    'Assets/images/splash_bg6.png',
    'Assets/images/splash_bg12.png',
  ];

  @override
  void initState() {
    super.initState();
    _scrollController.addListener(_onScroll);
    _loadOwnerContext();
    _chatInputController = TextEditingController();
    _chatScrollController = ScrollController();
  }

  @override
  void dispose() {
    _scrollController.removeListener(_onScroll);
    _scrollController.dispose();
    _bannerController.dispose();
    _chatInputController?.dispose();
    _chatScrollController?.dispose();
    super.dispose();
  }

  void _initChatButtonOffset(BoxConstraints constraints) {
    if (_chatButtonOffset != null) return;
    final initial = Offset(
      constraints.maxWidth - 72,
      constraints.maxHeight * 0.6,
    );
    WidgetsBinding.instance.addPostFrameCallback((_) {
      if (!mounted) return;
      setState(() {
        _chatButtonOffset = _clampChatOffset(initial, constraints);
      });
    });
  }

  Offset _clampChatOffset(Offset offset, BoxConstraints constraints) {
    const size = 56.0;
    final minX = 8.0;
    final minY = 8.0;
    final maxX = constraints.maxWidth - size - 8.0;
    final maxY = constraints.maxHeight - size - 120.0;
    return Offset(offset.dx.clamp(minX, maxX), offset.dy.clamp(minY, maxY));
  }

  List<_ChatMessage> get _messages => _chatMessages ??= <_ChatMessage>[];
  TextEditingController get _chatInput =>
      _chatInputController ??= TextEditingController();
  ScrollController get _chatScroll =>
      _chatScrollController ??= ScrollController();

  Future<void> _persistThreadId(String threadId) async {
    final session = await _authStorage.getSession();
    final userId = session?.userId ?? 0;
    if (userId <= 0) return;
    await _chatStorage.saveLastThreadId(userId, threadId);
  }

  Future<String> _ensureThreadId(String accessToken) async {
    if (_threadId != null && _threadId!.isNotEmpty) return _threadId!;
    final response = await _aiClient.post(
      '/threads',
      accessToken: accessToken,
      body: const {'title': 'Hoi thoai moi'},
    );
    final threadId = response['thread_id']?.toString();
    if (threadId == null || threadId.isEmpty) {
      throw const AiApiException('Missing thread id from AI', 500);
    }
    _threadId = threadId;
    await _persistThreadId(threadId);
    return threadId;
  }

  Future<void> _restoreChatHistory() async {
    if (_isHistoryLoaded || _isHistoryLoading) return;

    final token = await _authStorage.getAccessToken();
    if (token == null || token.isEmpty) return;

    setState(() {
      _isHistoryLoading = true;
    });

    try {
      final session = await _authStorage.getSession();
      final userId = session?.userId ?? 0;
      if (userId <= 0) return;

      var threadId = await _chatStorage.getLastThreadId(userId);
      if (threadId == null || threadId.isEmpty) {
        final threadsResponse = await _aiClient.get(
          '/threads',
          accessToken: token,
        );
        final threadsData = threadsResponse['data'];
        if (threadsData is List && threadsData.isNotEmpty) {
          final first = threadsData.first as Map<String, dynamic>;
          threadId = first['thread_id']?.toString();
        }
      }

      if (threadId == null || threadId.isEmpty) return;

      final messagesResponse = await _aiClient.get(
        '/threads/$threadId/messages',
        accessToken: token,
      );
      final messagesData = messagesResponse['data'];
      if (messagesData is List) {
        setState(() {
          _threadId = threadId;
          _messages
            ..clear()
            ..addAll(
              messagesData.map((item) {
                final map = item as Map<String, dynamic>;
                return _ChatMessage(
                  role: map['role']?.toString() ?? 'assistant',
                  content: map['content']?.toString() ?? '',
                );
              }),
            );
        });
        await _persistThreadId(threadId);
      }
    } catch (_) {
      // Ignore history loading errors and let the user start a new chat.
    } finally {
      if (!mounted) return;
      setState(() {
        _isHistoryLoading = false;
        _isHistoryLoaded = true;
      });
    }
  }

  void _toggleChatOpen() {
    final next = !_isChatOpen;
    setState(() => _isChatOpen = next);
    if (next) {
      _restoreChatHistory();
    }
  }

  Future<void> _sendChatMessage() async {
    final raw = _chatInput.text.trim();
    if (raw.isEmpty || _isSendingMessage) return;

    final token = await _authStorage.getAccessToken();
    if (token == null || token.isEmpty) {
      if (!mounted) return;
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Vui long dang nhap de chat.')),
      );
      return;
    }

    setState(() {
      _messages.add(_ChatMessage(role: 'user', content: raw));
      _chatInput.clear();
      _isSendingMessage = true;
    });
    _scrollChatToBottom();

    try {
      final threadId = await _ensureThreadId(token);
      final response = await _aiClient.post(
        '/threads/$threadId/messages',
        accessToken: token,
        body: {'message': raw},
      );
      final message = response['message'] as Map<String, dynamic>?;
      final content = message?['content']?.toString() ?? '';
      if (content.isNotEmpty) {
        setState(() {
          _messages.add(_ChatMessage(role: 'assistant', content: content));
        });
      }
    } catch (error) {
      if (!mounted) return;
      ScaffoldMessenger.of(
        context,
      ).showSnackBar(SnackBar(content: Text(error.toString())));
    } finally {
      if (!mounted) return;
      setState(() {
        _isSendingMessage = false;
      });
      _scrollChatToBottom();
    }
  }

  void _scrollChatToBottom() {
    if (!_chatScroll.hasClients) return;
    WidgetsBinding.instance.addPostFrameCallback((_) {
      if (!_chatScroll.hasClients) return;
      _chatScroll.animateTo(
        _chatScroll.position.maxScrollExtent + 60,
        duration: const Duration(milliseconds: 250),
        curve: Curves.easeOut,
      );
    });
  }

  String _formatAssistantText(String content) {
    var cleaned = content.replaceAll('**', '');
    cleaned = cleaned.replaceAll('*', '');
    cleaned = cleaned.replaceAll('\r', '');
    return cleaned.trim();
  }

  Widget _buildChatBubble(_ChatMessage message) {
    final isUser = message.role == 'user';
    final bubbleColor = isUser ? _kGreen : Colors.white;
    final textColor = isUser ? Colors.white : _kTextPrimary;
    final alignment = isUser ? Alignment.centerRight : Alignment.centerLeft;
    final displayText = isUser
        ? message.content
        : _formatAssistantText(message.content);

    return Align(
      alignment: alignment,
      child: Container(
        margin: const EdgeInsets.symmetric(vertical: 4),
        padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 10),
        constraints: const BoxConstraints(maxWidth: 300),
        decoration: BoxDecoration(
          color: bubbleColor,
          borderRadius: BorderRadius.circular(14),
          boxShadow: [
            BoxShadow(
              color: Colors.black.withOpacity(0.08),
              blurRadius: 8,
              offset: const Offset(0, 2),
            ),
          ],
        ),
        child: Text(
          displayText,
          style: GoogleFonts.dmSans(
            color: textColor,
            fontSize: 13,
            height: 1.35,
          ),
        ),
      ),
    );
  }

  Widget _buildChatBox(BoxConstraints constraints) {
    final maxWidth = constraints.maxWidth - 24;
    final width = maxWidth.clamp(300, 380).toDouble();
    final height = (constraints.maxHeight * 0.7).clamp(360, 560).toDouble();

    return Positioned(
      right: 12,
      bottom: 90,
      child: IgnorePointer(
        ignoring: !_isChatOpen,
        child: AnimatedOpacity(
          duration: const Duration(milliseconds: 200),
          opacity: _isChatOpen ? 1 : 0,
          child: Container(
            width: width,
            height: height,
            decoration: BoxDecoration(
              color: Colors.white,
              borderRadius: BorderRadius.circular(18),
              boxShadow: [
                BoxShadow(
                  color: Colors.black.withOpacity(0.18),
                  blurRadius: 24,
                  offset: const Offset(0, 12),
                ),
              ],
            ),
            child: Column(
              children: [
                Container(
                  padding: const EdgeInsets.symmetric(
                    horizontal: 14,
                    vertical: 12,
                  ),
                  decoration: const BoxDecoration(
                    color: _kGreen,
                    borderRadius: BorderRadius.vertical(
                      top: Radius.circular(18),
                    ),
                  ),
                  child: Row(
                    children: [
                      const Icon(Icons.auto_awesome, color: Colors.white),
                      const SizedBox(width: 8),
                      Expanded(
                        child: Text(
                          'Tro ly dat phong',
                          style: GoogleFonts.dmSans(
                            color: Colors.white,
                            fontWeight: FontWeight.w600,
                          ),
                        ),
                      ),
                      IconButton(
                        onPressed: () {
                          setState(() => _isChatOpen = false);
                        },
                        icon: const Icon(Icons.close, color: Colors.white),
                      ),
                    ],
                  ),
                ),
                Expanded(
                  child: Padding(
                    padding: const EdgeInsets.symmetric(
                      horizontal: 12,
                      vertical: 8,
                    ),
                    child: _isHistoryLoading && _messages.isEmpty
                        ? Center(
                            child: Text(
                              'Dang tai lich su...',
                              style: GoogleFonts.dmSans(
                                color: _kTextSec,
                                fontStyle: FontStyle.italic,
                                fontSize: 12,
                              ),
                            ),
                          )
                        : ListView.builder(
                            controller: _chatScroll,
                            itemCount:
                                _messages.length + (_isSendingMessage ? 1 : 0),
                            itemBuilder: (context, index) {
                              if (index >= _messages.length) {
                                return Align(
                                  alignment: Alignment.centerLeft,
                                  child: Padding(
                                    padding: const EdgeInsets.only(top: 6),
                                    child: Text(
                                      'Dang tra loi...',
                                      style: GoogleFonts.dmSans(
                                        color: _kTextSec,
                                        fontStyle: FontStyle.italic,
                                        fontSize: 12,
                                      ),
                                    ),
                                  ),
                                );
                              }
                              return _buildChatBubble(_messages[index]);
                            },
                          ),
                  ),
                ),
                Container(
                  padding: const EdgeInsets.fromLTRB(12, 4, 12, 12),
                  child: Row(
                    children: [
                      Expanded(
                        child: TextField(
                          controller: _chatInput,
                          minLines: 1,
                          maxLines: 3,
                          textInputAction: TextInputAction.send,
                          onSubmitted: (_) => _sendChatMessage(),
                          decoration: InputDecoration(
                            hintText: 'Nhap tin nhan...',
                            hintStyle: GoogleFonts.dmSans(
                              color: _kTextSec,
                              fontSize: 13,
                            ),
                            filled: true,
                            fillColor: const Color(0xFFF4F6F5),
                            border: OutlineInputBorder(
                              borderRadius: BorderRadius.circular(12),
                              borderSide: BorderSide.none,
                            ),
                          ),
                        ),
                      ),
                      const SizedBox(width: 8),
                      IconButton(
                        onPressed: _isSendingMessage ? null : _sendChatMessage,
                        icon: const Icon(Icons.send, color: _kGreen),
                      ),
                    ],
                  ),
                ),
              ],
            ),
          ),
        ),
      ),
    );
  }

  Widget _buildChatButton(BoxConstraints constraints) {
    _initChatButtonOffset(constraints);
    final offset =
        _chatButtonOffset ??
        Offset(constraints.maxWidth - 72, constraints.maxHeight * 0.6);

    return Positioned(
      left: offset.dx,
      top: offset.dy,
      child: GestureDetector(
        onPanUpdate: (details) {
          final current = _chatButtonOffset ?? offset;
          final next = current + details.delta;
          setState(() {
            _chatButtonOffset = _clampChatOffset(next, constraints);
          });
        },
        onTap: _toggleChatOpen,
        child: Container(
          width: 58,
          height: 58,
          decoration: BoxDecoration(
            shape: BoxShape.circle,
            gradient: const LinearGradient(
              colors: [Color(0xFF1E7F4D), Color(0xFF2FB66B)],
              begin: Alignment.topLeft,
              end: Alignment.bottomRight,
            ),
            boxShadow: [
              BoxShadow(
                color: Colors.black.withOpacity(0.2),
                blurRadius: 14,
                offset: const Offset(0, 6),
              ),
            ],
          ),
          child: const Icon(Icons.auto_awesome, color: Colors.white),
        ),
      ),
    );
  }

  void _onScroll() {
    if (_scrollController.position.pixels >=
        _scrollController.position.maxScrollExtent - 200) {
      context.read<HomeBloc>().add(LoadMoreHotelsEvent());
    }
  }

  Future<void> _loadOwnerContext() async {
    final session = await _authStorage.getSession();
    if (!mounted) return;
    final isOwner = session?.isOwner ?? false;
    setState(() {
      _isOwner = isOwner;
      _ownerStatsFuture = isOwner && session != null
          ? _loadOwnerStats(session.userId)
          : null;
    });
  }

  Future<_OwnerStats> _loadOwnerStats(int ownerId) async {
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

    if (ownerHotels.isEmpty) {
      return const _OwnerStats();
    }

    final roomResults = await Future.wait(
      ownerHotels.map(
        (hotel) => _bookingRepository.getRoomsCatalogByHotel(hotel.id),
      ),
    );

    final roomTypeById = <int, String?>{};
    for (final rooms in roomResults) {
      for (final room in rooms) {
        roomTypeById[room.id] = room.roomTypeName;
      }
    }

    final ownerRoomIds = roomTypeById.keys.toSet();
    if (ownerRoomIds.isEmpty) {
      return _OwnerStats(totalHotels: ownerHotels.length);
    }

    pageIndex = 1;
    totalCount = 0;
    final bookings = <dynamic>[];
    do {
      final (items, total) = await _bookingRepository.getAllBookings(
        pageIndex: pageIndex,
        pageSize: pageSize,
      );
      totalCount = total;
      bookings.addAll(items);
      pageIndex++;
    } while ((pageIndex - 1) * pageSize < totalCount);

    final bookedRoomIds = bookings
        .where(
          (booking) =>
              ownerRoomIds.contains(booking.roomId) && booking.status != 2,
        )
        .map((booking) => booking.roomId)
        .toSet();

    var vipBooked = 0;
    var regularBooked = 0;
    for (final roomId in bookedRoomIds) {
      final roomTypeName = (roomTypeById[roomId] ?? '').toLowerCase();
      if (roomTypeName.contains('vip')) {
        vipBooked++;
      } else {
        regularBooked++;
      }
    }

    return _OwnerStats(
      totalHotels: ownerHotels.length,
      bookedRooms: bookedRoomIds.length,
      vipBookedRooms: vipBooked,
      regularBookedRooms: regularBooked,
    );
  }

  @override
  Widget build(BuildContext context) {
    return AnnotatedRegion<SystemUiOverlayStyle>(
      value: SystemUiOverlayStyle.dark, // Chữ status bar màu tối
      child: Scaffold(
        key: const Key('home_screen_root'),
        backgroundColor: _kSurface,
        body: BlocBuilder<HomeBloc, HomeState>(
          builder: (context, state) {
            if (state is HomeLoading || state is HomeInitial) {
              return const Center(
                child: CircularProgressIndicator(color: _kGreen),
              );
            } else if (state is HomeError) {
              return Center(
                child: Column(
                  mainAxisAlignment: MainAxisAlignment.center,
                  children: [
                    const Icon(
                      Icons.error_outline,
                      color: Colors.red,
                      size: 60,
                    ),
                    const SizedBox(height: 16),
                    Text(
                      state.message,
                      style: GoogleFonts.dmSans(color: _kTextPrimary),
                    ),
                    const SizedBox(height: 16),
                    ElevatedButton(
                      onPressed: () =>
                          context.read<HomeBloc>().add(LoadHomeDataEvent()),
                      style: ElevatedButton.styleFrom(backgroundColor: _kGreen),
                      child: Text(
                        'Thử lại',
                        style: GoogleFonts.dmSans(color: Colors.white),
                      ),
                    ),
                  ],
                ),
              );
            } else if (state is HomeLoaded) {
              return LayoutBuilder(
                builder: (context, constraints) {
                  return SafeArea(
                    child: Stack(
                      children: [
                        RefreshIndicator(
                          color: _kGreen,
                          onRefresh: () async {
                            context.read<HomeBloc>().add(LoadHomeDataEvent());
                          },
                          child: CustomScrollView(
                            controller: _scrollController,
                            physics: const BouncingScrollPhysics(),
                            slivers: [
                              // 1. Header & Logo
                              SliverToBoxAdapter(
                                child: _buildHeader(context, state.fullName),
                              ),

                              // 2. Thanh Tim Kiem
                              SliverToBoxAdapter(
                                child: _buildSearchBar(context),
                              ),

                              if (_isOwner)
                                SliverToBoxAdapter(
                                  child: _buildOwnerStatsCard(),
                                ),

                              // 3. Banner Quang cao
                              SliverToBoxAdapter(child: _buildPromoBanner()),

                              // 4. Danh sach tinh/thanh pho
                              SliverToBoxAdapter(
                                child: _buildProvincesSection(context, state),
                              ),

                              // 5. Tieu de khach san noi bat
                              SliverToBoxAdapter(
                                child: Padding(
                                  padding: const EdgeInsets.fromLTRB(
                                    20,
                                    24,
                                    20,
                                    16,
                                  ),
                                  child: Text(
                                    'Khach san danh cho ban',
                                    style: GoogleFonts.dmSans(
                                      fontSize: 20,
                                      fontWeight: FontWeight.bold,
                                      color: _kTextPrimary,
                                    ),
                                  ),
                                ),
                              ),

                              // 6. Danh sach khach san
                              SliverPadding(
                                padding: const EdgeInsets.symmetric(
                                  horizontal: 20,
                                ),
                                sliver: SliverList(
                                  key: const Key('hotel_list'),
                                  delegate: SliverChildBuilderDelegate(
                                    (context, index) {
                                      if (index == state.hotels.length) {
                                        return state.isFetchingMore
                                            ? const Padding(
                                                padding: EdgeInsets.symmetric(
                                                  vertical: 20,
                                                ),
                                                child: Center(
                                                  child:
                                                      CircularProgressIndicator(
                                                        color: _kGreen,
                                                      ),
                                                ),
                                              )
                                            : const SizedBox.shrink();
                                      }
                                      final hotel = state.hotels[index];
                                      return Padding(
                                        padding: const EdgeInsets.only(
                                          bottom: 20,
                                        ),
                                        child: _HotelCard(
                                          hotel: hotel,
                                          index: index,
                                        ),
                                      );
                                    },
                                    childCount:
                                        state.hotels.length +
                                        (state.isFetchingMore ? 1 : 0),
                                  ),
                                ),
                              ),

                              const SliverToBoxAdapter(
                                child: SizedBox(height: 30),
                              ),
                            ],
                          ),
                        ),
                        _buildChatBox(constraints),
                        _buildChatButton(constraints),
                      ],
                    ),
                  );
                },
              );
            }
            return const SizedBox.shrink();
          },
        ),
      ),
    );
  }

  // --- WIDGET: HEADER LỜI CHÀO & LOGO ---
  Widget _buildHeader(BuildContext context, String fullName) {
    return Padding(
      padding: const EdgeInsets.fromLTRB(20, 20, 20, 10),
      child: Row(
        mainAxisAlignment: MainAxisAlignment.spaceBetween,
        children: [
          Row(
            children: [
              // Chỗ này hiện Logo App
              Container(
                width: 48,
                height: 48,
                decoration: BoxDecoration(
                  color: _kGreen.withOpacity(0.1),
                  shape: BoxShape.circle,
                ),
                child: ClipOval(
                  child: Image.asset(
                    'Assets/images/logo.png', // Đảm bảo có logo này trong Assets
                    fit: BoxFit.cover,
                    errorBuilder: (context, error, stackTrace) {
                      // Fallback nếu chưa có logo
                      return const Icon(
                        Icons.maps_home_work_rounded,
                        color: _kGreen,
                        size: 28,
                      );
                    },
                  ),
                ),
              ),
              const SizedBox(width: 12),
              Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    'Xin chào,',
                    style: GoogleFonts.dmSans(fontSize: 14, color: _kTextSec),
                  ),
                  Text(
                    fullName,
                    style: GoogleFonts.dmSans(
                      fontSize: 18,
                      fontWeight: FontWeight.bold,
                      color: _kTextPrimary,
                    ),
                  ),
                ],
              ),
            ],
          ),
          Container(
            decoration: BoxDecoration(
              color: Colors.white,
              shape: BoxShape.circle,
              boxShadow: [
                BoxShadow(
                  color: Colors.black.withOpacity(0.05),
                  blurRadius: 10,
                  offset: const Offset(0, 4),
                ),
              ],
            ),
            child: IconButton(
              icon: const Icon(
                Icons.notifications_outlined,
                color: _kTextPrimary,
              ),
              onPressed: () {
                Navigator.push(
                  context,
                  MaterialPageRoute(
                    builder: (_) => const NotificationsScreen(),
                  ),
                );
              },
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildOwnerStatsCard() {
    return Padding(
      padding: const EdgeInsets.fromLTRB(20, 12, 20, 4),
      child: FutureBuilder<_OwnerStats>(
        future: _ownerStatsFuture,
        builder: (context, snapshot) {
          if (snapshot.connectionState == ConnectionState.waiting) {
            return _buildStatsSkeleton();
          }

          final stats = snapshot.data ?? const _OwnerStats();
          return Container(
            padding: const EdgeInsets.all(16),
            decoration: BoxDecoration(
              gradient: LinearGradient(
                colors: [
                  Colors.white,
                  const Color(0xFFF3E5DC).withOpacity(0.3),
                ],
                begin: Alignment.topLeft,
                end: Alignment.bottomRight,
              ),
              borderRadius: BorderRadius.circular(20),
              border: Border.all(color: const Color(0xFFE8E0D5).withOpacity(0.8)),
              boxShadow: [
                BoxShadow(
                  color: const Color(0xFF4E342E).withOpacity(0.05),
                  blurRadius: 16,
                  offset: const Offset(0, 8),
                ),
              ],
            ),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Row(
                  mainAxisAlignment: MainAxisAlignment.spaceBetween,
                  children: [
                    Row(
                      children: [
                        const Icon(
                          Icons.dashboard_customize_rounded,
                          color: _kGreen,
                          size: 18,
                        ),
                        const SizedBox(width: 8),
                        Text(
                          'Tổng quan quản lý',
                          style: GoogleFonts.dmSans(
                            fontSize: 14,
                            fontWeight: FontWeight.bold,
                            color: _kTextPrimary,
                            letterSpacing: 0.2,
                          ),
                        ),
                      ],
                    ),
                    Container(
                      padding: const EdgeInsets.symmetric(
                        horizontal: 8,
                        vertical: 3,
                      ),
                      decoration: BoxDecoration(
                        color: _kGreen.withOpacity(0.1),
                        borderRadius: BorderRadius.circular(20),
                        border: Border.all(color: _kGreen.withOpacity(0.2)),
                      ),
                      child: Row(
                        mainAxisSize: MainAxisSize.min,
                        children: [
                          const Icon(
                            Icons.star_rounded,
                            color: Colors.amber,
                            size: 12,
                          ),
                          const SizedBox(width: 4),
                          Text(
                            'ĐỐI TÁC',
                            style: GoogleFonts.dmSans(
                              color: _kGreen,
                              fontSize: 9,
                              fontWeight: FontWeight.bold,
                              letterSpacing: 0.5,
                            ),
                          ),
                        ],
                      ),
                    ),
                  ],
                ),
                const SizedBox(height: 14),
                Row(
                  children: [
                    Expanded(
                      child: _statItem(
                        icon: Icons.hotel_class_rounded,
                        iconColor: _kGreen,
                        iconBgColor: _kGreen.withOpacity(0.12),
                        label: 'Khách sạn',
                        value: stats.totalHotels.toString(),
                      ),
                    ),
                    const SizedBox(width: 10),
                    Expanded(
                      child: _statItem(
                        icon: Icons.bookmark_added_rounded,
                        iconColor: const Color(0xFFF57F17),
                        iconBgColor: const Color(0xFFFFF3E0),
                        label: 'Đã đặt',
                        value: stats.bookedRooms.toString(),
                      ),
                    ),
                  ],
                ),
                const SizedBox(height: 10),
                Row(
                  children: [
                    Expanded(
                      child: _statItem(
                        icon: Icons.workspace_premium_rounded,
                        iconColor: const Color(0xFFFFB300),
                        iconBgColor: const Color(0xFFFFF8E1),
                        label: 'VIP',
                        value: stats.vipBookedRooms.toString(),
                      ),
                    ),
                    const SizedBox(width: 10),
                    Expanded(
                      child: _statItem(
                        icon: Icons.king_bed_rounded,
                        iconColor: const Color(0xFF8D6E63),
                        iconBgColor: const Color(0xFFEFEBE9),
                        label: 'Thường',
                        value: stats.regularBookedRooms.toString(),
                      ),
                    ),
                  ],
                ),
              ],
            ),
          );
        },
      ),
    );
  }

  Widget _buildStatsSkeleton() {
    return Container(
      height: 108,
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(18),
      ),
      alignment: Alignment.center,
      child: const SizedBox(
        height: 18,
        width: 18,
        child: CircularProgressIndicator(color: _kGreen, strokeWidth: 2),
      ),
    );
  }

  Widget _statItem({
    required IconData icon,
    required Color iconColor,
    required Color iconBgColor,
    required String label,
    required String value,
  }) {
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 12),
      decoration: BoxDecoration(
        color: const Color(0xFFFDFAF6),
        borderRadius: BorderRadius.circular(14),
        border: Border.all(color: const Color(0xFFE8E0D5).withOpacity(0.5)),
      ),
      child: Row(
        children: [
          Container(
            padding: const EdgeInsets.all(8),
            decoration: BoxDecoration(
              color: iconBgColor,
              shape: BoxShape.circle,
            ),
            child: Icon(icon, color: iconColor, size: 20),
          ),
          const SizedBox(width: 10),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              mainAxisSize: MainAxisSize.min,
              children: [
                Text(
                  label,
                  style: GoogleFonts.dmSans(
                    fontSize: 11,
                    fontWeight: FontWeight.w600,
                    color: _kTextSec,
                  ),
                  maxLines: 1,
                  overflow: TextOverflow.ellipsis,
                ),
                const SizedBox(height: 2),
                Text(
                  value,
                  style: GoogleFonts.dmSans(
                    fontSize: 15,
                    fontWeight: FontWeight.w800,
                    color: _kTextPrimary,
                  ),
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }

  // --- WIDGET: THANH TÌM KIẾM ---
  Widget _buildSearchBar(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.symmetric(horizontal: 20, vertical: 16),
      child: GestureDetector(
        onTap: () {
          Navigator.push(
            context,
            MaterialPageRoute(builder: (_) => const SearchScreen()),
          );
        },
        child: Container(
          padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 14),
          decoration: BoxDecoration(
            color: Colors.white,
            borderRadius: BorderRadius.circular(16),
            boxShadow: [
              BoxShadow(
                color: _kGreen.withOpacity(0.08),
                blurRadius: 20,
                offset: const Offset(0, 8),
              ),
            ],
            border: Border.all(color: Colors.grey.withOpacity(0.1)),
          ),
          child: Row(
            children: [
              const Icon(Icons.search_rounded, color: _kGreen, size: 28),
              const SizedBox(width: 12),
              Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    'Tìm kiếm khách sạn...',
                    style: GoogleFonts.dmSans(
                      fontSize: 16,
                      fontWeight: FontWeight.w600,
                      color: _kTextPrimary,
                    ),
                  ),
                  const SizedBox(height: 2),
                  Text(
                    'Địa điểm, tên khách sạn',
                    style: GoogleFonts.dmSans(fontSize: 13, color: _kTextSec),
                  ),
                ],
              ),
            ],
          ),
        ),
      ),
    );
  }

  // --- WIDGET: BANNER QUẢNG CÁO (CAROUSEL) ---
  Widget _buildPromoBanner() {
    return Column(
      children: [
        SizedBox(
          height: 160,
          child: PageView.builder(
            controller: _bannerController,
            onPageChanged: (index) {
              setState(() {
                _currentBannerIndex = index;
              });
            },
            itemCount: _promoBanners.length,
            itemBuilder: (context, index) {
              return Container(
                margin: const EdgeInsets.symmetric(horizontal: 20),
                decoration: BoxDecoration(
                  borderRadius: BorderRadius.circular(20),
                  image: DecorationImage(
                    image: AssetImage(_promoBanners[index]),
                    fit: BoxFit.cover,
                  ),
                  boxShadow: [
                    BoxShadow(
                      color: Colors.black.withOpacity(0.1),
                      blurRadius: 10,
                      offset: const Offset(0, 5),
                    ),
                  ],
                ),
                child: Container(
                  decoration: BoxDecoration(
                    borderRadius: BorderRadius.circular(20),
                    gradient: LinearGradient(
                      begin: Alignment.topCenter,
                      end: Alignment.bottomCenter,
                      colors: [
                        Colors.transparent,
                        Colors.black.withOpacity(0.6),
                      ],
                    ),
                  ),
                  padding: const EdgeInsets.all(20),
                  alignment: Alignment.bottomLeft,
                  child: Text(
                    index == 0 ? 'Ưu đãi mùa hè giảm 50%' : 'Khám phá ngay',
                    style: GoogleFonts.dmSans(
                      color: Colors.white,
                      fontSize: 18,
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                ),
              );
            },
          ),
        ),
        const SizedBox(height: 12),
        // Dấu chấm chỉ báo (Indicators)
        Row(
          mainAxisAlignment: MainAxisAlignment.center,
          children: List.generate(
            3,
            (index) => AnimatedContainer(
              duration: const Duration(milliseconds: 300),
              margin: const EdgeInsets.symmetric(horizontal: 4),
              height: 8,
              width: _currentBannerIndex == index ? 24 : 8,
              decoration: BoxDecoration(
                color: _currentBannerIndex == index
                    ? _kGreen
                    : Colors.grey.shade300,
                borderRadius: BorderRadius.circular(4),
              ),
            ),
          ),
        ),
      ],
    );
  }

  // --- WIDGET: CHỌN TỈNH/THÀNH PHỐ ---
  Widget _buildProvincesSection(BuildContext context, HomeLoaded state) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Padding(
          padding: const EdgeInsets.fromLTRB(20, 24, 20, 16),
          child: Text(
            'Khám phá điểm đến',
            style: GoogleFonts.dmSans(
              fontSize: 20,
              fontWeight: FontWeight.bold,
              color: _kTextPrimary,
            ),
          ),
        ),
        SizedBox(
          height: 42,
          child: ListView.builder(
            padding: const EdgeInsets.symmetric(horizontal: 16),
            scrollDirection: Axis.horizontal,
            itemCount: state.provinces.length + 1,
            itemBuilder: (context, index) {
              if (index == 0) {
                return Padding(
                  padding: const EdgeInsets.symmetric(horizontal: 4),
                  child: _ProvinceChip(
                    label: 'Tất cả',
                    isSelected: state.selectedProvince == null,
                    onTap: () {
                      context.read<HomeBloc>().add(
                        RefreshHotelsEvent(province: null),
                      );
                    },
                  ),
                );
              }
              final province = state.provinces[index - 1];
              return Padding(
                padding: const EdgeInsets.symmetric(horizontal: 4),
                child: _ProvinceChip(
                  label: province.name ?? '',
                  isSelected: state.selectedProvince?.id == province.id,
                  onTap: () {
                    context.read<HomeBloc>().add(
                      RefreshHotelsEvent(province: province),
                    );
                  },
                ),
              );
            },
          ),
        ),
      ],
    );
  }
}

// --- WIDGET: THẺ KHÁCH SẠN (HOTEL CARD) ---
class _HotelCard extends StatelessWidget {
  final HotelRecommendationEntity hotel;
  final int index;

  const _HotelCard({required this.hotel, required this.index});

  @override
  Widget build(BuildContext context) {
    // 1. CƠ CHẾ FALLBACK ẢNH RẤT QUAN TRỌNG
    // Nếu API trả về null hoặc rỗng, dùng ảnh xịn này để app luôn đẹp
    const String defaultHotelImage =
        'https://images.unsplash.com/photo-1566073771259-6a8506099945?q=80&w=1000&auto=format&fit=crop';

    // Tùy theo Entity của bạn có trường imageUrl hay list ảnh
    // Dưới đây giả định Entity có property tên là `imageUrl` hoặc `images`
    String displayImage = defaultHotelImage;
    // GIẢ THUYẾT: Nếu entity của bạn gọi là hotel.imageUrl (Sửa lại cho đúng với entity của bạn)
    // if (hotel.imageUrl != null && hotel.imageUrl!.isNotEmpty) {
    //   displayImage = hotel.imageUrl!;
    // }

    return GestureDetector(
      key: Key('hotel_card_$index'),
      onTap: () {
        Navigator.push(
          context,
          MaterialPageRoute(
            builder: (_) => HotelDetailScreen(
              hotelId: hotel.hotelId,
              hotelName: hotel.name,
              rating: hotel.averageRating,
            ),
          ),
        );
      },
      child: Container(
        decoration: BoxDecoration(
          color: Colors.white,
          borderRadius: BorderRadius.circular(20),
          boxShadow: [
            BoxShadow(
              color: Colors.black.withOpacity(0.04),
              blurRadius: 15,
              offset: const Offset(0, 6),
            ),
          ],
        ),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            // Phần 1: Hình ảnh
            Stack(
              children: [
                ClipRRect(
                  borderRadius: const BorderRadius.only(
                    topLeft: Radius.circular(20),
                    topRight: Radius.circular(20),
                  ),
                  child: Image.network(
                    displayImage,
                    height: 180,
                    width: double.infinity,
                    fit: BoxFit.cover,
                    errorBuilder: (context, error, stackTrace) {
                      // Nếu link ảnh từ DB bị chết (404), nó sẽ tự động render ảnh này thay vì báo lỗi đỏ
                      return Image.network(
                        defaultHotelImage,
                        height: 180,
                        width: double.infinity,
                        fit: BoxFit.cover,
                      );
                    },
                  ),
                ),
                // Nút thả tim (Favorite)
                Positioned(
                  top: 12,
                  right: 12,
                  child: Container(
                    padding: const EdgeInsets.all(8),
                    decoration: const BoxDecoration(
                      color: Colors.white,
                      shape: BoxShape.circle,
                    ),
                    child: const Icon(
                      Icons.favorite_border_rounded,
                      color: Colors.redAccent,
                      size: 20,
                    ),
                  ),
                ),
                // Badge Đánh giá
                Positioned(
                  top: 12,
                  left: 12,
                  child: Container(
                    padding: const EdgeInsets.symmetric(
                      horizontal: 10,
                      vertical: 6,
                    ),
                    decoration: BoxDecoration(
                      color: Colors.black.withOpacity(0.6),
                      borderRadius: BorderRadius.circular(20),
                      border: Border.all(color: Colors.white.withOpacity(0.2)),
                    ),
                    child: Row(
                      mainAxisSize: MainAxisSize.min,
                      children: [
                        const Icon(
                          Icons.star_rounded,
                          color: Colors.amber,
                          size: 16,
                        ),
                        const SizedBox(width: 4),
                        Text(
                          '${hotel.averageRating ?? 4.5}',
                          style: GoogleFonts.dmSans(
                            color: Colors.white,
                            fontWeight: FontWeight.bold,
                            fontSize: 12,
                          ),
                        ),
                      ],
                    ),
                  ),
                ),
              ],
            ),

            // Phần 2: Nội dung chi tiết
            Padding(
              padding: const EdgeInsets.all(16),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Row(
                    mainAxisAlignment: MainAxisAlignment.spaceBetween,
                    children: [
                      Expanded(
                        child: Text(
                          hotel.name ?? 'Tên khách sạn',
                          style: GoogleFonts.dmSans(
                            fontSize: 18,
                            fontWeight: FontWeight.bold,
                            color: _kTextPrimary,
                          ),
                          maxLines: 1,
                          overflow: TextOverflow.ellipsis,
                        ),
                      ),
                    ],
                  ),
                  const SizedBox(height: 8),
                  Row(
                    children: [
                      const Icon(
                        Icons.location_on_rounded,
                        color: _kGreen,
                        size: 16,
                      ),
                      const SizedBox(width: 4),
                      Expanded(
                        child: Text(
                          hotel.province ?? 'Địa chỉ',
                          style: GoogleFonts.dmSans(
                            fontSize: 14,
                            color: _kTextSec,
                          ),
                          maxLines: 1,
                          overflow: TextOverflow.ellipsis,
                        ),
                      ),
                    ],
                  ),
                  const SizedBox(height: 12),
                  const Divider(color: Color(0xFFEEEEEE), height: 1),
                  const SizedBox(height: 12),
                  Row(
                    mainAxisAlignment: MainAxisAlignment.spaceBetween,
                    children: [
                      Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          Text(
                            'Giá từ',
                            style: GoogleFonts.dmSans(
                              fontSize: 12,
                              color: _kTextSec,
                            ),
                          ),
                          Row(
                            crossAxisAlignment: CrossAxisAlignment.end,
                            children: [
                              Text(
                                '${_formatCurrency(hotel.avgRoomPrice ?? 500000.0)}₫',
                                style: GoogleFonts.dmSans(
                                  fontSize: 18,
                                  fontWeight: FontWeight.w900,
                                  color: _kGreen,
                                ),
                              ),
                              Text(
                                ' /đêm',
                                style: GoogleFonts.dmSans(
                                  fontSize: 12,
                                  color: _kTextSec,
                                ),
                              ),
                            ],
                          ),
                        ],
                      ),
                      Container(
                        padding: const EdgeInsets.symmetric(
                          horizontal: 16,
                          vertical: 10,
                        ),
                        decoration: BoxDecoration(
                          color: _kGreen,
                          borderRadius: BorderRadius.circular(12),
                        ),
                        child: Text(
                          'Đặt ngay',
                          style: GoogleFonts.dmSans(
                            color: Colors.white,
                            fontWeight: FontWeight.bold,
                            fontSize: 14,
                          ),
                        ),
                      ),
                    ],
                  ),
                ],
              ),
            ),
          ],
        ),
      ),
    );
  }
}

class _OwnerStats {
  final int totalHotels;
  final int bookedRooms;
  final int vipBookedRooms;
  final int regularBookedRooms;

  const _OwnerStats({
    this.totalHotels = 0,
    this.bookedRooms = 0,
    this.vipBookedRooms = 0,
    this.regularBookedRooms = 0,
  });
}

// --- WIDGET: CHIP PROVINCE ---
class _ProvinceChip extends StatelessWidget {
  final String label;
  final bool isSelected;
  final VoidCallback onTap;

  const _ProvinceChip({
    required this.label,
    required this.isSelected,
    required this.onTap,
  });

  @override
  Widget build(BuildContext context) {
    return GestureDetector(
      onTap: onTap,
      child: AnimatedContainer(
        duration: const Duration(milliseconds: 200),
        padding: const EdgeInsets.symmetric(horizontal: 20, vertical: 10),
        decoration: BoxDecoration(
          color: isSelected ? _kGreen : Colors.white,
          borderRadius: BorderRadius.circular(999),
          border: Border.all(
            color: isSelected ? _kGreen : Colors.grey.shade300,
            width: 1,
          ),
          boxShadow: isSelected
              ? [
                  BoxShadow(
                    color: _kGreen.withOpacity(0.3),
                    blurRadius: 8,
                    offset: const Offset(0, 3),
                  ),
                ]
              : [],
        ),
        child: Center(
          child: Text(
            label,
            style: GoogleFonts.dmSans(
              color: isSelected ? Colors.white : _kTextPrimary,
              fontWeight: isSelected ? FontWeight.bold : FontWeight.w500,
              fontSize: 14,
            ),
          ),
        ),
      ),
    );
  }
}

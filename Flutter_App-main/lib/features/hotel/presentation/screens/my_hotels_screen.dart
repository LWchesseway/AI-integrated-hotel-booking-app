import 'package:flutter/material.dart';
import 'package:google_fonts/google_fonts.dart';
import '../../../../core/constants/app_colors.dart';
import '../../../../core/network/api_client.dart';
import '../../../../core/storage/auth_storage.dart';
import '../../../../injection.dart' as di;
import 'edit_hotel_screen.dart';

class MyHotelsScreen extends StatefulWidget {
  const MyHotelsScreen({super.key});

  @override
  State<MyHotelsScreen> createState() => _MyHotelsScreenState();
}

class _MyHotelsScreenState extends State<MyHotelsScreen> {
  final ApiClient _apiClient = di.sl<ApiClient>();
  final AuthStorage _authStorage = AuthStorage();

  List<dynamic> _hotels = [];
  bool _isLoading = true;

  @override
  void initState() {
    super.initState();
    _fetchMyHotels();
  }

  Future<void> _fetchMyHotels() async {
    setState(() => _isLoading = true);
    try {
      final token = await _authStorage.getAccessToken();
      final session = await _authStorage.getSession();
      if (session == null) {
        throw Exception('Không tìm thấy phiên đăng nhập');
      }

      final ownerId = session.userId;

      // Gọi lấy toàn bộ danh sách khách sạn
      final response = await _apiClient.get(
        '/api/hotels/all-with-province?pageIndex=1&pageSize=100',
        accessToken: token,
      );

      final List data = response['data'] ?? [];
      
      // Lọc ra các khách sạn do mình sở hữu
      final myHotels = data.where((hotel) => hotel['ownerId'] == ownerId).toList();

      setState(() {
        _hotels = myHotels;
        _isLoading = false;
      });
    } catch (e) {
      setState(() => _isLoading = false);
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(
            content: Text('Không thể tải danh sách khách sạn: $e'),
            backgroundColor: AppColors.error,
          ),
        );
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    const String defaultHotelImage =
        'https://images.unsplash.com/photo-1566073771259-6a8506099945?q=80&w=1000&auto=format&fit=crop';

    return Scaffold(
      backgroundColor: AppColors.scaffoldBg,
      appBar: AppBar(
        backgroundColor: Colors.transparent,
        elevation: 0,
        leading: IconButton(
          icon: const Icon(Icons.arrow_back_ios_new_rounded, color: AppColors.textPrimary),
          onPressed: () => Navigator.pop(context),
        ),
        title: Text(
          'Khách sạn của tôi',
          style: GoogleFonts.dmSans(
            color: AppColors.textPrimary,
            fontWeight: FontWeight.bold,
            fontSize: 20,
          ),
        ),
        centerTitle: true,
      ),
      body: RefreshIndicator(
        color: AppColors.greenPrimary,
        onRefresh: _fetchMyHotels,
        child: _isLoading
            ? const Center(
                child: CircularProgressIndicator(color: AppColors.greenPrimary),
              )
            : _hotels.isEmpty
                ? ListView(
                    physics: const AlwaysScrollableScrollPhysics(),
                    children: [
                      SizedBox(height: MediaQuery.of(context).size.height * 0.25),
                      Center(
                        child: Column(
                          children: [
                            Icon(
                              Icons.apartment_rounded,
                              size: 72,
                              color: AppColors.textHint.withOpacity(0.5),
                            ),
                            const SizedBox(height: 16),
                            Text(
                              'Bạn chưa sở hữu khách sạn nào',
                              style: GoogleFonts.dmSans(
                                color: AppColors.textSecondary,
                                fontWeight: FontWeight.bold,
                                fontSize: 16,
                              ),
                            ),
                            const SizedBox(height: 6),
                            Text(
                              'Hãy tạo khách sạn mới ngoài trang chủ!',
                              style: GoogleFonts.dmSans(
                                color: AppColors.textHint,
                                fontSize: 13,
                              ),
                            ),
                          ],
                        ),
                      ),
                    ],
                  )
                : ListView.builder(
                    physics: const AlwaysScrollableScrollPhysics(),
                    padding: const EdgeInsets.symmetric(horizontal: 20, vertical: 12),
                    itemCount: _hotels.length,
                    itemBuilder: (context, index) {
                      final hotel = _hotels[index];
                      final name = hotel['name'] ?? 'Không tên';
                      final street = hotel['street'] ?? 'Chưa cấu hình địa chỉ';
                      final imageUrl = hotel['imageUrl'] ?? hotel['image'] ?? defaultHotelImage;

                      return Container(
                        margin: const EdgeInsets.only(bottom: 20),
                        decoration: BoxDecoration(
                          color: AppColors.cardBg,
                          borderRadius: BorderRadius.circular(24),
                          border: Border.all(color: AppColors.divider),
                          boxShadow: [
                            BoxShadow(
                              color: Colors.black.withOpacity(0.03),
                              blurRadius: 16,
                              offset: const Offset(0, 8),
                            ),
                          ],
                        ),
                        child: Column(
                          crossAxisAlignment: CrossAxisAlignment.start,
                          children: [
                            // ẢNH KHÁCH SẠN
                            ClipRRect(
                              borderRadius: const BorderRadius.only(
                                topLeft: Radius.circular(24),
                                topRight: Radius.circular(24),
                              ),
                              child: Image.network(
                                imageUrl,
                                height: 160,
                                width: double.infinity,
                                fit: BoxFit.cover,
                                errorBuilder: (context, error, stackTrace) {
                                  return Image.network(
                                    defaultHotelImage,
                                    height: 160,
                                    width: double.infinity,
                                    fit: BoxFit.cover,
                                  );
                                },
                              ),
                            ),
                            Padding(
                              padding: const EdgeInsets.all(16),
                              child: Column(
                                crossAxisAlignment: CrossAxisAlignment.start,
                                children: [
                                  Text(
                                    name,
                                    style: GoogleFonts.dmSans(
                                      color: AppColors.textPrimary,
                                      fontWeight: FontWeight.bold,
                                      fontSize: 18,
                                    ),
                                    maxLines: 1,
                                    overflow: TextOverflow.ellipsis,
                                  ),
                                  const SizedBox(height: 6),
                                  Row(
                                    children: [
                                      const Icon(
                                        Icons.location_on_rounded,
                                        color: AppColors.greenMedium,
                                        size: 16,
                                      ),
                                      const SizedBox(width: 6),
                                      Expanded(
                                        child: Text(
                                          street,
                                          style: GoogleFonts.dmSans(
                                            color: AppColors.textSecondary,
                                            fontSize: 13,
                                          ),
                                          maxLines: 1,
                                          overflow: TextOverflow.ellipsis,
                                        ),
                                      ),
                                    ],
                                  ),
                                  const SizedBox(height: 14),
                                  const Divider(color: AppColors.divider, height: 1, thickness: 1),
                                  const SizedBox(height: 12),
                                  Row(
                                    mainAxisAlignment: MainAxisAlignment.end,
                                    children: [
                                      ElevatedButton.icon(
                                        onPressed: () async {
                                          final edited = await Navigator.push(
                                            context,
                                            MaterialPageRoute(
                                              builder: (_) => EditHotelScreen(hotel: hotel),
                                            ),
                                          );
                                          if (edited == true) {
                                            _fetchMyHotels();
                                          }
                                        },
                                        icon: const Icon(Icons.edit_note_rounded, color: Colors.white, size: 20),
                                        label: Text(
                                          'Chỉnh sửa',
                                          style: GoogleFonts.dmSans(
                                            color: Colors.white,
                                            fontWeight: FontWeight.bold,
                                            fontSize: 13,
                                          ),
                                        ),
                                        style: ElevatedButton.styleFrom(
                                          backgroundColor: AppColors.greenPrimary,
                                          shape: RoundedRectangleBorder(
                                            borderRadius: BorderRadius.circular(12),
                                          ),
                                          elevation: 2,
                                          shadowColor: AppColors.greenPrimary.withOpacity(0.2),
                                        ),
                                      ),
                                    ],
                                  ),
                                ],
                              ),
                            ),
                          ],
                        ),
                      );
                    },
                  ),
      ),
    );
  }
}

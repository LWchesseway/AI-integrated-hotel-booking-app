import 'package:flutter/material.dart';
import 'package:google_fonts/google_fonts.dart';
import '../../../../core/constants/app_colors.dart';
import '../../../../core/network/api_client.dart';
import '../../../../core/storage/auth_storage.dart';
import '../../../../injection.dart' as di;

class CreateHotelScreen extends StatefulWidget {
  const CreateHotelScreen({super.key});

  @override
  State<CreateHotelScreen> createState() => _CreateHotelScreenState();
}

class _CreateHotelScreenState extends State<CreateHotelScreen> {
  final _formKey = GlobalKey<FormState>();
  final ApiClient _apiClient = di.sl<ApiClient>();
  final AuthStorage _authStorage = AuthStorage();

  // Form Controllers
  final TextEditingController _nameController = TextEditingController();
  final TextEditingController _streetController = TextEditingController();
  final TextEditingController _phoneController = TextEditingController();
  final TextEditingController _descriptionController = TextEditingController();
  final TextEditingController _imageUrlController = TextEditingController();

  // Wards and Location selection
  List<dynamic> _wards = [];
  int? _selectedWardId;
  bool _isLoadingWards = true;

  // Rooms counters & prices
  int _vipCount = 2;
  int _regularCount = 5;
  final TextEditingController _vipPriceController = TextEditingController(text: '1500000');
  final TextEditingController _regularPriceController = TextEditingController(text: '600000');

  // Image Selection
  final List<String> _presetImages = [
    'https://images.unsplash.com/photo-1566073771259-6a8506099945?q=80&w=600&auto=format&fit=crop',
    'https://images.unsplash.com/photo-1520250497591-112f2f40a3f4?q=80&w=600&auto=format&fit=crop',
    'https://images.unsplash.com/photo-1542314831-068cd1dbfeeb?q=80&w=600&auto=format&fit=crop',
    'https://images.unsplash.com/photo-1584132967334-10e028bd69f7?q=80&w=600&auto=format&fit=crop',
  ];
  late String _selectedImage;

  bool _isSubmitting = false;

  @override
  void initState() {
    super.initState();
    _selectedImage = _presetImages[0];
    _imageUrlController.text = _selectedImage;
    _fetchWards();
  }

  @override
  void dispose() {
    _nameController.dispose();
    _streetController.dispose();
    _phoneController.dispose();
    _descriptionController.dispose();
    _imageUrlController.dispose();
    _vipPriceController.dispose();
    _regularPriceController.dispose();
    super.dispose();
  }

  Future<void> _fetchWards() async {
    try {
      final token = await _authStorage.getAccessToken();
      final response = await _apiClient.get(
        '/api/wards?pageIndex=1&pageSize=100',
        accessToken: token,
      );
      final List data = response['data'] ?? [];
      setState(() {
        _wards = data;
        _isLoadingWards = false;
        if (_wards.isNotEmpty) {
          _selectedWardId = _wards[0]['id'];
        }
      });
    } catch (e) {
      setState(() {
        _isLoadingWards = false;
      });
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(
            content: Text('Không thể tải danh sách địa điểm: $e'),
            backgroundColor: AppColors.error,
          ),
        );
      }
    }
  }

  Future<void> _submitForm() async {
    if (!_formKey.currentState!.validate()) return;
    if (_selectedWardId == null) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text('Vui lòng chọn Phường/Xã'),
          backgroundColor: AppColors.error,
        ),
      );
      return;
    }

    setState(() => _isSubmitting = true);

    try {
      final token = await _authStorage.getAccessToken();

      // Bước 1: Tạo Khách Sạn cơ bản
      final hotelResponse = await _apiClient.post(
        '/api/hotels/owner',
        body: {
          'wardId': _selectedWardId,
          'name': _nameController.text.trim(),
          'street': _streetController.text.trim(),
          'phone': _phoneController.text.trim(),
          'description': _descriptionController.text.trim(),
          'status': 1,
        },
        accessToken: token,
      );

      final hotelData = hotelResponse['data'];
      final hotelId = hotelData['id'] as int;

      // Bước 2: Tạo Ảnh Khách Sạn
      if (_imageUrlController.text.trim().isNotEmpty) {
        await _apiClient.post(
          '/api/hotels/$hotelId/images',
          body: {
            'imageUrl': _imageUrlController.text.trim(),
            'isPrimary': true,
            'sortOrder': 1,
          },
          accessToken: token,
        );
      }

      // Bước 3: Tạo Phòng song song để tối ưu hiệu năng
      final List<Future> roomFutures = [];
      final double vipPrice = double.tryParse(_vipPriceController.text) ?? 1500000;
      final double regPrice = double.tryParse(_regularPriceController.text) ?? 600000;

      // Thêm phòng VIP
      for (int i = 1; i <= _vipCount; i++) {
        roomFutures.add(_apiClient.post(
          '/api/rooms',
          body: {
            'hotelId': hotelId,
            'roomTypeId': 2, // VIP/Deluxe
            'roomNumber': 'VIP-${100 + i}',
            'capacity': 2,
            'price': vipPrice,
            'status': 1,
          },
          accessToken: token,
        ));
      }

      // Thêm phòng Thường
      for (int j = 1; j <= _regularCount; j++) {
        roomFutures.add(_apiClient.post(
          '/api/rooms',
          body: {
            'hotelId': hotelId,
            'roomTypeId': 1, // Regular/Standard
            'roomNumber': 'STD-${100 + j}',
            'capacity': 2,
            'price': regPrice,
            'status': 1,
          },
          accessToken: token,
        ));
      }

      await Future.wait(roomFutures);

      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(
            content: Text('Tạo khách sạn và phòng thành công!'),
            backgroundColor: AppColors.success,
          ),
        );
        Navigator.pop(context, true); // Trả về true để home_screen tự load lại
      }
    } catch (e) {
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(
            content: Text('Lỗi khi tạo khách sạn: $e'),
            backgroundColor: AppColors.error,
          ),
        );
      }
    } finally {
      if (mounted) {
        setState(() => _isSubmitting = false);
      }
    }
  }

  @override
  Widget build(BuildContext context) {
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
          'Tạo khách sạn mới',
          style: GoogleFonts.dmSans(
            color: AppColors.textPrimary,
            fontWeight: FontWeight.bold,
            fontSize: 20,
          ),
        ),
        centerTitle: true,
      ),
      body: _isLoadingWards
          ? const Center(
              child: CircularProgressIndicator(color: AppColors.greenPrimary),
            )
          : Stack(
              children: [
                SingleChildScrollView(
                  padding: const EdgeInsets.fromLTRB(20, 10, 20, 40),
                  child: Form(
                    key: _formKey,
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        // --- THẺ CHỌN ẢNH ---
                        _buildSectionHeader('1. Ảnh đại diện khách sạn', Icons.image_rounded),
                        const SizedBox(height: 12),
                        Container(
                          height: 160,
                          width: double.infinity,
                          decoration: BoxDecoration(
                            borderRadius: BorderRadius.circular(20),
                            boxShadow: [
                              BoxShadow(
                                color: Colors.black.withOpacity(0.05),
                                blurRadius: 10,
                                offset: const Offset(0, 4),
                              ),
                            ],
                            image: DecorationImage(
                              image: NetworkImage(_imageUrlController.text),
                              fit: BoxFit.cover,
                              onError: (e, s) {},
                            ),
                          ),
                          child: Container(
                            decoration: BoxDecoration(
                              borderRadius: BorderRadius.circular(20),
                              color: Colors.black.withOpacity(0.2),
                            ),
                          ),
                        ),
                        const SizedBox(height: 14),
                        Text(
                          'Chọn ảnh từ thư viện gợi ý hoặc dán liên kết:',
                          style: GoogleFonts.dmSans(
                            color: AppColors.textSecondary,
                            fontSize: 13,
                          ),
                        ),
                        const SizedBox(height: 10),
                        SizedBox(
                          height: 70,
                          child: ListView.builder(
                            scrollDirection: Axis.horizontal,
                            itemCount: _presetImages.length,
                            itemBuilder: (context, index) {
                              final img = _presetImages[index];
                              final isSelected = _selectedImage == img;
                              return GestureDetector(
                                onTap: () {
                                  setState(() {
                                    _selectedImage = img;
                                    _imageUrlController.text = img;
                                  });
                                },
                                child: Container(
                                  margin: const EdgeInsets.only(right: 12),
                                  width: 90,
                                  decoration: BoxDecoration(
                                    borderRadius: BorderRadius.circular(12),
                                    border: isSelected
                                        ? Border.all(color: AppColors.greenPrimary, width: 3)
                                        : null,
                                    image: DecorationImage(
                                      image: NetworkImage(img),
                                      fit: BoxFit.cover,
                                    ),
                                  ),
                                ),
                              );
                            },
                          ),
                        ),
                        const SizedBox(height: 12),
                        _buildTextField(
                          controller: _imageUrlController,
                          label: 'Đường dẫn ảnh khách sạn (URL)',
                          hint: 'https://...',
                          icon: Icons.link_rounded,
                          onChanged: (val) {
                            setState(() {});
                          },
                        ),
                        const SizedBox(height: 24),

                        // --- THÔNG TIN CƠ BẢN ---
                        _buildSectionHeader('2. Thông tin khách sạn', Icons.info_outline_rounded),
                        const SizedBox(height: 14),
                        _buildTextField(
                          controller: _nameController,
                          label: 'Tên khách sạn',
                          hint: 'Nhập tên khách sạn...',
                          icon: Icons.domain_rounded,
                          validator: (val) =>
                              (val == null || val.trim().isEmpty) ? 'Vui lòng nhập tên khách sạn' : null,
                        ),
                        const SizedBox(height: 14),
                        _buildDropdownField(
                          label: 'Địa điểm (Phường / Xã)',
                          value: _selectedWardId,
                          icon: Icons.location_on_rounded,
                          items: _wards.map((ward) {
                            return DropdownMenuItem<int>(
                              value: ward['id'],
                              child: Text(
                                ward['name'] ?? '',
                                style: GoogleFonts.dmSans(
                                  color: AppColors.textPrimary,
                                  fontSize: 14,
                                ),
                              ),
                            );
                          }).toList(),
                          onChanged: (val) {
                            setState(() => _selectedWardId = val);
                          },
                        ),
                        const SizedBox(height: 14),
                        _buildTextField(
                          controller: _streetController,
                          label: 'Tên đường / Số nhà',
                          hint: 'Số 12 Đường Lê Lợi...',
                          icon: Icons.streetview_rounded,
                          validator: (val) =>
                              (val == null || val.trim().isEmpty) ? 'Vui lòng nhập địa chỉ đường' : null,
                        ),
                        const SizedBox(height: 14),
                        _buildTextField(
                          controller: _phoneController,
                          label: 'Số điện thoại liên hệ',
                          hint: 'Nhập số điện thoại...',
                          icon: Icons.phone_rounded,
                          keyboardType: TextInputType.phone,
                          validator: (val) =>
                              (val == null || val.trim().isEmpty) ? 'Vui lòng nhập số điện thoại' : null,
                        ),
                        const SizedBox(height: 14),
                        _buildTextField(
                          controller: _descriptionController,
                          label: 'Mô tả ngắn',
                          hint: 'Nhập giới thiệu về khách sạn...',
                          icon: Icons.description_rounded,
                          maxLines: 3,
                        ),
                        const SizedBox(height: 24),

                        // --- THÔNG TIN PHÒNG ---
                        _buildSectionHeader('3. Cấu hình phòng & Giá tiền', Icons.meeting_room_rounded),
                        const SizedBox(height: 14),

                        // Thẻ phòng VIP
                        _buildRoomCard(
                          title: 'PHÒNG VIP (Deluxe Room)',
                          color: AppColors.warning,
                          count: _vipCount,
                          priceController: _vipPriceController,
                          onIncrement: () => setState(() => _vipCount++),
                          onDecrement: () {
                            if (_vipCount > 0) setState(() => _vipCount--);
                          },
                        ),
                        const SizedBox(height: 14),

                        // Thẻ phòng Thường
                        _buildRoomCard(
                          title: 'PHÒNG THƯỜNG (Standard Room)',
                          color: AppColors.greenPrimary,
                          count: _regularCount,
                          priceController: _regularPriceController,
                          onIncrement: () => setState(() => _regularCount++),
                          onDecrement: () {
                            if (_regularCount > 0) setState(() => _regularCount--);
                          },
                        ),
                        const SizedBox(height: 35),

                        // --- NÚT HOÀN TẤT ---
                        SizedBox(
                          width: double.infinity,
                          height: 55,
                          child: ElevatedButton(
                            onPressed: _submitForm,
                            style: ElevatedButton.styleFrom(
                              backgroundColor: AppColors.greenPrimary,
                              shape: RoundedRectangleBorder(
                                borderRadius: BorderRadius.circular(16),
                              ),
                              elevation: 4,
                              shadowColor: AppColors.greenPrimary.withOpacity(0.3),
                            ),
                            child: Text(
                              'Tạo Khách Sạn & Khởi Tạo Phòng',
                              style: GoogleFonts.dmSans(
                                color: Colors.white,
                                fontWeight: FontWeight.bold,
                                fontSize: 16,
                              ),
                            ),
                          ),
                        ),
                      ],
                    ),
                  ),
                ),
                if (_isSubmitting)
                  Container(
                    color: Colors.black.withOpacity(0.5),
                    child: Center(
                      child: Card(
                        shape: RoundedRectangleBorder(
                          borderRadius: BorderRadius.circular(20),
                        ),
                        color: Colors.white,
                        child: Padding(
                          padding: const EdgeInsets.symmetric(horizontal: 30, vertical: 24),
                          child: Column(
                            mainAxisSize: MainAxisSize.min,
                            children: [
                              const CircularProgressIndicator(color: AppColors.greenPrimary),
                              const SizedBox(height: 20),
                              Text(
                                'Đang khởi tạo khách sạn...',
                                style: GoogleFonts.dmSans(
                                  fontWeight: FontWeight.bold,
                                  color: AppColors.textPrimary,
                                ),
                              ),
                              const SizedBox(height: 6),
                              Text(
                                'Hệ thống đang cấu hình các phòng VIP và phòng Thường tự động',
                                textAlign: TextAlign.center,
                                style: GoogleFonts.dmSans(
                                  fontSize: 12,
                                  color: AppColors.textSecondary,
                                ),
                              ),
                            ],
                          ),
                        ),
                      ),
                    ),
                  ),
              ],
            ),
    );
  }

  Widget _buildSectionHeader(String title, IconData icon) {
    return Row(
      children: [
        Icon(icon, color: AppColors.greenPrimary, size: 20),
        const SizedBox(width: 8),
        Text(
          title,
          style: GoogleFonts.dmSans(
            color: AppColors.textPrimary,
            fontWeight: FontWeight.w800,
            fontSize: 15,
            letterSpacing: 0.2,
          ),
        ),
      ],
    );
  }

  Widget _buildTextField({
    required TextEditingController controller,
    required String label,
    required String hint,
    required IconData icon,
    TextInputType keyboardType = TextInputType.text,
    int maxLines = 1,
    String? Function(String?)? validator,
    void Function(String)? onChanged,
  }) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          label,
          style: GoogleFonts.dmSans(
            color: AppColors.textPrimary,
            fontWeight: FontWeight.w600,
            fontSize: 13,
          ),
        ),
        const SizedBox(height: 6),
        TextFormField(
          controller: controller,
          keyboardType: keyboardType,
          maxLines: maxLines,
          validator: validator,
          onChanged: onChanged,
          style: GoogleFonts.dmSans(color: AppColors.textPrimary, fontSize: 14),
          decoration: InputDecoration(
            hintText: hint,
            hintStyle: GoogleFonts.dmSans(color: AppColors.textHint, fontSize: 13),
            prefixIcon: Icon(icon, color: AppColors.greenMedium, size: 20),
            filled: true,
            fillColor: AppColors.cardBg,
            contentPadding: const EdgeInsets.symmetric(horizontal: 16, vertical: 14),
            border: OutlineInputBorder(
              borderRadius: BorderRadius.circular(16),
              borderSide: BorderSide.none,
            ),
            enabledBorder: OutlineInputBorder(
              borderRadius: BorderRadius.circular(16),
              borderSide: const BorderSide(color: AppColors.divider, width: 1),
            ),
            focusedBorder: OutlineInputBorder(
              borderRadius: BorderRadius.circular(16),
              borderSide: const BorderSide(color: AppColors.greenPrimary, width: 1.5),
            ),
          ),
        ),
      ],
    );
  }

  Widget _buildDropdownField({
    required String label,
    required int? value,
    required IconData icon,
    required List<DropdownMenuItem<int>> items,
    required void Function(int?) onChanged,
  }) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          label,
          style: GoogleFonts.dmSans(
            color: AppColors.textPrimary,
            fontWeight: FontWeight.w600,
            fontSize: 13,
          ),
        ),
        const SizedBox(height: 6),
        DropdownButtonFormField<int>(
          value: value,
          items: items,
          onChanged: onChanged,
          style: GoogleFonts.dmSans(color: AppColors.textPrimary, fontSize: 14),
          decoration: InputDecoration(
            prefixIcon: Icon(icon, color: AppColors.greenMedium, size: 20),
            filled: true,
            fillColor: AppColors.cardBg,
            contentPadding: const EdgeInsets.symmetric(horizontal: 16, vertical: 14),
            border: OutlineInputBorder(
              borderRadius: BorderRadius.circular(16),
              borderSide: BorderSide.none,
            ),
            enabledBorder: OutlineInputBorder(
              borderRadius: BorderRadius.circular(16),
              borderSide: const BorderSide(color: AppColors.divider, width: 1),
            ),
            focusedBorder: OutlineInputBorder(
              borderRadius: BorderRadius.circular(16),
              borderSide: const BorderSide(color: AppColors.greenPrimary, width: 1.5),
            ),
          ),
        ),
      ],
    );
  }

  Widget _buildRoomCard({
    required String title,
    required Color color,
    required int count,
    required TextEditingController priceController,
    required VoidCallback onIncrement,
    required VoidCallback onDecrement,
  }) {
    return Container(
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        color: AppColors.cardBg,
        borderRadius: BorderRadius.circular(20),
        border: Border.all(color: AppColors.divider),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            children: [
              Container(
                width: 8,
                height: 20,
                decoration: BoxDecoration(
                  color: color,
                  borderRadius: BorderRadius.circular(4),
                ),
              ),
              const SizedBox(width: 8),
              Text(
                title,
                style: GoogleFonts.dmSans(
                  fontWeight: FontWeight.bold,
                  fontSize: 12,
                  color: AppColors.textPrimary,
                  letterSpacing: 0.5,
                ),
              ),
            ],
          ),
          const SizedBox(height: 14),
          Row(
            children: [
              Expanded(
                flex: 4,
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      'Số lượng phòng',
                      style: GoogleFonts.dmSans(
                        fontSize: 11,
                        color: AppColors.textSecondary,
                        fontWeight: FontWeight.w600,
                      ),
                    ),
                    const SizedBox(height: 6),
                    Row(
                      children: [
                        IconButton(
                          onPressed: onDecrement,
                          icon: const Icon(Icons.remove_circle_outline_rounded, color: AppColors.brownAccent),
                        ),
                        Text(
                          '$count',
                          style: GoogleFonts.dmSans(
                            fontSize: 16,
                            fontWeight: FontWeight.bold,
                            color: AppColors.textPrimary,
                          ),
                        ),
                        IconButton(
                          onPressed: onIncrement,
                          icon: const Icon(Icons.add_circle_outline_rounded, color: AppColors.greenPrimary),
                        ),
                      ],
                    ),
                  ],
                ),
              ),
              const SizedBox(width: 16),
              Expanded(
                flex: 5,
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      'Đơn giá / Đêm (VND)',
                      style: GoogleFonts.dmSans(
                        fontSize: 11,
                        color: AppColors.textSecondary,
                        fontWeight: FontWeight.w600,
                      ),
                    ),
                    const SizedBox(height: 6),
                    TextFormField(
                      controller: priceController,
                      keyboardType: TextInputType.number,
                      style: GoogleFonts.dmSans(
                        fontWeight: FontWeight.bold,
                        fontSize: 14,
                        color: AppColors.textPrimary,
                      ),
                      decoration: InputDecoration(
                        filled: true,
                        fillColor: Colors.white,
                        contentPadding: const EdgeInsets.symmetric(horizontal: 12, vertical: 10),
                        enabledBorder: OutlineInputBorder(
                          borderRadius: BorderRadius.circular(12),
                          borderSide: const BorderSide(color: AppColors.divider),
                        ),
                        focusedBorder: OutlineInputBorder(
                          borderRadius: BorderRadius.circular(12),
                          borderSide: const BorderSide(color: AppColors.greenPrimary),
                        ),
                      ),
                    ),
                  ],
                ),
              ),
            ],
          ),
        ],
      ),
    );
  }
}

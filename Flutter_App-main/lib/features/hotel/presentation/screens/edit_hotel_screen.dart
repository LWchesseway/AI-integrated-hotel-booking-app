import 'package:flutter/material.dart';
import 'package:google_fonts/google_fonts.dart';
import '../../../../core/constants/app_colors.dart';
import '../../../../core/network/api_client.dart';
import '../../../../core/storage/auth_storage.dart';
import '../../../../injection.dart' as di;

class EditHotelScreen extends StatefulWidget {
  final Map<String, dynamic> hotel;
  const EditHotelScreen({super.key, required this.hotel});

  @override
  State<EditHotelScreen> createState() => _EditHotelScreenState();
}

class _EditHotelScreenState extends State<EditHotelScreen> {
  final _formKey = GlobalKey<FormState>();
  final ApiClient _apiClient = di.sl<ApiClient>();
  final AuthStorage _authStorage = AuthStorage();

  // Form Controllers
  late TextEditingController _nameController;
  late TextEditingController _streetController;
  late TextEditingController _phoneController;
  late TextEditingController _descriptionController;
  late TextEditingController _imageUrlController;

  // Wards and Location selection
  List<dynamic> _wards = [];
  int? _selectedWardId;
  bool _isLoadingWards = true;

  // Current rooms list and prices
  List<dynamic> _vipRooms = [];
  List<dynamic> _regularRooms = [];
  final TextEditingController _vipPriceController = TextEditingController(text: '1500000');
  final TextEditingController _regularPriceController = TextEditingController(text: '600000');
  bool _isLoadingRooms = true;

  // Images tracking
  List<dynamic> _hotelImages = [];

  // Preset images
  final List<String> _presetImages = [
    'https://images.unsplash.com/photo-1566073771259-6a8506099945?q=80&w=600&auto=format&fit=crop',
    'https://images.unsplash.com/photo-1520250497591-112f2f40a3f4?q=80&w=600&auto=format&fit=crop',
    'https://images.unsplash.com/photo-1542314831-068cd1dbfeeb?q=80&w=600&auto=format&fit=crop',
    'https://images.unsplash.com/photo-1584132967334-10e028bd69f7?q=80&w=600&auto=format&fit=crop',
  ];
  String? _selectedImage;

  bool _isSubmitting = false;

  @override
  void initState() {
    super.initState();
    final hotel = widget.hotel;
    _nameController = TextEditingController(text: hotel['name'] ?? '');
    _streetController = TextEditingController(text: hotel['street'] ?? '');
    _phoneController = TextEditingController(text: hotel['phone'] ?? '');
    _descriptionController = TextEditingController(text: hotel['description'] ?? '');
    
    final img = hotel['imageUrl'] ?? hotel['image'] ?? _presetImages[0];
    _imageUrlController = TextEditingController(text: img);
    _selectedImage = _presetImages.contains(img) ? img : null;

    _selectedWardId = hotel['wardId'];

    _loadData();
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

  Future<void> _loadData() async {
    setState(() {
      _isLoadingWards = true;
      _isLoadingRooms = true;
    });

    try {
      final token = await _authStorage.getAccessToken();
      final hotelId = widget.hotel['id'] as int;

      // 1. Tải danh sách Phường/Xã
      final wardsRes = await _apiClient.get(
        '/api/wards?pageIndex=1&pageSize=100',
        accessToken: token,
      );
      final List wardsData = wardsRes['data'] ?? [];

      // 2. Tải danh sách phòng của khách sạn để lấy giá hiện tại
      final roomsRes = await _apiClient.get(
        '/api/rooms/by-hotel?hotelId=$hotelId',
        accessToken: token,
      );
      final List roomsData = roomsRes['data'] ?? [];

      // 3. Tải danh sách ảnh của khách sạn
      final imagesRes = await _apiClient.get(
        '/api/hotels/$hotelId/images',
        accessToken: token,
      );
      final List imagesData = imagesRes['data'] ?? [];

      final vip = roomsData.where((r) => r['roomTypeId'] == 2).toList();
      final reg = roomsData.where((r) => r['roomTypeId'] == 1).toList();

      setState(() {
        _wards = wardsData;
        _isLoadingWards = false;

        _vipRooms = vip;
        _regularRooms = reg;
        _isLoadingRooms = false;

        _hotelImages = imagesData;

        if (_vipRooms.isNotEmpty) {
          final price = (_vipRooms[0]['price'] as num).toInt();
          _vipPriceController.text = price.toString();
        }
        if (_regularRooms.isNotEmpty) {
          final price = (_regularRooms[0]['price'] as num).toInt();
          _regularPriceController.text = price.toString();
        }
      });
    } catch (e) {
      setState(() {
        _isLoadingWards = false;
        _isLoadingRooms = false;
      });
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(
            content: Text('Lỗi khi tải thông tin: $e'),
            backgroundColor: AppColors.error,
          ),
        );
      }
    }
  }

  Future<void> _saveForm() async {
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
      final hotelId = widget.hotel['id'] as int;

      // Bước 1: Cập nhật thông tin khách sạn
      await _apiClient.put(
        '/api/hotels/owner/$hotelId',
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

      // Bước 2: Cập nhật / Thêm Ảnh Khách Sạn
      final newImgUrl = _imageUrlController.text.trim();
      if (newImgUrl.isNotEmpty) {
        if (_hotelImages.isNotEmpty) {
          // Đã có ảnh -> Cập nhật ảnh hiện tại
          final imageId = _hotelImages[0]['id'] as int;
          await _apiClient.put(
            '/api/hotels/$hotelId/images/$imageId',
            body: {
              'imageUrl': newImgUrl,
              'isPrimary': true,
              'sortOrder': 1,
            },
            accessToken: token,
          );
        } else {
          // Chưa có ảnh -> Thêm mới
          await _apiClient.post(
            '/api/hotels/$hotelId/images',
            body: {
              'imageUrl': newImgUrl,
              'isPrimary': true,
              'sortOrder': 1,
            },
            accessToken: token,
          );
        }
      }

      // Bước 3: Cập nhật giá cho toàn bộ phòng song song
      final List<Future> roomFutures = [];
      final double newVipPrice = double.tryParse(_vipPriceController.text) ?? 1500000;
      final double newRegPrice = double.tryParse(_regularPriceController.text) ?? 600000;

      // Cập nhật các phòng VIP
      for (final r in _vipRooms) {
        final roomId = r['id'] as int;
        roomFutures.add(_apiClient.put(
          '/api/rooms/$roomId',
          body: {
            'roomTypeId': 2,
            'roomNumber': r['roomNumber'] ?? 'VIP',
            'capacity': (r['capacity'] as num?)?.toInt() ?? 2,
            'price': newVipPrice,
            'status': (r['status'] as num?)?.toInt() ?? 1,
            'isDeleted': false,
          },
          accessToken: token,
        ));
      }

      // Cập nhật các phòng Thường
      for (final r in _regularRooms) {
        final roomId = r['id'] as int;
        roomFutures.add(_apiClient.put(
          '/api/rooms/$roomId',
          body: {
            'roomTypeId': 1,
            'roomNumber': r['roomNumber'] ?? 'STD',
            'capacity': (r['capacity'] as num?)?.toInt() ?? 2,
            'price': newRegPrice,
            'status': (r['status'] as num?)?.toInt() ?? 1,
            'isDeleted': false,
          },
          accessToken: token,
        ));
      }

      await Future.wait(roomFutures);

      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(
            content: Text('Lưu thông tin khách sạn thành công!'),
            backgroundColor: AppColors.success,
          ),
        );
        Navigator.pop(context, true); // Trả về true để tải lại danh sách
      }
    } catch (e) {
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(
            content: Text('Lỗi khi lưu thông tin: $e'),
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
          'Chỉnh sửa khách sạn',
          style: GoogleFonts.dmSans(
            color: AppColors.textPrimary,
            fontWeight: FontWeight.bold,
            fontSize: 20,
          ),
        ),
        centerTitle: true,
      ),
      body: (_isLoadingWards || _isLoadingRooms)
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
                          'Chọn ảnh nhanh từ gợi ý hoặc dán liên kết mới:',
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
                            setState(() {
                              _selectedImage = _presetImages.contains(val) ? val : null;
                            });
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
                        _buildSectionHeader('3. Giá phòng khách sạn', Icons.meeting_room_rounded),
                        const SizedBox(height: 14),

                        // Thẻ phòng VIP
                        _buildRoomPriceCard(
                          title: 'PHÒNG VIP (Deluxe Room)',
                          color: AppColors.warning,
                          count: _vipRooms.length,
                          priceController: _vipPriceController,
                        ),
                        const SizedBox(height: 14),

                        // Thẻ phòng Thường
                        _buildRoomPriceCard(
                          title: 'PHÒNG THƯỜNG (Standard Room)',
                          color: AppColors.greenPrimary,
                          count: _regularRooms.length,
                          priceController: _regularPriceController,
                        ),
                        const SizedBox(height: 35),

                        // --- NÚT LƯU THAY ĐỔI ---
                        SizedBox(
                          width: double.infinity,
                          height: 55,
                          child: ElevatedButton(
                            onPressed: _saveForm,
                            style: ElevatedButton.styleFrom(
                              backgroundColor: AppColors.greenPrimary,
                              shape: RoundedRectangleBorder(
                                borderRadius: BorderRadius.circular(16),
                              ),
                              elevation: 4,
                              shadowColor: AppColors.greenPrimary.withOpacity(0.3),
                            ),
                            child: Text(
                              'Lưu thay đổi',
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
                                'Đang lưu thay đổi...',
                                style: GoogleFonts.dmSans(
                                  fontWeight: FontWeight.bold,
                                  color: AppColors.textPrimary,
                                ),
                              ),
                              const SizedBox(height: 6),
                              Text(
                                'Đang áp dụng mức giá phòng mới cho toàn bộ các phòng',
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

  Widget _buildRoomPriceCard({
    required String title,
    required Color color,
    required int count,
    required TextEditingController priceController,
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
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
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
              Container(
                padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 3),
                decoration: BoxDecoration(
                  color: color.withOpacity(0.1),
                  borderRadius: BorderRadius.circular(8),
                ),
                child: Text(
                  'Tổng số: $count phòng',
                  style: GoogleFonts.dmSans(
                    color: color,
                    fontWeight: FontWeight.bold,
                    fontSize: 10,
                  ),
                ),
              ),
            ],
          ),
          const SizedBox(height: 14),
          Row(
            children: [
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      'Đơn giá hiện tại / Đêm (VND)',
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

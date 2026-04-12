# 📚 API Endpoints Reference - Hotel & Parking Management System

## 🏨 Hotel Endpoints

### Get Hotels
```
GET /api/hotels
Description: Lấy tất cả khách sạn
Response: ApiResponseDto<IEnumerable<HotelDto>>
```

```
GET /api/hotels/paged?pageIndex=1&pageSize=10
Description: Lấy khách sạn theo phân trang
Response: ApiPagedResponseDto<HotelDto>
```

```
GET /api/hotels/{id}
Description: Lấy chi tiết khách sạn kèm danh sách phòng
Response: ApiResponseDto<HotelDetailDto>
```

```
GET /api/hotels/by-name/{name}
Description: Lấy khách sạn theo tên
Response: ApiResponseDto<HotelDto>
```

```
GET /api/hotels/by-province/{province}
Description: Lấy danh sách khách sạn theo tỉnh/thành phố
Response: ApiResponseDto<IEnumerable<HotelDto>>
```

### Create, Update, Delete Hotels
```
POST /api/hotels
Body: CreateHotelDto
Description: Tạo khách sạn mới
Response: ApiResponseDto<HotelDto>

Example:
{
  "name": "Khách sạn Gold",
  "street": "123 Đường Lê Lợi",
  "ward": "Phường 1",
  "province": "TP. Hồ Chí Minh",
  "phone": "0123456789",
  "description": "Khách sạn 5 sao"
}
```

```
PUT /api/hotels/{id}
Body: UpdateHotelDto
Description: Cập nhật thông tin khách sạn
Response: ApiResponseDto<HotelDto>
```

```
DELETE /api/hotels/{id}
Description: Xóa khách sạn
Response: ApiResponseDto<bool>
```

```
PATCH /api/hotels/{id}/status
Body: int (status code)
Description: Thay đổi trạng thái khách sạn
Response: ApiResponseDto<HotelDto>
```

---

## 🛏️ Room Endpoints

### Get Rooms
```
GET /api/rooms
Description: Lấy tất cả phòng
Response: ApiResponseDto<IEnumerable<RoomDto>>
```

```
GET /api/rooms/paged?pageIndex=1&pageSize=10
Description: Lấy phòng theo phân trang
Response: ApiPagedResponseDto<RoomDto>
```

```
GET /api/rooms/{id}
Description: Lấy chi tiết phòng kèm ảnh và đánh giá
Response: ApiResponseDto<RoomDetailDto>
```

```
GET /api/rooms/hotel/{hotelId}
Description: Lấy danh sách phòng theo khách sạn
Response: ApiResponseDto<IEnumerable<RoomDto>>
```

```
GET /api/rooms/available?hotelId=1&checkIn=2024-01-01&checkOut=2024-01-05
Description: Lấy danh sách phòng trống trong khoảng thời gian
Response: ApiResponseDto<IEnumerable<RoomDto>>
```

### Create, Update, Delete Rooms
```
POST /api/rooms
Body: CreateRoomDto
Description: Tạo phòng mới
Response: ApiResponseDto<RoomDto>

Example:
{
  "hotelId": 1,
  "roomTypeId": 1,
  "roomNumber": "101",
  "capacity": 2
}
```

```
PUT /api/rooms/{id}
Body: UpdateRoomDto
Description: Cập nhật thông tin phòng
Response: ApiResponseDto<RoomDto>
```

```
DELETE /api/rooms/{id}
Description: Xóa phòng
Response: ApiResponseDto<bool>
```

```
PATCH /api/rooms/{id}/status
Body: int (status code)
Description: Thay đổi trạng thái phòng
Response: ApiResponseDto<RoomDto>
```

---

## 📅 Booking Endpoints

### Get Bookings
```
GET /api/bookings
Description: Lấy tất cả đặt phòng
Response: ApiResponseDto<IEnumerable<BookingDto>>
```

```
GET /api/bookings/paged?pageIndex=1&pageSize=10
Description: Lấy đặt phòng theo phân trang
Response: ApiPagedResponseDto<BookingDto>
```

```
GET /api/bookings/{id}
Description: Lấy chi tiết đặt phòng kèm thông tin thanh toán
Response: ApiResponseDto<BookingDetailDto>
```

```
GET /api/bookings/customer/{customerId}
Description: Lấy danh sách đặt phòng của khách hàng
Response: ApiResponseDto<IEnumerable<BookingDto>>
```

```
GET /api/bookings/pending
Description: Lấy danh sách đặt phòng chờ duyệt
Response: ApiResponseDto<IEnumerable<BookingDto>>
```

### Create & Manage Bookings
```
POST /api/bookings
Body: CreateBookingDto
Description: Tạo đặt phòng mới
Response: ApiResponseDto<BookingDto>

Example:
{
  "roomId": 1,
  "customerId": 1,
  "totalAmount": 1000000,
  "depositAmount": 300000,
  "note": "Yêu cầu view thành phố"
}
```

```
POST /api/bookings/{id}/approve
Query: approvedBy (int - ID người duyệt)
Description: Duyệt đặt phòng
Response: ApiResponseDto<BookingDto>
```

```
POST /api/bookings/{id}/cancel
Query: cancelledBy (int - ID người hủy)
Description: Hủy đặt phòng
Response: ApiResponseDto<BookingDto>
```

```
POST /api/bookings/{id}/payment-proof
Body: { proofUrl: string }
Description: Tải lên chứng minh thanh toán
Response: ApiResponseDto<BookingDto>
```

```
POST /api/bookings/{id}/confirm-payment
Description: Xác nhận thanh toán
Response: ApiResponseDto<BookingDto>
```

---

## 🅿️ Parking Session Endpoints

### Get Parking Sessions
```
GET /api/parking-sessions
Description: Lấy tất cả phiên đậu xe
Response: ApiResponseDto<IEnumerable<ParkingSessionDto>>
```

```
GET /api/parking-sessions/paged?pageIndex=1&pageSize=10
Description: Lấy phiên đậu xe theo phân trang
Response: ApiPagedResponseDto<ParkingSessionDto>
```

```
GET /api/parking-sessions/{id}
Description: Lấy chi tiết phiên đậu xe kèm lịch sử biển số
Response: ApiResponseDto<ParkingSessionDetailDto>
```

```
GET /api/parking-sessions/user/{userId}
Description: Lấy danh sách phiên đậu xe của người dùng
Response: ApiResponseDto<IEnumerable<ParkingSessionDto>>
```

```
GET /api/parking-sessions/active
Description: Lấy danh sách phiên đậu xe đang hoạt động
Response: ApiResponseDto<IEnumerable<ParkingSessionDto>>
```

### Check-in/Check-out
```
POST /api/parking-sessions/check-in
Body: CheckInDto
Description: Check-in phiên đậu xe
Response: ApiResponseDto<ParkingSessionDto>

Example:
{
  "userId": 1,
  "qrUserId": 2,
  "licensePlate": "29A-12345"
}
```

```
POST /api/parking-sessions/{id}/check-out
Body: { licensePlate: string }
Description: Check-out phiên đậu xe
Response: ApiResponseDto<ParkingSessionDto>
```

```
POST /api/parking-sessions/{id}/verify
Query: verifiedBy (int - ID người xác nhận)
Description: Xác nhận phiên đậu xe
Response: ApiResponseDto<ParkingSessionDto>
```

```
DELETE /api/parking-sessions/{id}
Description: Hủy phiên đậu xe
Response: ApiResponseDto<bool>
```

---

## ⭐ Review Endpoints

### Get Reviews
```
GET /api/reviews
Description: Lấy tất cả đánh giá
Response: ApiResponseDto<IEnumerable<ReviewDto>>
```

```
GET /api/reviews/paged?pageIndex=1&pageSize=10
Description: Lấy đánh giá theo phân trang
Response: ApiPagedResponseDto<ReviewDto>
```

```
GET /api/reviews/{id}
Description: Lấy chi tiết đánh giá
Response: ApiResponseDto<ReviewDetailDto>
```

```
GET /api/reviews/room/{roomId}
Description: Lấy danh sách đánh giá của phòng
Response: ApiResponseDto<IEnumerable<ReviewDto>>
```

```
GET /api/reviews/customer/{customerId}
Description: Lấy danh sách đánh giá của khách hàng
Response: ApiResponseDto<IEnumerable<ReviewDto>>
```

```
GET /api/reviews/room/{roomId}/stats
Description: Lấy thống kê đánh giá phòng
Response: ApiResponseDto<RoomReviewStatsDto>

Example Response:
{
  "roomId": 1,
  "roomNumber": "101",
  "totalReviews": 25,
  "averageRating": 4.5,
  "rating5Count": 15,
  "rating4Count": 8,
  "rating3Count": 2,
  "rating2Count": 0,
  "rating1Count": 0
}
```

### Create, Update, Delete Reviews
```
POST /api/reviews
Body: CreateReviewDto
Description: Tạo đánh giá mới
Response: ApiResponseDto<ReviewDto>

Example:
{
  "bookingId": 1,
  "roomId": 1,
  "customerId": 1,
  "rating": 5,
  "comment": "Phòng sạch sẽ, nhân viên thân thiện!"
}
```

```
PUT /api/reviews/{id}
Body: UpdateReviewDto
Description: Cập nhật đánh giá
Response: ApiResponseDto<ReviewDto>
```

```
DELETE /api/reviews/{id}
Description: Xóa đánh giá
Response: ApiResponseDto<bool>
```

---

## 👤 User Endpoints

### Get Users
```
GET /api/users
Description: Lấy tất cả người dùng
Response: ApiResponseDto<IEnumerable<UserDto>>
```

```
GET /api/users/paged?pageIndex=1&pageSize=10
Description: Lấy người dùng theo phân trang
Response: ApiPagedResponseDto<UserDto>
```

```
GET /api/users/{id}
Description: Lấy chi tiết người dùng kèm vai trò và thống kê
Response: ApiResponseDto<UserDetailDto>
```

```
GET /api/users/by-email/{email}
Description: Lấy người dùng theo email
Response: ApiResponseDto<UserDto>
```

```
GET /api/users/search?searchTerm=john
Description: Tìm kiếm người dùng theo tên, email hoặc số điện thoại
Response: ApiResponseDto<IEnumerable<UserDto>>
```

### Update User Profile
```
PUT /api/users/{id}
Body: UpdateUserDto
Description: Cập nhật thông tin người dùng
Response: ApiResponseDto<UserDto>

Example:
{
  "lastName": "Nguyễn",
  "firstName": "Văn A",
  "phone": "0123456789"
}
```

```
PUT /api/users/{id}/avatar
Body: { avatarUrl: string }
Description: Cập nhật avatar
Response: ApiResponseDto<UserDto>
```

### Admin User Management
```
PATCH /api/users/{id}/status
Body: int (status code)
Description: Thay đổi trạng thái người dùng
Response: ApiResponseDto<UserDto>
```

```
DELETE /api/users/{id}
Query: deletedBy (int - ID admin xóa)
Description: Xóa người dùng (soft delete)
Response: ApiResponseDto<bool>
```

```
POST /api/users/{id}/restore
Description: Khôi phục người dùng bị xóa
Response: ApiResponseDto<UserDto>
```

### Role Management
```
POST /api/users/{id}/roles/{roleId}
Description: Gán vai trò cho người dùng
Response: ApiResponseDto<UserDetailDto>
```

```
DELETE /api/users/{id}/roles/{roleId}
Description: Hủy vai trò của người dùng
Response: ApiResponseDto<UserDetailDto>
```

---

## 📊 Status Codes

### Hotel Status
- 0 = Active
- 1 = Inactive
- 2 = Maintenance

### Room Status
- 0 = Available (Trống)
- 1 = Occupied (Đang bị chiếm dụng)
- 2 = Maintenance (Bảo trì)
- 3 = Reserved (Đã được đặt)

### Booking Status
- 0 = Pending (Chờ duyệt)
- 1 = Confirmed (Đã xác nhận)
- 2 = CheckedIn (Đã nhận phòng)
- 3 = CheckedOut (Đã trả phòng)
- 4 = Cancelled (Đã hủy)

### Parking Session Status
- 0 = CheckedIn (Đang đậu)
- 1 = CheckedOut (Đã rời)
- 2 = Verified (Đã xác nhận)
- 3 = Cancelled (Đã hủy)

### User Status
- 0 = Active (Hoạt động)
- 1 = Inactive (Không hoạt động)
- 2 = Suspended (Tạm khóa)

---

## 🔑 Common Query Parameters

- **pageIndex**: Trang hiện tại (mặc định: 1)
- **pageSize**: Số lượng items trên trang (mặc định: 10)
- **searchTerm**: Từ khóa tìm kiếm
- **status**: Trạng thái để lọc

---

## ✅ Response Format Examples

### Success Response
```json
{
  "success": true,
  "message": "Lấy dữ liệu thành công",
  "data": { /* actual data */ }
}
```

### Paged Response
```json
{
  "success": true,
  "message": "Lấy dữ liệu thành công",
  "data": [...],
  "pageIndex": 1,
  "pageSize": 10,
  "totalCount": 100
}
```

### Error Response
```json
{
  "success": false,
  "message": "Lỗi: Không tìm thấy khách sạn",
  "data": null
}
```

---

**Happy Coding! 🚀**

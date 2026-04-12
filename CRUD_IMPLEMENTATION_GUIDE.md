# 🚀 CRUD Implementation Guide - Hotel & Parking Management System

## 📋 Tổng Quan Các Dịch Vụ Đã Tạo

Tôi đã xây dựng hoàn chỉnh các dịch vụ CRUD (Create, Read, Update, Delete) cho tất cả các nghiệp vụ chính của hệ thống:

### 1. **Hotel Service** - Quản Lý Khách Sạn
- **Interfaces**: 
  - `IHotelRepository` - Lấy dữ liệu khách sạn từ database
  - `IHotelService` - Xử lý nghiệp vụ khách sạn
- **Implementations**:
  - `HotelRepository` - `Infrastructure/Repositories/Hotel/HotelRepository.cs`
  - `HotelService` - `Core.Application/Services/Hotel/HotelService.cs`

**Chức năng chính:**
```
✓ Lấy tất cả khách sạn
✓ Lấy khách sạn theo phân trang
✓ Lấy chi tiết khách sạn (kèm danh sách phòng)
✓ Lấy khách sạn theo tên hoặc tỉnh/thành phố
✓ Tạo khách sạn mới
✓ Cập nhật thông tin khách sạn
✓ Xóa khách sạn
✓ Thay đổi trạng thái khách sạn
```

### 2. **Room Service** - Quản Lý Phòng Khách Sạn
- **Interfaces**:
  - `IRoomRepository` - Lấy dữ liệu phòng
  - `IRoomService` - Xử lý nghiệp vụ phòng
- **Implementations**:
  - `RoomRepository` - `Infrastructure/Repositories/Rooms/RoomRepository.cs`
  - `RoomService` - `Core.Application/Services/Rooms/RoomService.cs`

**Chức năng chính:**
```
✓ Lấy tất cả phòng
✓ Lấy phòng theo phân trang
✓ Lấy chi tiết phòng (kèm ảnh, đánh giá, lịch đặt)
✓ Lấy phòng theo khách sạn
✓ Lấy phòng trống trong khoảng thời gian (tính năng đặt phòng)
✓ Tạo phòng mới
✓ Cập nhật thông tin phòng
✓ Xóa phòng
✓ Thay đổi trạng thái phòng
```

### 3. **Booking Service** - Quản Lý Đặt Phòng
- **Interfaces**:
  - `IBookingRepository` - Lấy dữ liệu đặt phòng
  - `IBookingService` - Xử lý nghiệp vụ đặt phòng
- **Implementations**:
  - `BookingRepository` - `Infrastructure/Repositories/Booking/BookingRepository.cs`
  - `BookingService` - `Core.Application/Services/Booking/BookingService.cs`

**Chức năng chính:**
```
✓ Lấy tất cả đặt phòng
✓ Lấy đặt phòng theo phân trang
✓ Lấy chi tiết đặt phòng (kèm thông tin thanh toán)
✓ Lấy danh sách đặt phòng của khách hàng
✓ Lấy danh sách đặt phòng chờ duyệt
✓ Tạo đặt phòng mới
✓ Duyệt/Hủy đặt phòng
✓ Tải lên chứng minh thanh toán
✓ Xác nhận thanh toán
```

### 4. **ParkingSession Service** - Quản Lý Phiên Đậu Xe
- **Interfaces**:
  - `IParkingSessionRepository` - Lấy dữ liệu phiên đậu xe
  - `IParkingSessionService` - Xử lý nghiệp vụ đậu xe
- **Implementations**:
  - `ParkingSessionRepository` - `Infrastructure/Repositories/Parking/ParkingSessionRepository.cs`
  - `ParkingSessionService` - `Core.Application/Services/Parking/ParkingSessionService.cs`

**Chức năng chính:**
```
✓ Lấy tất cả phiên đậu xe
✓ Lấy phiên đậu xe theo phân trang
✓ Lấy chi tiết phiên đậu xe (kèm lịch sử biển số)
✓ Lấy danh sách phiên đậu xe của người dùng
✓ Lấy danh sách phiên đậu xe đang hoạt động
✓ Check-in phiên đậu xe
✓ Check-out phiên đậu xe
✓ Xác nhận phiên đậu xe
✓ Hủy phiên đậu xe
```

### 5. **Review Service** - Quản Lý Đánh Giá
- **Interfaces**:
  - `IReviewRepository` - Lấy dữ liệu đánh giá
  - `IReviewService` - Xử lý nghiệp vụ đánh giá
- **Implementations**:
  - `ReviewRepository` - `Infrastructure/Repositories/Review/ReviewRepository.cs`
  - `ReviewService` - `Core.Application/Services/Review/ReviewService.cs`

**Chức năng chính:**
```
✓ Lấy tất cả đánh giá
✓ Lấy đánh giá theo phân trang
✓ Lấy chi tiết đánh giá
✓ Lấy danh sách đánh giá của phòng
✓ Lấy danh sách đánh giá của khách hàng
✓ Lấy thống kê đánh giá phòng (trung bình sao, số lượng theo mức)
✓ Tạo đánh giá mới
✓ Cập nhật đánh giá
✓ Xóa đánh giá
```

### 6. **User Service** - Quản Lý Người Dùng
- **Interfaces**:
  - `IUserRepository` - Lấy dữ liệu người dùng
  - `IUserService` - Xử lý nghiệp vụ người dùng
- **Implementations**:
  - `UserRepository` - `Infrastructure/Repositories/User/UserRepository.cs`
  - `UserService` - `Core.Application/Services/User/UserService.cs`

**Chức năng chính:**
```
✓ Lấy tất cả người dùng
✓ Lấy người dùng theo phân trang
✓ Lấy chi tiết người dùng (kèm vai trò, thống kê hoạt động)
✓ Lấy người dùng theo email
✓ Tìm kiếm người dùng
✓ Cập nhật thông tin người dùng
✓ Cập nhật avatar người dùng
✓ Thay đổi trạng thái người dùng
✓ Xóa/Khôi phục người dùng
✓ Gán/Hủy vai trò cho người dùng
```

---

## 📁 Cấu Trúc Thư Mục

```
DoAn.HotelParking.Core.Application/
├── Interfaces/
│   ├── Hotel/
│   │   ├── IHotelRepository.cs
│   │   └── IHotelService.cs
│   ├── Rooms/
│   │   ├── IRoomRepository.cs
│   │   └── IRoomService.cs
│   ├── Booking/
│   │   ├── IBookingRepository.cs
│   │   └── IBookingService.cs
│   ├── Parking/
│   │   ├── IParkingSessionRepository.cs
│   │   └── IParkingSessionService.cs
│   ├── Review/
│   │   ├── IReviewRepository.cs
│   │   └── IReviewService.cs
│   └── User/
│       ├── IUserRepository.cs
│       └── IUserService.cs
└── Services/
    ├── Hotel/
    │   └── HotelService.cs
    ├── Rooms/
    │   └── RoomService.cs
    ├── Booking/
    │   └── BookingService.cs
    ├── Parking/
    │   └── ParkingSessionService.cs
    ├── Review/
    │   └── ReviewService.cs
    └── User/
        └── UserService.cs

DoAn.HotelParking.Infrastructure/
└── Repositories/
    ├── Hotel/
    │   └── HotelRepository.cs
    ├── Rooms/
    │   └── RoomRepository.cs
    ├── Booking/
    │   └── BookingRepository.cs
    ├── Parking/
    │   └── ParkingSessionRepository.cs
    ├── Review/
    │   └── ReviewRepository.cs
    └── User/
        └── UserRepository.cs
```

---

## 🔧 Cách Tích Hợp Vào Ứng Dụng

### Step 1: Đăng ký Services trong ApplicationServiceRegistration.cs

```csharp
public static IServiceCollection AddApplicationServices(this IServiceCollection services)
{
    // ... existing registrations ...

    // Hotel Services
    services.AddScoped<IHotelRepository, HotelRepository>();
    services.AddScoped<IHotelService, HotelService>();

    // Room Services
    services.AddScoped<IRoomRepository, RoomRepository>();
    services.AddScoped<IRoomService, RoomService>();

    // Booking Services
    services.AddScoped<IBookingRepository, BookingRepository>();
    services.AddScoped<IBookingService, BookingService>();

    // Parking Services
    services.AddScoped<IParkingSessionRepository, ParkingSessionRepository>();
    services.AddScoped<IParkingSessionService, ParkingSessionService>();

    // Review Services
    services.AddScoped<IReviewRepository, ReviewRepository>();
    services.AddScoped<IReviewService, ReviewService>();

    // User Services
    services.AddScoped<IUserRepository, UserRepository>();
    services.AddScoped<IUserService, UserService>();

    return services;
}
```

### Step 2: Đăng ký Repositories trong InfrastructureServiceRegistration.cs

```csharp
public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
{
    // ... existing registrations ...

    services.AddScoped<IHotelRepository, HotelRepository>();
    services.AddScoped<IRoomRepository, RoomRepository>();
    services.AddScoped<IBookingRepository, BookingRepository>();
    services.AddScoped<IParkingSessionRepository, ParkingSessionRepository>();
    services.AddScoped<IReviewRepository, ReviewRepository>();
    services.AddScoped<IUserRepository, UserRepository>();

    return services;
}
```

---

## 💻 Ví Dụ Sử Dụng Trong Controllers

### Hotel Controller Example

```csharp
[ApiController]
[Route("api/[controller]")]
public class HotelsController : ControllerBase
{
    private readonly IHotelService _hotelService;

    public HotelsController(IHotelService hotelService)
    {
        _hotelService = hotelService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllHotels(CancellationToken cancellationToken)
    {
        var result = await _hotelService.GetAllHotelsAsync(cancellationToken);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("paged")]
    public async Task<IActionResult> GetHotelsPaged(int pageIndex = 1, int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var result = await _hotelService.GetHotelsPagedAsync(pageIndex, pageSize, cancellationToken);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetHotelById(int id, CancellationToken cancellationToken)
    {
        var result = await _hotelService.GetHotelByIdAsync(id, cancellationToken);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateHotel([FromBody] CreateHotelDto dto, CancellationToken cancellationToken)
    {
        var result = await _hotelService.CreateHotelAsync(dto, cancellationToken);
        return result.Success ? Created($"api/hotels/{result.Data?.Id}", result) : BadRequest(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateHotel(int id, [FromBody] UpdateHotelDto dto, CancellationToken cancellationToken)
    {
        var result = await _hotelService.UpdateHotelAsync(id, dto, cancellationToken);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteHotel(int id, CancellationToken cancellationToken)
    {
        var result = await _hotelService.DeleteHotelAsync(id, cancellationToken);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPatch("{id}/status")]
    public async Task<IActionResult> ChangeHotelStatus(int id, [FromBody] int status, CancellationToken cancellationToken)
    {
        var result = await _hotelService.ChangeHotelStatusAsync(id, status, cancellationToken);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}
```

### Room Controller Example

```csharp
[ApiController]
[Route("api/[controller]")]
public class RoomsController : ControllerBase
{
    private readonly IRoomService _roomService;

    public RoomsController(IRoomService roomService)
    {
        _roomService = roomService;
    }

    [HttpGet("hotel/{hotelId}")]
    public async Task<IActionResult> GetRoomsByHotel(int hotelId, CancellationToken cancellationToken)
    {
        var result = await _roomService.GetRoomsByHotelAsync(hotelId, cancellationToken);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("available")]
    public async Task<IActionResult> GetAvailableRooms(int hotelId, DateTime checkIn, DateTime checkOut, CancellationToken cancellationToken)
    {
        var result = await _roomService.GetAvailableRoomsAsync(hotelId, checkIn, checkOut, cancellationToken);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateRoom([FromBody] CreateRoomDto dto, CancellationToken cancellationToken)
    {
        var result = await _roomService.CreateRoomAsync(dto, cancellationToken);
        return result.Success ? Created($"api/rooms/{result.Data?.Id}", result) : BadRequest(result);
    }
}
```

### Booking Controller Example

```csharp
[ApiController]
[Route("api/[controller]")]
public class BookingsController : ControllerBase
{
    private readonly IBookingService _bookingService;

    public BookingsController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateBooking([FromBody] CreateBookingDto dto, CancellationToken cancellationToken)
    {
        var result = await _bookingService.CreateBookingAsync(dto, cancellationToken);
        return result.Success ? Created($"api/bookings/{result.Data?.Id}", result) : BadRequest(result);
    }

    [HttpGet("my-bookings/{customerId}")]
    public async Task<IActionResult> GetMyBookings(int customerId, CancellationToken cancellationToken)
    {
        var result = await _bookingService.GetMyBookingsAsync(customerId, cancellationToken);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("pending")]
    public async Task<IActionResult> GetPendingBookings(CancellationToken cancellationToken)
    {
        var result = await _bookingService.GetPendingBookingsAsync(cancellationToken);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("{id}/approve")]
    public async Task<IActionResult> ApproveBooking(int id, int approvedBy, CancellationToken cancellationToken)
    {
        var result = await _bookingService.ApproveBookingAsync(id, approvedBy, cancellationToken);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("{id}/cancel")]
    public async Task<IActionResult> CancelBooking(int id, int cancelledBy, CancellationToken cancellationToken)
    {
        var result = await _bookingService.CancelBookingAsync(id, cancelledBy, cancellationToken);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}
```

### Review Controller Example

```csharp
[ApiController]
[Route("api/[controller]")]
public class ReviewsController : ControllerBase
{
    private readonly IReviewService _reviewService;

    public ReviewsController(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateReview([FromBody] CreateReviewDto dto, CancellationToken cancellationToken)
    {
        var result = await _reviewService.CreateReviewAsync(dto, cancellationToken);
        return result.Success ? Created($"api/reviews/{result.Data?.Id}", result) : BadRequest(result);
    }

    [HttpGet("room/{roomId}")]
    public async Task<IActionResult> GetRoomReviews(int roomId, CancellationToken cancellationToken)
    {
        var result = await _reviewService.GetReviewsByRoomAsync(roomId, cancellationToken);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("room/{roomId}/stats")]
    public async Task<IActionResult> GetRoomReviewStats(int roomId, CancellationToken cancellationToken)
    {
        var result = await _reviewService.GetRoomReviewStatsAsync(roomId, cancellationToken);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}
```

---

## 🔐 DTOs (Data Transfer Objects)

Tất cả các DTOs đã được định nghĩa trong các Interfaces:

- **Hotel**: `HotelDto`, `HotelDetailDto`, `CreateHotelDto`, `UpdateHotelDto`
- **Room**: `RoomDto`, `RoomDetailDto`, `RoomImageDto`, `CreateRoomDto`, `UpdateRoomDto`
- **Booking**: `BookingDto`, `BookingDetailDto`, `PaymentDto`, `CreateBookingDto`, `UpdateBookingStatusDto`
- **ParkingSession**: `ParkingSessionDto`, `ParkingSessionDetailDto`, `LicensePlateLogDto`, `CheckInDto`, `CheckOutDto`
- **Review**: `ReviewDto`, `ReviewDetailDto`, `RoomReviewStatsDto`, `CreateReviewDto`, `UpdateReviewDto`
- **User**: `UserDto`, `UserDetailDto`, `UserRoleDto`, `UpdateUserDto`

---

## 🎯 Response Format (API Response Standard)

Tất cả API đều trả về định dạng thống nhất:

```json
{
  "success": true/false,
  "message": "Mô tả thành công hoặc lỗi",
  "data": { /* actual data */ }
}
```

Với phân trang:

```json
{
  "success": true/false,
  "message": "Mô tả",
  "data": [ /* list of items */ ],
  "pageIndex": 1,
  "pageSize": 10,
  "totalCount": 100
}
```

---

## ⚠️ Tính Năng Bảo Mật & Validate

- ✅ Soft Delete (xóa mềm - đánh dấu IsDeleted = true)
- ✅ Audit Trail (ghi lại người thay đổi, thời gian thay đổi)
- ✅ Validating Input (kiểm tra dữ liệu đầu vào)
- ✅ Transaction Support (hỗ trợ SaveChangesAsync để rollback)
- ✅ Error Handling (bắt lỗi xn và trả về thông báo rõ ràng)
- ✅ Status Enum (sử dụng enum cho trạng thái thay vì int)

---

## 📝 Ghi Chú Quan Trọng

1. **AutoMapper**: Đảm bảo bạn đã cấu hình AutoMapper mappings cho các DTOs
2. **DbContext**: Các repositories sử dụng ApplicationDbContext được inject qua constructor
3. **CancellationToken**: Luôn truyền CancellationToken để hỗ trợ request cancellation
4. **Error Handling**: Tất cả methods đều có try-catch để xử lý lỗi gracefully
5. **Async/Await**: Tất cả operations là async để tối ưu performance

---

## 🚀 Next Steps

1. Đăng ký tất cả Services trong Startup/Program.cs
2. Tạo Controllers để expose các endpoints API
3. Thêm Authorization/Authentication attributes nếu cần
4. Thêm logging để track operations
5. Viết Unit Tests cho các Services
6. Thêm validation rules cho DTOs

---

**Hy vọng CRUD implementation này giúp bạn xây dựng một hệ thống hotel booking đầy đủ! 🎉**

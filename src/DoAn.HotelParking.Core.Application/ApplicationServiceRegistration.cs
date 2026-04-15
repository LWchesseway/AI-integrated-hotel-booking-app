using Microsoft.Extensions.DependencyInjection;
using DoAn.HotelParking.Core.Application.Interfaces.Auth;
using DoAn.HotelParking.Core.Application.Interfaces.Booking;
using DoAn.HotelParking.Core.Application.Interfaces.Hotel;
using DoAn.HotelParking.Core.Application.Interfaces.Notification;
using DoAn.HotelParking.Core.Application.Interfaces.Parking;
using DoAn.HotelParking.Core.Application.Interfaces.Payment;
using DoAn.HotelParking.Core.Application.Interfaces.Review;
using DoAn.HotelParking.Core.Application.Interfaces.Role;
using DoAn.HotelParking.Core.Application.Interfaces.Room;
using DoAn.HotelParking.Core.Application.Interfaces.RoomType;
using DoAn.HotelParking.Core.Application.Interfaces.User;
using DoAn.HotelParking.Core.Application.Services.Auth;
using DoAn.HotelParking.Core.Application.Services.Booking;
using DoAn.HotelParking.Core.Application.Services.Hotel;
using DoAn.HotelParking.Core.Application.Services.Notification;
using DoAn.HotelParking.Core.Application.Services.Parking;
using DoAn.HotelParking.Core.Application.Services.Payment;
using DoAn.HotelParking.Core.Application.Services.Review;
using DoAn.HotelParking.Core.Application.Services.Role;
using DoAn.HotelParking.Core.Application.Services.Room;
using DoAn.HotelParking.Core.Application.Services.RoomType;
using DoAn.HotelParking.Core.Application.Services.User;

namespace DoAn.HotelParking.Core.Application;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(ApplicationServiceRegistration).Assembly);

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IHotelService, HotelService>();
        services.AddScoped<IRoomService, RoomService>();
        services.AddScoped<IRoomTypeService, RoomTypeService>();
        services.AddScoped<IBookingService, BookingService>();
        services.AddScoped<IPaymentService, PaymentService>();
        services.AddScoped<IParkingSessionService, ParkingSessionService>();
        services.AddScoped<ILicensePlateLogService, LicensePlateLogService>();
        services.AddScoped<IReviewService, ReviewService>();
        services.AddScoped<INotificationService, NotificationService>();

        return services;
    }
}
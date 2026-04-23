using Microsoft.Extensions.DependencyInjection;
using DoAn.HotelParking.Core.Application.Interfaces.Auth;
using DoAn.HotelParking.Core.Application.Interfaces.Booking;
using DoAn.HotelParking.Core.Application.Interfaces.Hotel;
using DoAn.HotelParking.Core.Application.Interfaces.Location;
using DoAn.HotelParking.Core.Application.Interfaces.Notification;
using DoAn.HotelParking.Core.Application.Interfaces.OwnerSetting;
using DoAn.HotelParking.Core.Application.Interfaces.Payment;
using DoAn.HotelParking.Core.Application.Interfaces.Recommendation;
using DoAn.HotelParking.Core.Application.Interfaces.Review;
using DoAn.HotelParking.Core.Application.Interfaces.Role;
using DoAn.HotelParking.Core.Application.Interfaces.Room;
using DoAn.HotelParking.Core.Application.Interfaces.RoomType;
using DoAn.HotelParking.Core.Application.Interfaces.Statistics;
using DoAn.HotelParking.Core.Application.Interfaces.SystemConfig;
using DoAn.HotelParking.Core.Application.Interfaces.TimeSlot;
using DoAn.HotelParking.Core.Application.Interfaces.User;
using DoAn.HotelParking.Core.Application.Services.Auth;
using DoAn.HotelParking.Core.Application.Services.Booking;
using DoAn.HotelParking.Core.Application.Services.Hotel;
using DoAn.HotelParking.Core.Application.Services.Location;
using DoAn.HotelParking.Core.Application.Services.Notification;
using DoAn.HotelParking.Core.Application.Services.OwnerSetting;
using DoAn.HotelParking.Core.Application.Services.Payment;
using DoAn.HotelParking.Core.Application.Services.Recommendation;
using DoAn.HotelParking.Core.Application.Services.Review;
using DoAn.HotelParking.Core.Application.Services.Role;
using DoAn.HotelParking.Core.Application.Services.Room;
using DoAn.HotelParking.Core.Application.Services.RoomType;
using DoAn.HotelParking.Core.Application.Services.Statistics;
using DoAn.HotelParking.Core.Application.Services.SystemConfig;
using DoAn.HotelParking.Core.Application.Services.TimeSlot;
using DoAn.HotelParking.Core.Application.Services.User;

namespace DoAn.HotelParking.Core.Application;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(ApplicationServiceRegistration).Assembly);

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IPermissionService, PermissionService>();
        services.AddScoped<IPermissionManagementService, PermissionManagementService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IHotelService, HotelService>();
        services.AddScoped<IFavoriteHotelService, FavoriteHotelService>();
        services.AddScoped<IHotelImageService, HotelImageService>();
        services.AddScoped<IRoomService, RoomService>();
        services.AddScoped<IRoomTypeService, RoomTypeService>();
        services.AddScoped<ITimeSlotService, TimeSlotService>();
        services.AddScoped<IProvinceService, ProvinceService>();
        services.AddScoped<IWardService, WardService>();
        services.AddScoped<IOwnerSettingService, OwnerSettingService>();
        services.AddScoped<ISystemConfigService, SystemConfigService>();
        services.AddScoped<IRecommendationService, RecommendationService>();
        services.AddScoped<IStatisticsService, StatisticsService>();
        services.AddScoped<IBookingService, BookingService>();
        services.AddScoped<IPaymentService, PaymentService>();
        services.AddScoped<IReviewService, ReviewService>();
        services.AddScoped<INotificationService, NotificationService>();

        return services;
    }
}
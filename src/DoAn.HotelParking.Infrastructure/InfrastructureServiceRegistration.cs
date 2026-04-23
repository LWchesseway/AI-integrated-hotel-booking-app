using DoAn.HotelParking.Core.Application.Interfaces.Base;
using DoAn.HotelParking.Core.Application.Interfaces.Auth;
using DoAn.HotelParking.Core.Application.Interfaces.Booking;
using DoAn.HotelParking.Core.Application.Interfaces.Hotel;
using DoAn.HotelParking.Core.Application.Interfaces.Location;
using DoAn.HotelParking.Core.Application.Interfaces.Notification;
using DoAn.HotelParking.Core.Application.Interfaces.OwnerSetting;
using DoAn.HotelParking.Core.Application.Interfaces.Payment;
using DoAn.HotelParking.Core.Application.Interfaces.Review;
using DoAn.HotelParking.Core.Application.Interfaces.Role;
using DoAn.HotelParking.Core.Application.Interfaces.Room;
using DoAn.HotelParking.Core.Application.Interfaces.RoomType;
using DoAn.HotelParking.Core.Application.Interfaces.Storage;
using DoAn.HotelParking.Core.Application.Interfaces.SystemConfig;
using DoAn.HotelParking.Core.Application.Interfaces.TimeSlot;
using DoAn.HotelParking.Core.Application.Interfaces.User;
using DoAn.HotelParking.Infrastructure.Data;
using DoAn.HotelParking.Infrastructure.Authentication;
using DoAn.HotelParking.Infrastructure.Repositories.Base;
using DoAn.HotelParking.Infrastructure.Repositories.Auth;
using DoAn.HotelParking.Infrastructure.Repositories.Booking;
using DoAn.HotelParking.Infrastructure.Repositories.Hotel;
using DoAn.HotelParking.Infrastructure.Repositories.Location;
using DoAn.HotelParking.Infrastructure.Repositories.Notification;
using DoAn.HotelParking.Infrastructure.Repositories.OwnerSetting;
using DoAn.HotelParking.Infrastructure.Repositories.Payment;
using DoAn.HotelParking.Infrastructure.Repositories.Review;
using DoAn.HotelParking.Infrastructure.Repositories.Role;
using DoAn.HotelParking.Infrastructure.Repositories.Room;
using DoAn.HotelParking.Infrastructure.Repositories.RoomType;
using DoAn.HotelParking.Infrastructure.Repositories.SystemConfig;
using DoAn.HotelParking.Infrastructure.Repositories.TimeSlot;
using DoAn.HotelParking.Infrastructure.Repositories.User;
using DoAn.HotelParking.Infrastructure.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DoAn.HotelParking.Infrastructure;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' was not found.");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString, sql =>
                sql.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
        services.Configure<MinioSettings>(configuration.GetSection("Minio"));

        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IPermissionRepository, PermissionRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IUserRoleRepository, UserRoleRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

        services.AddScoped<IHotelRepository, HotelRepository>();
        services.AddScoped<IFavoriteHotelRepository, FavoriteHotelRepository>();
        services.AddScoped<IHotelImageRepository, HotelImageRepository>();
        services.AddScoped<IRoomRepository, RoomRepository>();
        services.AddScoped<IRoomTypeRepository, RoomTypeRepository>();
        services.AddScoped<ITimeSlotRepository, TimeSlotRepository>();
        services.AddScoped<IProvinceRepository, ProvinceRepository>();
        services.AddScoped<IWardRepository, WardRepository>();
        services.AddScoped<IOwnerSettingRepository, OwnerSettingRepository>();
        services.AddScoped<ISystemConfigRepository, SystemConfigRepository>();
        services.AddScoped<IBookingRepository, BookingRepository>();
        services.AddScoped<IPaymentRepository, PaymentRepository>();
        services.AddScoped<IReviewRepository, ReviewRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();

        services.AddScoped<ITokenService, JwtTokenService>();
        services.AddScoped<IObjectStorageService, MinioObjectStorageService>();

        return services;
    }
}
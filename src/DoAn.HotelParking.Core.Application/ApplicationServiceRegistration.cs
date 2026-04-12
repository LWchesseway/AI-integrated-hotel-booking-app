using DoAn.HotelParking.Core.Application.Interfaces.Hotel;
using DoAn.HotelParking.Core.Application.Interfaces.Rooms;
using DoAn.HotelParking.Core.Application.Interfaces.Booking;
using DoAn.HotelParking.Core.Application.Interfaces.Parking;
using DoAn.HotelParking.Core.Application.Interfaces.Review;
using DoAn.HotelParking.Core.Application.Interfaces.User;
using DoAn.HotelParking.Core.Application.Services.Hotel;
using DoAn.HotelParking.Core.Application.Services.Rooms;
using DoAn.HotelParking.Core.Application.Services.Booking;
using DoAn.HotelParking.Core.Application.Services.Parking;
using DoAn.HotelParking.Core.Application.Services.Review;
using DoAn.HotelParking.Core.Application.Services.User;
using Microsoft.Extensions.DependencyInjection;

namespace DoAn.HotelParking.Core.Application;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Hotel Services
        services.AddScoped<IHotelService, HotelService>();

        // Room Services
        services.AddScoped<IRoomService, RoomService>();

        // Booking Services
        services.AddScoped<IBookingService, BookingService>();

        // Parking Services
        services.AddScoped<IParkingSessionService, ParkingSessionService>();

        // Review Services
        services.AddScoped<IReviewService, ReviewService>();

        // User Services
        services.AddScoped<IUserService, UserService>();

        return services;
    }
}
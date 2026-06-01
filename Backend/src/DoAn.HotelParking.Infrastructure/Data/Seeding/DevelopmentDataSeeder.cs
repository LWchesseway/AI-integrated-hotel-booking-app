using DoAn.HotelParking.Core.Domain.Entities.Auth;
using DoAn.HotelParking.Core.Domain.Entities.Booking;
using DoAn.HotelParking.Core.Domain.Entities.Hotel;
using DoAn.HotelParking.Core.Domain.Entities.OwnerSetting;
using DoAn.HotelParking.Core.Domain.Entities.Review;
using DoAn.HotelParking.Core.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DoAn.HotelParking.Infrastructure.Data.Seeding;

public class DevelopmentDataSeeder
{
    private static readonly string[] FirstNames =
    [
        "Linh", "Minh", "Anh", "Tuan", "Khanh", "Hieu", "Nam", "Bao",
        "Luan", "Trang", "Thao", "Hanh", "Vy", "Nga", "Thu", "Nhi"
    ];

    private static readonly string[] LastNames =
    [
        "Nguyen", "Tran", "Le", "Pham", "Hoang", "Phan", "Vu", "Do",
        "Bui", "Dang", "Mai", "Ngo"
    ];

    private static readonly string[] HotelNamePrefixes =
    [
        "Sun", "Moon", "Lotus", "Ocean", "Sky", "River", "Garden", "Emerald",
        "Golden", "Silver", "Crystal", "Pearl"
    ];

    private static readonly string[] HotelNameSuffixes =
    [
        "Hotel", "Resort", "Stay", "Inn", "Suites", "Retreat"
    ];

    private static readonly string[] Streets =
    [
        "Tran Hung Dao", "Le Loi", "Nguyen Trai", "Hai Ba Trung", "Vo Thi Sau",
        "Ly Thuong Kiet", "Dien Bien Phu", "Pham Ngu Lao"
    ];

    private static readonly string[] ReviewComments =
    [
        "Clean rooms and friendly staff.",
        "Great location and easy check-in.",
        "Nice stay, would book again.",
        "Comfortable beds and quiet nights.",
        "Service was prompt and helpful.",
        "Good value for the price."
    ];

    private static readonly string[] BankNames =
    [
        "Vietcombank", "VietinBank", "BIDV", "ACB", "Techcombank", "MB"
    ];

    private static readonly string[] PaymentMethods =
    [
        "cash", "card", "transfer"
    ];

    private readonly ApplicationDbContext _context;
    private readonly ILogger<DevelopmentDataSeeder> _logger;
    private readonly SeedDataOptions _options;

    public DevelopmentDataSeeder(
        ApplicationDbContext context,
        IOptions<SeedDataOptions> options,
        ILogger<DevelopmentDataSeeder> logger)
    {
        _context = context;
        _logger = logger;
        _options = options.Value;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        if (!_options.Enabled)
        {
            _logger.LogInformation("Seed data is disabled.");
            return;
        }

        if (_options.SkipIfHasData)
        {
            var hasData = await _context.Hotels.AsNoTracking().AnyAsync(cancellationToken)
                          || await _context.Rooms.AsNoTracking().AnyAsync(cancellationToken)
                          || await _context.Bookings.AsNoTracking().AnyAsync(cancellationToken);

            if (hasData)
            {
                _logger.LogInformation("Seed data skipped because target tables already contain data.");
                return;
            }
        }

        var wardIds = await _context.Wards
            .AsNoTracking()
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);

        if (wardIds.Count == 0)
        {
            _logger.LogWarning("Seed data skipped because no wards exist.");
            return;
        }

        var roomTypeIds = await _context.RoomTypes
            .AsNoTracking()
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);

        if (roomTypeIds.Count == 0)
        {
            _logger.LogWarning("Seed data skipped because no room types exist.");
            return;
        }

        var ownerCount = Math.Max(0, _options.OwnerCount);
        var customerCount = Math.Max(0, _options.CustomerCount);
        var hotelsPerOwner = Math.Max(0, _options.HotelsPerOwner);
        var roomsPerHotel = Math.Max(0, _options.RoomsPerHotel);
        var timeSlotsPerRoom = Math.Max(0, _options.TimeSlotsPerRoom);
        var bookingsPerRoom = Math.Max(0, _options.BookingsPerRoom);
        var favoritePerCustomer = Math.Max(0, _options.FavoriteHotelsPerCustomer);
        var reviewPercent = Clamp(_options.ReviewPercent, 0, 100);
        var depositRate = ClampDecimal(_options.DepositRate, 0m, 1m);

        if (ownerCount == 0 || customerCount == 0 || hotelsPerOwner == 0 || roomsPerHotel == 0)
        {
            _logger.LogWarning("Seed data skipped because required counts are zero.");
            return;
        }

        var random = new Random(_options.RandomSeed);
        var existingEmails = await _context.Users
            .AsNoTracking()
            .Where(x => x.Email != null)
            .Select(x => x.Email!)
            .ToHashSetAsync(cancellationToken);

        var owners = GenerateUsers(ownerCount, "owner", existingEmails, random);
        var customers = GenerateUsers(customerCount, "customer", existingEmails, random);

        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            await _context.Users.AddRangeAsync(owners, cancellationToken);
            await _context.Users.AddRangeAsync(customers, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            var ownerSettings = CreateOwnerSettings(owners, random, depositRate);
            await _context.OwnerSettings.AddRangeAsync(ownerSettings, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            var hotels = CreateHotels(owners, wardIds, random, hotelsPerOwner);
            await _context.Hotels.AddRangeAsync(hotels, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            var rooms = CreateRooms(hotels, roomTypeIds, random, roomsPerHotel);
            await _context.Rooms.AddRangeAsync(rooms, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            var timeSlots = CreateTimeSlots(rooms, random, timeSlotsPerRoom);
            await _context.TimeSlots.AddRangeAsync(timeSlots, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            var bookings = CreateBookings(rooms, customers, random, bookingsPerRoom, depositRate);
            await _context.Bookings.AddRangeAsync(bookings, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            var payments = CreatePayments(bookings, random);
            if (payments.Count > 0)
            {
                await _context.Payments.AddRangeAsync(payments, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
            }

            var reviews = CreateReviews(bookings, random, reviewPercent);
            if (reviews.Count > 0)
            {
                await _context.Reviews.AddRangeAsync(reviews, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
            }

            var favorites = CreateFavorites(customers, hotels, random, favoritePerCustomer);
            if (favorites.Count > 0)
            {
                await _context.FavoriteHotels.AddRangeAsync(favorites, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
            }

            await transaction.CommitAsync(cancellationToken);

            _logger.LogInformation(
                "Seed data completed: Users={UserCount}, Hotels={HotelCount}, Rooms={RoomCount}, TimeSlots={TimeSlotCount}, Bookings={BookingCount}",
                owners.Count + customers.Count,
                hotels.Count,
                rooms.Count,
                timeSlots.Count,
                bookings.Count);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            _logger.LogError(ex, "Seed data failed.");
            throw;
        }
    }

    private static List<User> GenerateUsers(
        int count,
        string emailPrefix,
        HashSet<string> existingEmails,
        Random random)
    {
        var users = new List<User>(count);
        var baseDate = DateTime.UtcNow.AddDays(-random.Next(30, 180));

        for (var i = 1; i <= count; i++)
        {
            var firstName = FirstNames[random.Next(FirstNames.Length)];
            var lastName = LastNames[random.Next(LastNames.Length)];
            var email = BuildUniqueEmail(emailPrefix, i, existingEmails);
            var phone = BuildPhoneNumber(random);

            users.Add(new User
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                Phone = phone,
                Password = "P@ssw0rd!",
                AvatarUrl = "https://example.com/avatar.png",
                Status = UserStatus.Active,
                IsDeleted = false,
                CreatedAt = baseDate.AddDays(random.Next(0, 60))
            });
        }

        return users;
    }

    private static List<OwnerSetting> CreateOwnerSettings(
        IReadOnlyCollection<User> owners,
        Random random,
        decimal depositRate)
    {
        var settings = new List<OwnerSetting>(owners.Count);

        foreach (var owner in owners)
        {
            settings.Add(new OwnerSetting
            {
                OwnerId = owner.Id,
                DepositRate = depositRate,
                MinBookingNotice = random.Next(0, 7),
                AllowReview = random.Next(0, 100) > 10,
                BankName = BankNames[random.Next(BankNames.Length)],
                BankAccountNumber = BuildBankAccount(random),
                BankAccountName = $"{owner.FirstName} {owner.LastName}".Trim(),
                BankQrCodeUrl = $"https://example.com/qr/owner-{owner.Id}.png",
                CreatedAt = DateTime.UtcNow.AddDays(-random.Next(5, 40)),
                UpdatedAt = DateTime.UtcNow
            });
        }

        return settings;
    }

    private static List<Hotel> CreateHotels(
        IReadOnlyCollection<User> owners,
        IReadOnlyList<int> wardIds,
        Random random,
        int hotelsPerOwner)
    {
        var hotels = new List<Hotel>(owners.Count * hotelsPerOwner);
        var today = DateTime.UtcNow;
        var ownerIndex = 0;

        foreach (var owner in owners)
        {
            ownerIndex++;
            for (var i = 1; i <= hotelsPerOwner; i++)
            {
                var createdAt = today.AddDays(-random.Next(10, 400));
                hotels.Add(new Hotel
                {
                    OwnerId = owner.Id,
                    WardId = wardIds[random.Next(wardIds.Count)],
                    Name = $"{HotelNamePrefixes[random.Next(HotelNamePrefixes.Length)]} {HotelNameSuffixes[random.Next(HotelNameSuffixes.Length)]} {ownerIndex}-{i}",
                    Street = Streets[random.Next(Streets.Length)],
                    Phone = BuildPhoneNumber(random),
                    Description = "Comfortable stay with convenient amenities.",
                    Status = random.Next(0, 100) > 5 ? HotelStatus.Active : HotelStatus.Inactive,
                    IsDeleted = false,
                    CreatedAt = createdAt,
                    UpdatedAt = createdAt.AddDays(random.Next(1, 20))
                });
            }
        }

        return hotels;
    }

    private static List<Room> CreateRooms(
        IReadOnlyCollection<Hotel> hotels,
        IReadOnlyList<int> roomTypeIds,
        Random random,
        int roomsPerHotel)
    {
        var rooms = new List<Room>(hotels.Count * roomsPerHotel);

        foreach (var hotel in hotels)
        {
            for (var i = 1; i <= roomsPerHotel; i++)
            {
                var capacity = random.Next(1, 6);
                var price = Math.Round(30m + (decimal)random.NextDouble() * 170m, 2, MidpointRounding.AwayFromZero);

                rooms.Add(new Room
                {
                    HotelId = hotel.Id,
                    RoomTypeId = roomTypeIds[random.Next(roomTypeIds.Count)],
                    RoomNumber = i.ToString("D3"),
                    Capacity = capacity,
                    Price = price,
                    Status = PickRoomStatus(random),
                    IsDeleted = false,
                    CreatedAt = hotel.CreatedAt.AddDays(random.Next(0, 30))
                });
            }
        }

        return rooms;
    }

    private static List<TimeSlot> CreateTimeSlots(
        IReadOnlyCollection<Room> rooms,
        Random random,
        int timeSlotsPerRoom)
    {
        var timeSlots = new List<TimeSlot>(rooms.Count * timeSlotsPerRoom);
        var baseDate = DateTime.UtcNow.Date.AddDays(-random.Next(0, 14));

        foreach (var room in rooms)
        {
            for (var i = 0; i < timeSlotsPerRoom; i++)
            {
                var startDate = baseDate.AddDays(i * 3).Date;
                var endDate = startDate.AddDays(2).Date;
                var priceMultiplier = 0.9m + (decimal)random.NextDouble() * 0.3m;
                var slotPrice = Math.Round(room.Price * priceMultiplier, 2, MidpointRounding.AwayFromZero);

                timeSlots.Add(new TimeSlot
                {
                    RoomId = room.Id,
                    StartDate = startDate,
                    EndDate = endDate,
                    Price = slotPrice,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow.AddDays(-random.Next(0, 30)),
                    UpdatedAt = DateTime.UtcNow
                });
            }
        }

        return timeSlots;
    }

    private static List<Booking> CreateBookings(
        IReadOnlyCollection<Room> rooms,
        IReadOnlyList<User> customers,
        Random random,
        int bookingsPerRoom,
        decimal depositRate)
    {
        var bookings = new List<Booking>(rooms.Count * bookingsPerRoom);
        var today = DateTime.UtcNow.Date;

        foreach (var room in rooms)
        {
            for (var i = 0; i < bookingsPerRoom; i++)
            {
                var customer = customers[random.Next(customers.Count)];
                var checkIn = today.AddDays(random.Next(-120, 60));
                var nightCount = random.Next(1, 6);
                var checkOut = checkIn.AddDays(nightCount);
                var totalAmount = Math.Round(room.Price * nightCount, 2, MidpointRounding.AwayFromZero);
                var status = PickBookingStatus(random);
                var paidAmount = status switch
                {
                    BookingStatus.Completed => totalAmount,
                    BookingStatus.Confirmed => Math.Round(totalAmount * depositRate, 2, MidpointRounding.AwayFromZero),
                    _ => 0m
                };

                var createdAt = checkIn.AddDays(-random.Next(1, 20));
                var cancelled = status == BookingStatus.Cancelled;

                bookings.Add(new Booking
                {
                    RoomId = room.Id,
                    CustomerId = customer.Id,
                    CheckInDate = checkIn,
                    CheckOutDate = checkOut,
                    NightCount = nightCount,
                    GuestCount = random.Next(1, Math.Max(2, room.Capacity + 1)),
                    RoomUnitPrice = room.Price,
                    TotalAmount = totalAmount,
                    PaidAmount = paidAmount,
                    Note = random.Next(0, 100) > 80 ? "Request early check-in." : null,
                    Status = status,
                    CancelledBy = cancelled ? customer.Id : null,
                    CancelReason = cancelled ? "Plan changed" : null,
                    CancelledAt = cancelled ? createdAt.AddDays(random.Next(0, 3)) : null,
                    CreatedAt = createdAt,
                    UpdatedAt = createdAt.AddDays(random.Next(0, 10))
                });
            }
        }

        return bookings;
    }

    private static List<Payment> CreatePayments(IReadOnlyCollection<Booking> bookings, Random random)
    {
        var payments = new List<Payment>();

        foreach (var booking in bookings)
        {
            if (booking.PaidAmount <= 0m)
            {
                continue;
            }

            var status = booking.Status == BookingStatus.Completed || booking.Status == BookingStatus.Confirmed
                ? PaymentStatus.Completed
                : PaymentStatus.Pending;

            payments.Add(new Payment
            {
                BookingId = booking.Id,
                Amount = booking.PaidAmount,
                Method = PaymentMethods[random.Next(PaymentMethods.Length)],
                Status = status,
                TransactionCode = status == PaymentStatus.Completed ? BuildTransactionCode() : null,
                Note = status == PaymentStatus.Completed ? "Payment received" : "Awaiting payment",
                PaidAt = status == PaymentStatus.Completed ? booking.CheckInDate.AddDays(-random.Next(0, 2)) : null,
                CreatedAt = booking.CreatedAt.AddDays(random.Next(0, 2))
            });
        }

        return payments;
    }

    private static List<Review> CreateReviews(
        IReadOnlyCollection<Booking> bookings,
        Random random,
        int reviewPercent)
    {
        var reviews = new List<Review>();

        foreach (var booking in bookings)
        {
            if (booking.Status != BookingStatus.Completed)
            {
                continue;
            }

            if (random.Next(0, 100) >= reviewPercent)
            {
                continue;
            }

            reviews.Add(new Review
            {
                BookingId = booking.Id,
                CustomerId = booking.CustomerId,
                RoomId = booking.RoomId,
                Rating = (byte)random.Next(3, 6),
                Comment = ReviewComments[random.Next(ReviewComments.Length)],
                CreatedAt = booking.CheckOutDate.AddDays(random.Next(0, 7))
            });
        }

        return reviews;
    }

    private static List<FavoriteHotel> CreateFavorites(
        IReadOnlyList<User> customers,
        IReadOnlyList<Hotel> hotels,
        Random random,
        int favoritePerCustomer)
    {
        var favorites = new List<FavoriteHotel>();

        if (hotels.Count == 0 || favoritePerCustomer == 0)
        {
            return favorites;
        }

        var favoriteCount = Math.Min(favoritePerCustomer, hotels.Count);

        foreach (var customer in customers)
        {
            var selected = new HashSet<int>();

            while (selected.Count < favoriteCount)
            {
                var hotelId = hotels[random.Next(hotels.Count)].Id;
                if (!selected.Add(hotelId))
                {
                    continue;
                }

                favorites.Add(new FavoriteHotel
                {
                    UserId = customer.Id,
                    HotelId = hotelId,
                    CreatedAt = DateTime.UtcNow.AddDays(-random.Next(0, 40))
                });
            }
        }

        return favorites;
    }

    private static string BuildUniqueEmail(string prefix, int index, HashSet<string> existingEmails)
    {
        var email = $"{prefix}{index}@seed.local";
        while (!existingEmails.Add(email))
        {
            index++;
            email = $"{prefix}{index}@seed.local";
        }

        return email;
    }

    private static string BuildPhoneNumber(Random random)
    {
        return $"0{random.Next(100000000, 999999999)}";
    }

    private static string BuildBankAccount(Random random)
    {
        return $"{random.Next(10000000, 99999999)}{random.Next(10000000, 99999999)}";
    }

    private static string BuildTransactionCode()
    {
        return $"TRX-{Guid.NewGuid():N}";
    }

    private static BookingStatus PickBookingStatus(Random random)
    {
        var roll = random.Next(0, 100);
        if (roll < 50)
        {
            return BookingStatus.Completed;
        }

        if (roll < 75)
        {
            return BookingStatus.Confirmed;
        }

        if (roll < 90)
        {
            return BookingStatus.Pending;
        }

        return BookingStatus.Cancelled;
    }

    private static RoomStatus PickRoomStatus(Random random)
    {
        var roll = random.Next(0, 100);
        if (roll < 80)
        {
            return RoomStatus.Available;
        }

        return roll < 90 ? RoomStatus.Maintenance : RoomStatus.Unavailable;
    }

    private static int Clamp(int value, int min, int max)
    {
        if (value < min)
        {
            return min;
        }

        return value > max ? max : value;
    }

    private static decimal ClampDecimal(decimal value, decimal min, decimal max)
    {
        if (value < min)
        {
            return min;
        }

        return value > max ? max : value;
    }
}

using AutoMapper;
using DoAn.HotelParking.Core.Application.DTOs.Booking;
using DoAn.HotelParking.Core.Application.Interfaces.Base;
using DoAn.HotelParking.Core.Application.Interfaces.Booking;
using DoAn.HotelParking.Core.Application.Interfaces.Hotel;
using DoAn.HotelParking.Core.Application.Interfaces.Notification;
using DoAn.HotelParking.Core.Application.Interfaces.OwnerSetting;
using DoAn.HotelParking.Core.Application.Interfaces.Payment;
using DoAn.HotelParking.Core.Application.Interfaces.Room;
using DoAn.HotelParking.Core.Application.Interfaces.TimeSlot;
using DoAn.HotelParking.Core.Application.Services.Booking;
using DoAn.HotelParking.Core.Domain.Entities.Booking;
using DoAn.HotelParking.Core.Domain.Entities.Hotel;
using DoAn.HotelParking.Core.Domain.Enums;
using FluentAssertions;
using Moq;
using Xunit;
using BookingEntity = DoAn.HotelParking.Core.Domain.Entities.Booking.Booking;

namespace DoAn.HotelParking.UnitTests.Services.Booking;

public class BookingServiceTests
{
    [Fact]
    public async Task CreateCustomerBookingAsync_ShouldThrow_WhenDatesInvalid()
    {
        var service = CreateService(
            new Mock<IBookingRepository>(),
            new Mock<IRoomRepository>(),
            new Mock<IPaymentRepository>(),
            new Mock<ITimeSlotRepository>(),
            new Mock<IOwnerSettingService>(),
            new Mock<IUnitOfWork>(),
            new Mock<IMapper>(),
            new Mock<IHotelRepository>(),
            new Mock<INotificationHelper>());

        var request = new CustomerCreateBookingRequestDto
        {
            RoomId = 1,
            CheckInDate = new DateTime(2026, 1, 10),
            CheckOutDate = new DateTime(2026, 1, 9),
            GuestCount = 1,
            PaidAmount = 0
        };

        var action = () => service.CreateCustomerBookingAsync(1, request);

        await action
            .Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("Check-out date must be after check-in date.");
    }

    [Fact]
    public async Task CreateCustomerBookingAsync_ShouldThrow_WhenRoomUnavailable()
    {
        var bookingRepository = new Mock<IBookingRepository>();
        var roomRepository = new Mock<IRoomRepository>();

        var room = new Room
        {
            Id = 5,
            HotelId = 10,
            Status = RoomStatus.Unavailable,
            IsDeleted = false,
            Price = 150,
            Hotel = new Hotel { OwnerId = 99 }
        };

        roomRepository
            .Setup(repo => repo.GetByIdWithHotelAsync(room.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(room);

        var service = CreateService(
            bookingRepository,
            roomRepository,
            new Mock<IPaymentRepository>(),
            new Mock<ITimeSlotRepository>(),
            new Mock<IOwnerSettingService>(),
            new Mock<IUnitOfWork>(),
            new Mock<IMapper>(),
            new Mock<IHotelRepository>(),
            new Mock<INotificationHelper>());

        var request = new CustomerCreateBookingRequestDto
        {
            RoomId = room.Id,
            CheckInDate = new DateTime(2026, 1, 10),
            CheckOutDate = new DateTime(2026, 1, 12),
            GuestCount = 1,
            PaidAmount = 0
        };

        var action = () => service.CreateCustomerBookingAsync(1, request);

        await action
            .Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("Room is not available for booking.");
    }

    [Fact]
    public async Task CreateCustomerBookingAsync_ShouldThrow_WhenOverlapExists()
    {
        var bookingRepository = new Mock<IBookingRepository>();
        var roomRepository = new Mock<IRoomRepository>();

        var room = new Room
        {
            Id = 5,
            HotelId = 10,
            Status = RoomStatus.Available,
            IsDeleted = false,
            Price = 150,
            Hotel = new Hotel { OwnerId = 99 }
        };

        roomRepository
            .Setup(repo => repo.GetByIdWithHotelAsync(room.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(room);

        bookingRepository
            .Setup(repo => repo.HasOverlappingBookingByHotelAsync(
                room.HotelId,
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>(),
                null,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var service = CreateService(
            bookingRepository,
            roomRepository,
            new Mock<IPaymentRepository>(),
            new Mock<ITimeSlotRepository>(),
            new Mock<IOwnerSettingService>(),
            new Mock<IUnitOfWork>(),
            new Mock<IMapper>(),
            new Mock<IHotelRepository>(),
            new Mock<INotificationHelper>());

        var request = new CustomerCreateBookingRequestDto
        {
            RoomId = room.Id,
            CheckInDate = new DateTime(2026, 1, 10),
            CheckOutDate = new DateTime(2026, 1, 12),
            GuestCount = 1,
            PaidAmount = 0
        };

        var action = () => service.CreateCustomerBookingAsync(1, request);

        await action
            .Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("Hotel already has a booking for the selected dates.");
    }

    [Fact]
    public async Task CreateCustomerBookingAsync_ShouldCreateBookingAndPayment_WhenPaidAmountPositive()
    {
        var bookingRepository = new Mock<IBookingRepository>();
        var roomRepository = new Mock<IRoomRepository>();
        var paymentRepository = new Mock<IPaymentRepository>();
        var timeSlotRepository = new Mock<ITimeSlotRepository>();
        var ownerSettingService = new Mock<IOwnerSettingService>();
        var unitOfWork = new Mock<IUnitOfWork>();
        var mapper = new Mock<IMapper>();
        var hotelRepository = new Mock<IHotelRepository>();
        var notificationHelper = new Mock<INotificationHelper>();

        var room = new Room
        {
            Id = 5,
            HotelId = 10,
            Status = RoomStatus.Available,
            IsDeleted = false,
            Price = 150,
            Hotel = new Hotel { OwnerId = 99 }
        };

        roomRepository
            .Setup(repo => repo.GetByIdWithHotelAsync(room.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(room);

        bookingRepository
            .Setup(repo => repo.HasOverlappingBookingByHotelAsync(
                room.HotelId,
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>(),
                null,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        timeSlotRepository
            .Setup(repo => repo.GetActiveByRoomAndDateRangeAsync(
                room.Id,
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((DoAn.HotelParking.Core.Domain.Entities.Hotel.TimeSlot?)null);

        ownerSettingService
            .Setup(service => service.ValidateBankInfoAsync(room.Hotel.OwnerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        BookingEntity? createdBooking = null;
        bookingRepository
            .Setup(repo => repo.AddAsync(It.IsAny<BookingEntity>(), It.IsAny<CancellationToken>()))
            .Callback<BookingEntity, CancellationToken>((booking, _) =>
            {
                booking.Id = 123;
                createdBooking = booking;
            })
            .Returns(Task.CompletedTask);

        paymentRepository
            .Setup(repo => repo.AddAsync(It.IsAny<Payment>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        hotelRepository
            .Setup(repo => repo.GetByRoomIdAsync(room.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Hotel { OwnerId = 99 });

        notificationHelper
            .Setup(helper => helper.SendBookingCreatedAsync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        notificationHelper
            .Setup(helper => helper.SendPaymentStatusAsync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<PaymentStatus>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        mapper
            .Setup(m => m.Map<BookingDto>(It.IsAny<BookingEntity>()))
            .Returns((BookingEntity booking) => new BookingDto
            {
                Id = booking.Id,
                RoomId = booking.RoomId,
                CustomerId = booking.CustomerId,
                CheckInDate = booking.CheckInDate,
                CheckOutDate = booking.CheckOutDate,
                NightCount = booking.NightCount,
                GuestCount = booking.GuestCount,
                TotalAmount = booking.TotalAmount
            });

        unitOfWork.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var service = CreateService(
            bookingRepository,
            roomRepository,
            paymentRepository,
            timeSlotRepository,
            ownerSettingService,
            unitOfWork,
            mapper,
            hotelRepository,
            notificationHelper);

        var request = new CustomerCreateBookingRequestDto
        {
            RoomId = room.Id,
            CheckInDate = new DateTime(2026, 1, 10),
            CheckOutDate = new DateTime(2026, 1, 12),
            GuestCount = 2,
            PaidAmount = 300,
            PaymentMethod = "Transfer",
            TransactionCode = "TXN-1",
            PaymentNote = "Paid"
        };

        var result = await service.CreateCustomerBookingAsync(101, request);

        result.Id.Should().Be(123);
        createdBooking.Should().NotBeNull();
        createdBooking!.PaidAmount.Should().Be(300);

        ownerSettingService.Verify(service => service.ValidateBankInfoAsync(room.Hotel.OwnerId, It.IsAny<CancellationToken>()), Times.Once);
        paymentRepository.Verify(repo => repo.AddAsync(It.IsAny<Payment>(), It.IsAny<CancellationToken>()), Times.Once);
        notificationHelper.Verify(helper => helper.SendBookingCreatedAsync(99, 101, 123, It.IsAny<CancellationToken>()), Times.Once);
        notificationHelper.Verify(helper => helper.SendPaymentStatusAsync(101, 123, PaymentStatus.Completed, It.IsAny<CancellationToken>()), Times.Once);
        unitOfWork.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CancelMyBookingAsync_ShouldReturnNull_WhenNotOwner()
    {
        var bookingRepository = new Mock<IBookingRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();
        var mapper = new Mock<IMapper>();

        var booking = new BookingEntity
        {
            Id = 10,
            CustomerId = 2,
            Status = BookingStatus.Pending
        };

        bookingRepository
            .Setup(repo => repo.GetByIdAsync(booking.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(booking);

        var service = CreateService(
            bookingRepository,
            new Mock<IRoomRepository>(),
            new Mock<IPaymentRepository>(),
            new Mock<ITimeSlotRepository>(),
            new Mock<IOwnerSettingService>(),
            unitOfWork,
            mapper,
            new Mock<IHotelRepository>(),
            new Mock<INotificationHelper>());

        var result = await service.CancelMyBookingAsync(booking.Id, 1, "Nope");

        result.Should().BeNull();
        unitOfWork.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task AdminForceCompleteBookingAsync_ShouldThrow_WhenBookingCancelled()
    {
        var bookingRepository = new Mock<IBookingRepository>();

        var booking = new BookingEntity
        {
            Id = 10,
            CustomerId = 1,
            Status = BookingStatus.Cancelled
        };

        bookingRepository
            .Setup(repo => repo.GetByIdAsync(booking.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(booking);

        var service = CreateService(
            bookingRepository,
            new Mock<IRoomRepository>(),
            new Mock<IPaymentRepository>(),
            new Mock<ITimeSlotRepository>(),
            new Mock<IOwnerSettingService>(),
            new Mock<IUnitOfWork>(),
            new Mock<IMapper>(),
            new Mock<IHotelRepository>(),
            new Mock<INotificationHelper>());

        var action = () => service.AdminForceCompleteBookingAsync(booking.Id);

        await action
            .Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("Cancelled booking cannot be force-completed.");
    }

    private static BookingService CreateService(
        Mock<IBookingRepository> bookingRepository,
        Mock<IRoomRepository> roomRepository,
        Mock<IPaymentRepository> paymentRepository,
        Mock<ITimeSlotRepository> timeSlotRepository,
        Mock<IOwnerSettingService> ownerSettingService,
        Mock<IUnitOfWork> unitOfWork,
        Mock<IMapper> mapper,
        Mock<IHotelRepository> hotelRepository,
        Mock<INotificationHelper> notificationHelper)
    {
        return new BookingService(
            bookingRepository.Object,
            roomRepository.Object,
            paymentRepository.Object,
            timeSlotRepository.Object,
            ownerSettingService.Object,
            unitOfWork.Object,
            mapper.Object,
            hotelRepository.Object,
            notificationHelper.Object);
    }
}

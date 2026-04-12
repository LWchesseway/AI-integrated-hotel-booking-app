using DoAn.HotelParking.Core.Application.DTOs.Base;
using DoAn.HotelParking.Core.Application.Interfaces.Base;
using DoAn.HotelParking.Core.Application.Interfaces.Parking;
using DoAn.HotelParking.Core.Domain.Entities.Parking;
using DoAn.HotelParking.Core.Domain.Enums;

namespace DoAn.HotelParking.Core.Application.Services.Parking;

public class ParkingSessionService : IParkingSessionService
{
    private readonly IParkingSessionRepository _sessionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ParkingSessionService(IParkingSessionRepository parkingSessionRepository, IUnitOfWork unitOfWork)
    {
        _parkingSessionRepository = parkingSessionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponseDto<IEnumerable<ParkingSessionDto>>> GetAllSessionsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var sessions = await _sessionRepository.GetAllAsync(cancellationToken);
            var dtos = sessions.Select(MapToDto).ToList();
            return new ApiResponseDto<IEnumerable<ParkingSessionDto>> { Success = true, Data = dtos };
        }
        catch (Exception ex)
        {
            return new ApiResponseDto<IEnumerable<ParkingSessionDto>> { Success = false, Message = ex.Message };
        }
    }

    public async Task<ApiPagedResponseDto<ParkingSessionDto>> GetSessionsPagedAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default)
    {
        try
        {
            var (sessions, totalCount) = await _sessionRepository.GetPagedAsync(pageIndex, pageSize, cancellationToken: cancellationToken);
            var dtos = sessions.Select(MapToDto).ToList();
            return new ApiPagedResponseDto<ParkingSessionDto>
            {
                Success = true,
                Data = dtos,
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }
        catch (Exception ex)
        {
            return new ApiPagedResponseDto<ParkingSessionDto> { Success = false, Message = ex.Message };
        }
    }

    public async Task<ApiResponseDto<ParkingSessionDetailDto>> GetSessionByIdAsync(int sessionId, CancellationToken cancellationToken = default)
    {
        try
        {
            var session = await _sessionRepository.GetSessionDetailAsync(sessionId, cancellationToken);
            if (session == null)
                return new ApiResponseDto<ParkingSessionDetailDto> { Success = false, Message = "Phiên đậu xe không tìm thấy" };

            var dto = MapToDetailDto(session);
            return new ApiResponseDto<ParkingSessionDetailDto> { Success = true, Data = dto };
        }
        catch (Exception ex)
        {
            return new ApiResponseDto<ParkingSessionDetailDto> { Success = false, Message = ex.Message };
        }
    }

    public async Task<ApiResponseDto<IEnumerable<ParkingSessionDto>>> GetMySessionsAsync(int userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var sessions = await _sessionRepository.GetSessionsByUserAsync(userId, cancellationToken);
            var dtos = sessions.Select(MapToDto).ToList();
            return new ApiResponseDto<IEnumerable<ParkingSessionDto>> { Success = true, Data = dtos };
        }
        catch (Exception ex)
        {
            return new ApiResponseDto<IEnumerable<ParkingSessionDto>> { Success = false, Message = ex.Message };
        }
    }

    public async Task<ApiResponseDto<IEnumerable<ParkingSessionDto>>> GetActiveSessions(CancellationToken cancellationToken = default)
    {
        try
        {
            var sessions = await _sessionRepository.GetActiveSessions(cancellationToken);
            var dtos = sessions.Select(MapToDto).ToList();
            return new ApiResponseDto<IEnumerable<ParkingSessionDto>> { Success = true, Data = dtos };
        }
        catch (Exception ex)
        {
            return new ApiResponseDto<IEnumerable<ParkingSessionDto>> { Success = false, Message = ex.Message };
        }
    }

    public async Task<ApiResponseDto<ParkingSessionDto>> CheckInAsync(int userId, int qrUserId, string licensePlate, CancellationToken cancellationToken = default)
    {
        try
        {
            // Kiểm tra người dùng tồn tại không
            var user = await _context.Users.FindAsync([userId], cancellationToken: cancellationToken);
            if (user == null)
                return new ApiResponseDto<ParkingSessionDto> { Success = false, Message = "Người dùng không tìm thấy" };

            var qrUser = await _context.Users.FindAsync([qrUserId], cancellationToken: cancellationToken);
            if (qrUser == null)
                return new ApiResponseDto<ParkingSessionDto> { Success = false, Message = "QR user không tìm thấy" };

            // Kiểm tra người dùng có phiên đậu xe đang hoạt động không
            var existingSession = await _sessionRepository.GetActiveSessionByUserAsync(userId, cancellationToken);
            if (existingSession != null)
                return new ApiResponseDto<ParkingSessionDto> { Success = false, Message = "Người dùng này đã có một phiên đậu xe đang hoạt động" };

            var session = new ParkingSession
            {
                UserId = userId,
                QrUserId = qrUserId,
                CheckInTime = DateTime.UtcNow,
                CheckInPlate = licensePlate,
                Status = ParkingSessionStatus.CheckedIn,
                CreatedAt = DateTime.UtcNow
            };

            await _sessionRepository.AddAsync(session, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            session.User = user;
            session.QrUser = qrUser;
            var dto = MapToDto(session);
            return new ApiResponseDto<ParkingSessionDto> { Success = true, Data = dto, Message = "Check-in thành công" };
        }
        catch (Exception ex)
        {
            return new ApiResponseDto<ParkingSessionDto> { Success = false, Message = ex.Message };
        }
    }

    public async Task<ApiResponseDto<ParkingSessionDto>> CheckOutAsync(int sessionId, string licensePlate, CancellationToken cancellationToken = default)
    {
        try
        {
            var session = await _sessionRepository.GetByIdAsync(sessionId, cancellationToken);
            if (session == null)
                return new ApiResponseDto<ParkingSessionDto> { Success = false, Message = "Phiên đậu xe không tìm thấy" };

            if (session.Status != ParkingSessionStatus.CheckedIn)
                return new ApiResponseDto<ParkingSessionDto> { Success = false, Message = "Phiên đậu xe này không ở trạng thái hoạt động" };

            session.CheckOutTime = DateTime.UtcNow;
            session.CheckOutPlate = licensePlate;
            session.Status = ParkingSessionStatus.CheckedOut;

            _sessionRepository.Update(session);
            await _context.SaveChangesAsync(cancellationToken);

            var updatedSession = await _sessionRepository.GetSessionDetailAsync(sessionId, cancellationToken);
            var dto = MapToDto(updatedSession!);
            return new ApiResponseDto<ParkingSessionDto> { Success = true, Data = dto, Message = "Check-out thành công" };
        }
        catch (Exception ex)
        {
            return new ApiResponseDto<ParkingSessionDto> { Success = false, Message = ex.Message };
        }
    }

    public async Task<ApiResponseDto<ParkingSessionDto>> VerifySessionAsync(int sessionId, int verifiedBy, CancellationToken cancellationToken = default)
    {
        try
        {
            var session = await _sessionRepository.GetByIdAsync(sessionId, cancellationToken);
            if (session == null)
                return new ApiResponseDto<ParkingSessionDto> { Success = false, Message = "Phiên đậu xe không tìm thấy" };

            session.VerifiedBy = verifiedBy;
            session.Status = ParkingSessionStatus.Verified;

            _sessionRepository.Update(session);
            await _context.SaveChangesAsync(cancellationToken);

            var updatedSession = await _sessionRepository.GetSessionDetailAsync(sessionId, cancellationToken);
            var dto = MapToDto(updatedSession!);
            return new ApiResponseDto<ParkingSessionDto> { Success = true, Data = dto, Message = "Xác nhận phiên đậu xe thành công" };
        }
        catch (Exception ex)
        {
            return new ApiResponseDto<ParkingSessionDto> { Success = false, Message = ex.Message };
        }
    }

    public async Task<ApiResponseDto<bool>> CancelSessionAsync(int sessionId, CancellationToken cancellationToken = default)
    {
        try
        {
            var session = await _sessionRepository.GetByIdAsync(sessionId, cancellationToken);
            if (session == null)
                return new ApiResponseDto<bool> { Success = false, Message = "Phiên đậu xe không tìm thấy" };

            if (session.Status == ParkingSessionStatus.Cancelled)
                return new ApiResponseDto<bool> { Success = false, Message = "Phiên đậu xe này đã bị hủy rồi" };

            session.Status = ParkingSessionStatus.Cancelled;
            _sessionRepository.Update(session);
            await _context.SaveChangesAsync(cancellationToken);

            return new ApiResponseDto<bool> { Success = true, Data = true, Message = "Hủy phiên đậu xe thành công" };
        }
        catch (Exception ex)
        {
            return new ApiResponseDto<bool> { Success = false, Message = ex.Message };
        }
    }

    private ParkingSessionDto MapToDto(ParkingSession session)
    {
        return new ParkingSessionDto
        {
            Id = session.Id,
            UserId = session.UserId,
            UserName = $"{session.User?.FirstName} {session.User?.LastName}".Trim(),
            QrUserId = session.QrUserId,
            QrUserName = $"{session.QrUser?.FirstName} {session.QrUser?.LastName}".Trim(),
            CheckInTime = session.CheckInTime,
            CheckOutTime = session.CheckOutTime,
            CheckInPlate = session.CheckInPlate,
            CheckOutPlate = session.CheckOutPlate,
            Status = (int)session.Status,
            CreatedAt = session.CreatedAt
        };
    }

    private ParkingSessionDetailDto MapToDetailDto(ParkingSession session)
    {
        var dto = new ParkingSessionDetailDto
        {
            Id = session.Id,
            UserId = session.UserId,
            UserName = $"{session.User?.FirstName} {session.User?.LastName}".Trim(),
            QrUserId = session.QrUserId,
            QrUserName = $"{session.QrUser?.FirstName} {session.QrUser?.LastName}".Trim(),
            CheckInTime = session.CheckInTime,
            CheckOutTime = session.CheckOutTime,
            CheckInPlate = session.CheckInPlate,
            CheckOutPlate = session.CheckOutPlate,
            Status = (int)session.Status,
            VerifiedBy = session.VerifiedBy,
            VerifiedByName = session.VerifiedByUser != null
                ? $"{session.VerifiedByUser.FirstName} {session.VerifiedByUser.LastName}".Trim()
                : null,
            CreatedAt = session.CreatedAt,
            Duration = session.CheckOutTime.HasValue
                ? (session.CheckOutTime.Value - session.CheckInTime).TotalHours
                : 0,
            LicensePlateLogs = session.LicensePlateLogs.Select(log => new LicensePlateLogDto
            {
                Id = log.Id,
                PlateNumber = log.DetectedPlate,
                LogTime = log.CreatedAt
            }).ToList()
        };

        return dto;
    }
}

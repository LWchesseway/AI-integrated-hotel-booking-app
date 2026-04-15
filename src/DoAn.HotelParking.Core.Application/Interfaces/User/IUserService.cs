using DoAn.HotelParking.Core.Application.DTOs.User;
using DoAn.HotelParking.Core.Application.Interfaces.Base;

namespace DoAn.HotelParking.Core.Application.Interfaces.User;

public interface IUserService : ICrudService<UserDto, CreateUserDto, UpdateUserDto>
{
}
using DoAn.HotelParking.Core.Application.DTOs.User;
using DoAn.HotelParking.Core.Application.Interfaces.User;
using DoAn.HotelParking.Presentation.Api.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoAn.HotelParking.Presentation.Api.Controllers.User;

[Route("api/users")]
[Authorize(Roles = "Admin")]
public class UsersController : CrudControllerBase<UserDto, CreateUserDto, UpdateUserDto>
{
    public UsersController(IUserService service) : base(service)
    {
    }
}
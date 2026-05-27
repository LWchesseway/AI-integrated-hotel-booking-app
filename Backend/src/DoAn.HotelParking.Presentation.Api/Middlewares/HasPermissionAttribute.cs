using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using DoAn.HotelParking.Core.Application.DTOs.Base;
using DoAn.HotelParking.Core.Application.Interfaces.User;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DoAn.HotelParking.Presentation.Api.Middlewares;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public sealed class HasPermissionAttribute : TypeFilterAttribute
{
    public HasPermissionAttribute(string permissionKey) : base(typeof(HasPermissionFilter))
    {
        Arguments = [permissionKey];
    }

    private sealed class HasPermissionFilter : IAsyncAuthorizationFilter
    {
        private readonly string _permissionKey;
        private readonly IPermissionService _permissionService;

        public HasPermissionFilter(string permissionKey, IPermissionService permissionService)
        {
            _permissionKey = permissionKey;
            _permissionService = permissionService;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;
            if (user.Identity?.IsAuthenticated != true)
            {
                context.Result = BuildResult(401, "Unauthorized");
                return;
            }

            var rawUserId = user.FindFirstValue(JwtRegisteredClaimNames.Sub);
            if (!int.TryParse(rawUserId, out var userId))
            {
                context.Result = BuildResult(401, "Unauthorized");
                return;
            }

            var hasPermission = await _permissionService.HasPermissionAsync(
                userId,
                _permissionKey,
                context.HttpContext.RequestAborted);

            if (!hasPermission)
            {
                context.Result = BuildResult(403, $"Forbidden. Missing permission '{_permissionKey}'.");
            }
        }

        private static ObjectResult BuildResult(int statusCode, string message)
        {
            return new ObjectResult(ApiResponse<object>.Fail(message, statusCode))
            {
                StatusCode = statusCode
            };
        }
    }
}

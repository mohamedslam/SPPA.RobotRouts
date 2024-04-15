using SPPA.Domain.Entities.Users;
using SPPA.Domain.Exceptions;
using SPPA.Logic.Services;
using SPPA.Web.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SPPA.Web.Filters;

public class CheckOrderPermissionAttribute : TypeFilterAttribute
{
    public CheckOrderPermissionAttribute(UserRoleTypeEnum requiredRole)
        : base(typeof(CheckOrderPermissionFilter))
    {
        Arguments = new object[] { requiredRole };
    }

    private class CheckOrderPermissionFilter : IAsyncActionFilter
    {
        private readonly UserRoleTypeEnum _requiredRole;

        private readonly RoleService _roleService;

        public CheckOrderPermissionFilter
        (
            RoleService roleService,
            UserRoleTypeEnum requiredRole
        )
        {
            _roleService = roleService;
            _requiredRole = requiredRole;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.ActionArguments.ContainsKey("orderId"))
                throw new Exception("Argument orderId not exist");

            var orderId = (Guid)context.ActionArguments["orderId"]!;
            var userId = context.HttpContext.User.GetUserId();
            if (!await _roleService.CheckOrderRoleAsync(userId: userId,
                    orderId: orderId,
                    requiredRole: _requiredRole)
               )
            {
                throw new MfPermissionException
                (
                    $"Access for order is denied. orderId:{orderId} Requested role:{_requiredRole}",
                    "server-error.permission.order-access-denied",
                    new() { { "RoleName", _requiredRole.ToString() } }
                );
            }

            await next();
        }
    }

}

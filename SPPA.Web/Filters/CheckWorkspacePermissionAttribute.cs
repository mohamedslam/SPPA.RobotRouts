using SPPA.Domain.Entities.Users;
using SPPA.Domain.Exceptions;
using SPPA.Logic.Services;
using SPPA.Web.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SPPA.Web.Filters;

public class CheckWorkspacePermissionAttribute : TypeFilterAttribute
{
    public CheckWorkspacePermissionAttribute(UserRoleTypeEnum requiredRole)
        : base(typeof(CheckWorkspacePermissionFilter))
    {
        Arguments = new object[] { requiredRole };
    }

    private class CheckWorkspacePermissionFilter : IAsyncActionFilter
    {
        private readonly UserRoleTypeEnum _requiredRole;

        private readonly RoleService _roleService;

        public CheckWorkspacePermissionFilter
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
            if (!context.ActionArguments.ContainsKey("workspaceId"))
                throw new Exception("Argument workspaceId not exist");

            var workspaceId = (Guid)context.ActionArguments["workspaceId"]!;
            var userId = context.HttpContext.User.GetUserId();
            if (!await _roleService.CheckWorkspaceRoleAsync(userId: userId,
                    workspaceId: workspaceId,
                    requiredRole: _requiredRole)
               )
            {
                throw new MfPermissionException
                (
                    $"Access for workspace is denied. WorkspaceId:{workspaceId} Requested role:{_requiredRole}",
                    "server-error.permission.workspace-access-denied",
                    new() { { "RoleName", _requiredRole.ToString() } }
                );
            }

            await next();
        }
    }

}

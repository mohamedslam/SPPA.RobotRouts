using SPPA.Domain;
using SPPA.Domain.Entities.Orders;
using SPPA.Domain.Entities.Users;
using SPPA.Domain.Entities.WorkSpaces;
using SPPA.Domain.Exceptions;
using Microsoft.Extensions.Options;

namespace SPPA.Logic.Services;

public class LicensePlanLimitationsService
{
    private readonly AppSettings _appSettings;

    public LicensePlanLimitationsService(
        IOptionsSnapshot<AppSettings> appSettings
    )
    {
        _appSettings = appSettings.Value;
    }

    #region Check limits

    private readonly HashSet<UserRoleTypeEnum> _limitedRoles = new HashSet<UserRoleTypeEnum>()
    {
        UserRoleTypeEnum.Admin,
        UserRoleTypeEnum.Editor
    };

    public void CheckUserRoleLimit(Workspace workspace, UserRoleTypeEnum? previousRole, UserRoleTypeEnum newRole)
    {
        if (!(_limitedRoles.Contains(newRole) && (previousRole == null || !_limitedRoles.Contains(previousRole.Value))))
            return;

        if (workspace == null)
            throw new ArgumentNullException(nameof(workspace));

        if (workspace.Roles == null)
            throw new ArgumentNullException(nameof(workspace.Roles));

        if (workspace.Orders == null)
            throw new ArgumentNullException(nameof(workspace.Orders));

        if (workspace.Orders.Any(x => x.Roles == null))
            throw new ArgumentNullException(nameof(Order.Roles));

        var currentRolesUsers = workspace.Roles.Where(x => _limitedRoles.Contains(x.RoleType));
        currentRolesUsers = currentRolesUsers.Union(
            workspace.Orders
                     .SelectMany(x => x.Roles)
                     .Where(x => _limitedRoles.Contains(x.RoleType))
        );
        var currentUsersCount = currentRolesUsers.Select(x => x.UserId).Distinct().Count();
        var workspaceLimit = GetWorkspaceLimit(workspace.Id);
        if (currentUsersCount >= workspaceLimit.EditorsLimit)
        {
            throw new MfLicenseException
            (
                $"Limit of {workspaceLimit.EditorsLimit} users for roles admin and editor.",
                "server-error.license.limit-editor-admin",
                new() { { "UserCount", workspaceLimit.EditorsLimit.ToString() } }
            );
        }
    }

    private PersonalLicenseLimit GetWorkspaceLimit(Guid workspaceId)
    {
        var workspaceLimit = _appSettings.LicenseLimit.PersonalLimit.FirstOrDefault(x => x.WorkspaceId == workspaceId);
        if (workspaceLimit != null)
            return workspaceLimit;

        return new PersonalLicenseLimit()
        {
            WorkspaceId = workspaceId,
            EditorsLimit = _appSettings.LicenseLimit.DefaultEditorsLimit,
            StorageLimit = _appSettings.LicenseLimit.DefaultStorageLimit
        };
    }

    #endregion

    #region Check e-mails

    public void CheckEmailForRegistration(string email)
    {
        if (!_appSettings.LicenseLimit.EnableRegistrationWhitelist)
            return;

        CheckEmail(email);
    }

    public void CheckEmailForInvite(string email)
    {
        if (!_appSettings.LicenseLimit.EnableInviteWhitelist)
            return;

        CheckEmail(email);
    }

    private void CheckEmail(string email)
    {
        var normalizeEmail = email.ToLowerInvariant().Trim();
        if (!_appSettings.LicenseLimit.EmailWhitelist.Any(x => normalizeEmail.Contains(x.ToLowerInvariant().Trim())))
        {
            throw new MfNotAcceptableException("E-mail is deny", "server-error.not-acceptable.email-deny");
        }
    }

    #endregion

}

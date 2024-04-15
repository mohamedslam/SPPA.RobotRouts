using SPPA.Domain.Entities.Users;

namespace SPPA.Domain.Extensions;

public static class RoleExtension
{

    #region Check

    public static bool CheckRequiredRole(this UserRoleTypeEnum? currentRole, UserRoleTypeEnum requiredRole)
    {
        if (currentRole == null)
            return false;

        return currentRole.Value.CheckRequiredRole(requiredRole);
    }

    public static bool CheckRequiredRole(this UserRoleTypeEnum currentRole, UserRoleTypeEnum requiredRole)
    {
        if (currentRole == UserRoleTypeEnum.MainAdmin)
            return true;

        switch (requiredRole)
        {
            case UserRoleTypeEnum.Viewer:
                if (currentRole is UserRoleTypeEnum.Admin or UserRoleTypeEnum.Editor or UserRoleTypeEnum.Viewer)
                {
                    return true;
                }
                break;

            case UserRoleTypeEnum.Editor:
                if (currentRole is UserRoleTypeEnum.Admin or UserRoleTypeEnum.Editor)
                {
                    return true;
                }
                break;

            case UserRoleTypeEnum.Admin:
                if (currentRole is UserRoleTypeEnum.Admin)
                {
                    return true;
                }
                break;
            default:
                throw new Exception("Lost logic for role:" + requiredRole.ToString());
        }
        return false;
    }

    #endregion

    #region Find

    public static UserRoleTypeEnum? GetUserRole(this IEnumerable<UserRole> roles, Guid userId)
    {
        var workspaceRole = roles?.Where(x => x.UserId == userId).SingleOrDefault();
        if (workspaceRole != null)
        {
            return workspaceRole.RoleType;
        }

        return null;
    }

    #endregion

}
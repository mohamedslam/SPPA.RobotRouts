using AutoMapper;
using SPPA.Domain.Entities.Orders;
using SPPA.Domain.Entities.Orders.Files;
using SPPA.Domain.Entities.Users;
using SPPA.Domain.Entities.WorkSpaces;
using SPPA.Logic.Dto.Orders;
using SPPA.Logic.Dto.Orders.File;
using SPPA.Logic.Dto.Users;
using SPPA.Logic.Dto.Users.OrderInvites;
using SPPA.Logic.Dto.Users.OrderRoles;
using SPPA.Logic.Dto.Users.WorkspaceInvites;
using SPPA.Logic.Dto.Users.WorkspaceRoles;
using SPPA.Logic.Dto.WorkSpaces;
using SPPA.Logic.Extensions;

namespace SPPA.Logic.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {

        #region Workspace

        CreateMap<Workspace, WorkspacePreviewDto>()
            .Map(x => x.WorkspaceId, x => x.Id);

        CreateMap<Workspace, WorkspaceDto>()
            .Map(x => x.WorkspaceId, x => x.Id)
            .Map(x => x.UserCount, x => x.GetUserCount())
            .Ignore(x => x.UserRole);

        CreateMap<WorkspaceCreateUpdateDto, Workspace>()
            .Ignore(x => x.Id)
            .Ignore(x => x.Orders)
            .Ignore(x => x.Roles)
            .Ignore(x => x.Invites);

        #endregion

        #region Order

        CreateMap<Order, OrderPreviewDto>()
            .Map(x => x.OrderId, x => x.Id);

        CreateMap<Order, OrderReportDto>()
            .Map(x => x.OrderId, x => x.Id)
            .Map(x => x.CustomFields, x => x.CustomFields.ToDictionary(cf => cf.Key, cf => cf.Value));

        CreateMap<Order, OrderDto>()
            .Map(x => x.OrderId, x => x.Id)
            .Map(x => x.UserCount, x => x.GetUserCount())
            .Map(x => x.IfcFiles, x => x.OrderFiles)
            .Ignore(x => x.UserRole);

        CreateMap<OrderCreateDto, Order>()
            .Ignore(x => x.Id)
            .Ignore(x => x.Archived)
            .Ignore(x => x.Status)
            .Ignore(x => x.WorkspaceId)
            .Ignore(x => x.Workspace)
            .Ignore(x => x.Roles)
            .Ignore(x => x.Invites)
            .Ignore(x => x.OrderFiles);

        CreateMap<OrderUpdateDto, Order>()
            .Ignore(x => x.Id)
            .Ignore(x => x.WorkspaceId)
            .Ignore(x => x.Workspace)
            .Ignore(x => x.Roles)
            .Ignore(x => x.Invites)
            .Ignore(x => x.OrderFiles);

        CreateMap<OrderCustomField, OrderCustomFieldDto>();
        CreateMap<OrderCustomFieldDto, OrderCustomField>();

        CreateMap<OrderFile, OrderFileDto>()
            .Map(x => x.FileId, x => x.Id)
            .Map(x => x.FileName, x => x.OriginalFileName);

        CreateMap<OrderFile, OrderFileImportResultDto>()
            .Map(x => x.FileId, x => x.Id)
            .Map(x => x.FileName, x => x.OriginalFileName)
            .Ignore(x => x.SuccessImport)
            .Ignore(x => x.ImportError);

        #endregion 

        #region Invite

        // Workspace
        CreateMap<UserInvite, WorkspaceInviteDto>()
            .Map(x => x.InviteId, x => x.Id);

        CreateMap<WorkspaceInviteCreateDto, UserInvite>()
            .Ignore(x => x.Id)
            .Ignore(x => x.InviteCode)
            .Ignore(x => x.WorkspaceId)
            .Ignore(x => x.Workspace)
            .Ignore(x => x.OrderId)
            .Ignore(x => x.Order);

        CreateMap<WorkspaceInviteUpdateDto, UserInvite>()
            .Ignore(x => x.Id)
            .Ignore(x => x.UserEmail)
            .Ignore(x => x.InviteCode)
            .Ignore(x => x.WorkspaceId)
            .Ignore(x => x.Workspace)
            .Ignore(x => x.OrderId)
            .Ignore(x => x.Order);

        // Order
        CreateMap<UserInvite, OrderInviteDto>()
            .Map(x => x.InviteId, x => x.Id);

        CreateMap<OrderInviteCreateDto, UserInvite>()
            .Ignore(x => x.Id)
            .Ignore(x => x.InviteCode)
            .Ignore(x => x.WorkspaceId)
            .Ignore(x => x.Workspace)
            .Ignore(x => x.OrderId)
            .Ignore(x => x.Order);

        CreateMap<OrderInviteUpdateDto, UserInvite>()
            .Ignore(x => x.Id)
            .Ignore(x => x.UserEmail)
            .Ignore(x => x.InviteCode)
            .Ignore(x => x.WorkspaceId)
            .Ignore(x => x.Workspace)
            .Ignore(x => x.OrderId)
            .Ignore(x => x.Order);

        #endregion

        #region Role

        // Workspace
        CreateMap<UserRole, WorkspaceRoleDto>()
            .Map(x => x.UserName, x => x.User.UserName)
            .Map(x => x.UserEmail, x => x.User.Email)
            .Map(x => x.RoleId, x => x.Id);

        CreateMap<WorkspaceRoleDto, OrderRoleDto>()
            .Ignore(x => x.RoleId)
            .Ignore(x => x.RoleType)
            .Map(x => x.WorkspaceRoleType, x => x.RoleType);

        CreateMap<WorkspaceRoleUpdateDto, UserRole>()
            .Ignore(x => x.Id)
            .Ignore(x => x.Order)
            .Ignore(x => x.OrderId)
            .Ignore(x => x.Workspace)
            .Ignore(x => x.WorkspaceId)
            .Ignore(x => x.User)
            .Ignore(x => x.UserId);

        // Order
        CreateMap<UserRole, OrderRoleDto>()
            .Ignore(x => x.WorkspaceRoleType)
            .Map(x => x.UserName, x => x.User.UserName)
            .Map(x => x.UserEmail, x => x.User.Email)
            .Map(x => x.RoleId, x => x.Id);

        CreateMap<OrderRoleUpdateDto, UserRole>()
            .Ignore(x => x.Id)
            .Ignore(x => x.Order)
            .Ignore(x => x.OrderId)
            .Ignore(x => x.Workspace)
            .Ignore(x => x.WorkspaceId)
            .Ignore(x => x.User)
            .Ignore(x => x.UserId);

        #endregion

        #region User

        CreateMap<User, UserDto>()
            .Map(x => x.UserId, x => x.Id);

        CreateMap<UserSettings, UserSettingsDto>();

        CreateMap<UserSettingsDto, UserSettings>()
            .Ignore(x => x.Id)
            .Ignore(x => x.User);

        #endregion

    }

}

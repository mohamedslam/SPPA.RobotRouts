using AutoMapper;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using SPPA.Domain.Entities.Users;
using SPPA.Domain.Exceptions;
using SPPA.Domain.Extensions;
using SPPA.Domain.Repository.Users;
using SPPA.Domain.Repository.Workspaces;
using SPPA.Logic.Dto.Users.OrderRoles;
using SPPA.Logic.Dto.Users.WorkspaceRoles;
using SPPA.Domain.Repository.Orders;

namespace SPPA.Logic.Services;

public class RoleService
{
    private readonly ILogger<RoleService> _logger;
    private readonly IMapper _mapper;
    private readonly LicensePlanLimitationsService _licenseLimitService;
    private readonly IUserRepository _userRepository;
    private readonly IWorkspaceRepository _workspaceRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IOrderRepository _orderRepository;

    public RoleService(
        ILogger<RoleService> logger,
        IMapper mapper,
        LicensePlanLimitationsService licenseLimitService,
        IUserRepository userRepository,
        IWorkspaceRepository workspaceRepository,
        IRoleRepository roleRepository,
        IOrderRepository orderRepository
    )
    {
        _logger = logger;
        _mapper = mapper;
        _licenseLimitService = licenseLimitService;
        _userRepository = userRepository;
        _workspaceRepository = workspaceRepository;
        _roleRepository = roleRepository;
        _orderRepository = orderRepository;
    }

    #region Workspace roles

    public async Task<WorkspaceRoleDto> CreateWorkspaceRoleAsync(Guid workspaceId, Guid userId, UserRoleTypeEnum roleType)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        var workspace = await _workspaceRepository.GetWorkspaceWitRolesAsync(workspaceId);
        _licenseLimitService.CheckUserRoleLimit(workspace, null, roleType);

        var model = new UserRole(userId, roleType, workspaceId, null);
        try
        {
            _roleRepository.Add(model);
            await _roleRepository.SaveChangesAsync();
            return _mapper.Map<WorkspaceRoleDto>(model);
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Fail on create Role entity. WorkspaceId:{workspaceId} UserId:{userId} RoleType{roleType}");
            throw;
        }
    }

    public async Task<IEnumerable<WorkspaceRoleDto>> GetAllWorkspaceRolesAsync(Guid workspaceId)
    {
        try
        {
            var roles = await _roleRepository.GetRolesWithUsersForWorkspaceAsync(workspaceId);
            return _mapper.Map<WorkspaceRoleDto[]>(roles);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Fail on read Workspace Role list entity");
            throw;
        }
    }

    //public async Task<WorkspaceRoleDto> GetWorkspaceRoleAsync(Guid workspaceId, Guid roleId)
    //{
    //    var model = await _roleRepository.GetRoleWithUsersForWorkspaceAsync(workspaceId, roleId);
    //    return _mapper.Map<WorkspaceRoleDto>(model);
    //}

    public async Task<WorkspaceRoleDto> UpdateWorkspaceRoleAsync(WorkspaceRoleUpdateDto dto, Guid workspaceId, Guid roleId)
    {
        var model = await _roleRepository.GetRoleWithUsersForWorkspaceAsync(workspaceId, roleId);
        var workspace = await _workspaceRepository.GetWorkspaceWitRolesAsync(workspaceId);
        _licenseLimitService.CheckUserRoleLimit(workspace, model.RoleType, dto.RoleType);

        var previousRole = model.RoleType;
        var newRole = dto.RoleType;

        if (previousRole == UserRoleTypeEnum.Admin && previousRole != newRole)
        {
            var otherAdminExist = (await _roleRepository.FindAsync(x =>
                x.WorkspaceId == workspaceId &&
                x.UserId != model.UserId &&
                x.RoleType == UserRoleTypeEnum.Admin
            )).Any();

            if (!otherAdminExist)
            {
                //_dbContext.ChangeTracker.Clear();
                throw new MfNotAcceptableException("Don't touch last admin. Requires one or more admins in workspace",
                    "server-error.not-acceptable.workspace-admin-required");
            }
        }

        _mapper.Map(dto, model);

        try
        {
            _roleRepository.Update(model);
            await _roleRepository.SaveChangesAsync();
            return _mapper.Map<WorkspaceRoleDto>(model);
        }
        catch (Exception e)
        {
            var json = JsonSerializer.Serialize(dto);
            _logger.LogError(e, "Fail on update Role entity. DTO: " + json);
            throw;
        }
    }

    public async Task DeleteWorkspaceRoleAsync(Guid workspaceId, Guid roleId)
    {
        var model = await _roleRepository.GetRoleWithUsersForWorkspaceAsync(workspaceId, roleId);

        if (model.RoleType == UserRoleTypeEnum.Admin)
        {
            var otherAdminExist = (await _roleRepository.FindAsync(x =>
                x.WorkspaceId == workspaceId &&
                x.UserId != model.UserId &&
                x.RoleType == UserRoleTypeEnum.Admin
            )).Any();

            if (!otherAdminExist)
            {
                //_dbContext.ChangeTracker.Clear();
                throw new MfNotAcceptableException("Don't touch last admin. Requires one or more admins in workspace",
                    "server-error.not-acceptable.workspace-admin-required");
            }
        }

        try
        {
            _roleRepository.Remove(model);
            await _roleRepository.SaveChangesAsync();
        }
        catch (Exception e)
        {
            _logger.LogError(e,
                $"Fail on delete Role entity. RoleId. WorkspaceId:{workspaceId} RoleId:{roleId}");
            throw;
        }
    }

    #endregion

    #region Order roles

    public async Task<OrderRoleDto> CreateOrderRoleAsync(Guid orderId, Guid userId, UserRoleTypeEnum roleType)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        var order = await _orderRepository.GetByIdAsync(orderId);
        var workspace = await _workspaceRepository.GetWorkspaceWitRolesAsync(order.WorkspaceId);
        _licenseLimitService.CheckUserRoleLimit(workspace, null, roleType);

        var model = new UserRole(userId, roleType, null, orderId);
        try
        {
            _roleRepository.Add(model);
            await _roleRepository.SaveChangesAsync();
            return _mapper.Map<OrderRoleDto>(model);
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Fail on create Role entity. OrderId:{orderId} UserId:{userId} RoleType{roleType}");

            throw;
        }
    }

    public async Task<IEnumerable<OrderRoleDto>> GetAllOrdersRolesAsync(Guid orderId)
    {
        try
        {
            var workspaceId = (await _orderRepository.GetByIdAsync(orderId))
                .WorkspaceId;

            var orderRoles = await _roleRepository.GetRolesWithUsersForOrderAsync(orderId);
            var workspaceRoles = await _roleRepository.GetRolesWithUsersForWorkspaceAsync(workspaceId);



            var result = _mapper.Map<List<OrderRoleDto>>(orderRoles);
            var workspaceRolesDto = _mapper.Map<WorkspaceRoleDto[]>(workspaceRoles);
            foreach (var workspaceRoleDto in workspaceRolesDto)
            {
                var orderRole = result.Where(x => x.UserId == workspaceRoleDto.UserId).SingleOrDefault();
                if (orderRole != null)
                {
                    orderRole.WorkspaceRoleType = workspaceRoleDto.RoleType;
                }
                else
                {
                    orderRole = _mapper.Map<OrderRoleDto>(workspaceRoleDto);
                    result.Add(orderRole);
                }
            }

            return result;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Fail on read Order Role list entity");
            throw;
        }
    }

    public async Task<OrderRoleDto> UpdateOrderRoleAsync(Guid orderId, Guid roleId, OrderRoleUpdateDto dto)
    {
        var model = await _roleRepository.GetRoleWithUsersForOrderAsync(orderId, roleId);
        var order = await _orderRepository.GetByIdAsync(orderId);
        var workspaceId = order.WorkspaceId;
        var workspace = await _workspaceRepository.GetWorkspaceWitRolesAsync(workspaceId);
        _licenseLimitService.CheckUserRoleLimit(workspace, model.RoleType, dto.RoleType);

        _mapper.Map(dto, model);

        try
        {
            _roleRepository.Update(model);
            await _roleRepository.SaveChangesAsync();
            return _mapper.Map<OrderRoleDto>(model);
        }
        catch (Exception e)
        {
            var json = JsonSerializer.Serialize(dto);
            _logger.LogError(e, "Fail on update Role entity. DTO: " + json);
            throw;
        }
    }

    public async Task DeleteOrderRoleAsync(Guid orderId, Guid roleId)
    {
        var model = await _roleRepository.GetRoleWithUsersForOrderAsync(orderId, roleId);
        try
        {
            _roleRepository.Remove(model);
            await _roleRepository.SaveChangesAsync();
        }
        catch (Exception e)
        {
            _logger.LogError(e,
                $"Fail on delete Role entity. RoleId. OrderId:{orderId} RoleId:{roleId}");
            throw;
        }
    }

    #endregion

    #region Check role

    public async Task<bool> CheckWorkspaceRoleAsync(Guid userId, Guid workspaceId, UserRoleTypeEnum requiredRole)
    {
        var role = await _roleRepository.GetRoleForWorkspaceAsync(workspaceId, userId);

        if (role == null)
        {
            role = await _roleRepository.GetRoleForWorkspaceInheritedByOrderAsync(workspaceId, userId);
        }

        if (role.CheckRequiredRole(requiredRole))
            return true;

        return false;
    }

    public async Task<bool> CheckOrderRoleAsync(Guid userId, Guid orderId, UserRoleTypeEnum requiredRole)
    {
        var orderRole = await _roleRepository.GetRoleForOrderAsync(orderId, userId);

        if (orderRole.CheckRequiredRole(requiredRole))
            return true;

        return false;
    }

    #endregion

}

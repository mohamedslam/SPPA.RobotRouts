using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SPPA.Domain;
using SPPA.Domain.Entities.Users;
using SPPA.Domain.Repository.Orders;
using SPPA.Domain.Repository.Users;
using SPPA.Domain.Repository.Workspaces;
using SPPA.Logic.Dto.Users.OrderInvites;
using SPPA.Logic.Dto.Users.OrderRoles;
using SPPA.Logic.Dto.Users.WorkspaceInvites;
using SPPA.Logic.Dto.Users.WorkspaceRoles;
using System.Text.Json;

namespace SPPA.Logic.Services;

public class InviteService
{
    private readonly ILogger<InviteService> _logger;
    private readonly IMapper _mapper;
    private readonly AppSettings _appSettings;
    private readonly EmailService _emailService;
    private readonly RoleService _roleService;
    private readonly LicensePlanLimitationsService _licenseLimitService;
    private readonly IWorkspaceRepository _workspaceRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IInviteRepository _inviteRepository;
    private readonly IUserRepository _userRepository;

    public InviteService(
        ILogger<InviteService> logger,
        IMapper mapper,
        IOptions<AppSettings> appSettings,
        EmailService emailService,
        RoleService roleService,
        LicensePlanLimitationsService licenseLimitService,
        IWorkspaceRepository workspaceRepository,
        IOrderRepository orderRepository,
        IInviteRepository inviteRepository,
        IUserRepository userRepository
    )
    {
        _logger = logger;
        _mapper = mapper;
        _appSettings = appSettings.Value;
        _emailService = emailService;
        _roleService = roleService;
        _licenseLimitService = licenseLimitService;
        _workspaceRepository = workspaceRepository;
        _orderRepository = orderRepository;
        _inviteRepository = inviteRepository;
        _userRepository = userRepository;
    }

    #region Workspace invite

    public async Task<WorkspaceInviteCreateResponseDto> CreateWorkspaceInviteAsync(WorkspaceInviteCreateDto dto, Guid workspaceId)
    {
        _licenseLimitService.CheckEmailForInvite(dto.UserEmail);

        WorkspaceInviteDto? inviteDto = null;
        WorkspaceRoleDto? roleDto = await CreateWorkspaceRoleIfUserExistAsync(dto, workspaceId);

        if (roleDto == null)
            inviteDto = await CreateWorkspaceInviteForNewUserAsync(dto, workspaceId);

        return new WorkspaceInviteCreateResponseDto()
        {
            Role = roleDto,
            Invite = inviteDto
        };
    }

    private async Task<WorkspaceInviteDto> CreateWorkspaceInviteForNewUserAsync(WorkspaceInviteCreateDto dto, Guid workspaceId)
    {
        var workspace = await _workspaceRepository.GetByIdAsync(workspaceId);
        var model = _mapper.Map<UserInvite>(dto);
        try
        {
            model.WorkspaceId = workspaceId;
            _inviteRepository.Add(model);
            await _inviteRepository.SaveChangesAsync();
            var inviteDto = _mapper.Map<WorkspaceInviteDto>(model);

            await SendWorkspaceInviteConfirmMessageAsync(model.UserEmail, model.InviteCode, workspace?.Name ?? "");
            return inviteDto;
        }
        catch (Exception e)
        {
            var json = JsonSerializer.Serialize(dto);
            _logger.LogError(e, "Fail on create UserInvite entity. DTO: " + json);
            throw;
        }
    }

    private async Task<WorkspaceRoleDto?> CreateWorkspaceRoleIfUserExistAsync(WorkspaceInviteCreateDto dto, Guid workspaceId)
    {
        var workspace = await _workspaceRepository.GetByIdAsync(workspaceId);
        var user = (await _userRepository.FindAsync(x => x.Email == dto.UserEmail.ToLowerInvariant()))
            .SingleOrDefault();

        if (user != null)
        {
            var roleDto = await _roleService.CreateWorkspaceRoleAsync(workspaceId, user.Id, dto.RoleType);
            await SendWorkspaceInviteInfoMessageAsync(user.Email, workspace?.Name ?? "", workspaceId);
            return roleDto;
        }

        return null;
    }

    public async Task<WorkspaceInviteDto[]> GetAllWorkspaceInvitesAsync(Guid workspaceId)
    {
        try
        {
            var list = await _inviteRepository.FindAsync(x => x.WorkspaceId == workspaceId);

            return _mapper.Map<WorkspaceInviteDto[]>(list);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Fail on read UserInvite list entity");
            throw;
        }
    }

    //public async Task<WorkspaceInviteDto> GetWorkspaceInviteAsync(Guid workspaceId, Guid inviteId)
    //{
    //    var model = await _inviteRepository.GetWorkspaceInviteAsync(workspaceId, inviteId);
    //    return _mapper.Map<WorkspaceInviteDto>(model);
    //}

    public async Task<WorkspaceInviteDto> UpdateWorkspaceInviteAsync(WorkspaceInviteUpdateDto dto, Guid workspaceId, Guid inviteId)
    {
        var model = await _inviteRepository.GetWorkspaceInviteAsync(workspaceId, inviteId);
        _mapper.Map(dto, model);

        try
        {
            _inviteRepository.Update(model);
            await _inviteRepository.SaveChangesAsync();
            return _mapper.Map<WorkspaceInviteDto>(model);
        }
        catch (Exception e)
        {
            var json = JsonSerializer.Serialize(dto);
            _logger.LogError(e, "Fail on update UserInvite entity. DTO: " + json);
            throw;
        }
    }

    public async Task DeleteWorkspaceInviteAsync(Guid workspaceId, Guid inviteId)
    {
        var model = await _inviteRepository.GetWorkspaceInviteAsync(workspaceId, inviteId);

        try
        {
            _inviteRepository.Remove(model);
            await _inviteRepository.SaveChangesAsync();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Fail on delete UserInvite entity." +
                                $" workspaceId:{workspaceId} inviteId:{inviteId}");
            throw;
        }

    }

    public async Task UserExitFromWorkspace(Guid workspaceId, Guid userId)
    {
        try
        {
            var model = await _inviteRepository.GetUserWorkSpaceInvitationAsync(workspaceId, userId);

            var listOrdersinWorkSpace = await _orderRepository.GetAllForUserAsync(workspaceId, userId);
            var user = (await _userRepository.FindAsync(x => x.Id == userId)).FirstOrDefault();

            //Delete All User  Invitation Orders witch under workspaceId WorkSpace
            foreach (var ordr in listOrdersinWorkSpace)
            {
                //Get User Invitation For order
                var model_Order_invitation = await _inviteRepository.GetUserOrderInvitationAsync(ordr.Id, userId);

                //Delete Invitation order for workspacee
                _inviteRepository.Remove(model_Order_invitation);
                await _inviteRepository.SaveChangesAsync();
            }
            //Delete Invitation Workspace for user
            _inviteRepository.Remove(model);
            await _inviteRepository.SaveChangesAsync();

            //Get All Workspace Admins Emails
            var listAdminsInvites = _inviteRepository.GetAll().
                         Where(x => x.WorkspaceId == workspaceId && x.RoleType == UserRoleTypeEnum.Admin)
                         .ToList();

            //Send Workspace's Admins Emails
            foreach (var inviteAdmin in listAdminsInvites)
            {
                await SendAdminsUserExitWorkspaceInfoMessageAsync(inviteAdmin.UserEmail,
                                                        model.Workspace.Name,
                                                        user.UserName);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Fail on Exit User from workspace." +
                                $" workspaceId:{workspaceId} userId:{userId}");
            throw;
        }
    }

    #endregion


    #region Order invite

    public async Task<OrderInviteCreateResponseDto> CreateOrderInviteAsync(OrderInviteCreateDto dto, Guid orderId)
    {
        _licenseLimitService.CheckEmailForInvite(dto.UserEmail);

        OrderInviteDto? inviteDto = null;
        OrderRoleDto? roleDto = await CreateOrderRoleIfUserExistAsync(dto, orderId);

        if (roleDto == null)
            inviteDto = await CreateOrderInviteForNewUserAsync(dto, orderId);

        return new OrderInviteCreateResponseDto()
        {
            Role = roleDto,
            Invite = inviteDto
        };
    }

    private async Task<OrderInviteDto> CreateOrderInviteForNewUserAsync(OrderInviteCreateDto dto, Guid orderId)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);
        var model = _mapper.Map<UserInvite>(dto);
        try
        {
            model.OrderId = orderId;
            _inviteRepository.Add(model);
            await _inviteRepository.SaveChangesAsync();
            var inviteDto = _mapper.Map<OrderInviteDto>(model);

            await SendOrderInviteConfirmMessageAsync(model.UserEmail, model.InviteCode, order?.Name ?? "");
            return inviteDto;
        }
        catch (Exception e)
        {
            var json = JsonSerializer.Serialize(dto);
            _logger.LogError(e, "Fail on create UserInvite entity. DTO: " + json);
            throw;
        }
    }

    private async Task<OrderRoleDto?> CreateOrderRoleIfUserExistAsync(OrderInviteCreateDto dto, Guid orderId)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);
        var user = (await _userRepository.FindAsync(x => x.Email == dto.UserEmail.ToLowerInvariant()))
            .SingleOrDefault();

        if (user != null)
        {
            var roleDto = await _roleService.CreateOrderRoleAsync(orderId, user.Id, dto.RoleType);

            await SendOrderInviteInfoMessageAsync(user.Email, order?.Name ?? "", orderId);
            return roleDto;
        }

        return null;
    }

    public async Task<OrderInviteDto[]> GetAllOrderInvitesAsync(Guid orderId)
    {
        try
        {
            var list = await _inviteRepository.FindAsync(x => x.OrderId == orderId);

            return _mapper.Map<OrderInviteDto[]>(list);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Fail on read UserInvite list entity");
            throw;
        }
    }

    public async Task<OrderInviteDto> UpdateOrderInviteAsync(OrderInviteUpdateDto dto, Guid orderId, Guid inviteId)
    {
        var model = await _inviteRepository.GetOrderInviteAsync(orderId, inviteId);
        _mapper.Map(dto, model);

        try
        {
            _inviteRepository.Update(model);
            await _inviteRepository.SaveChangesAsync();
            return _mapper.Map<OrderInviteDto>(model);
        }
        catch (Exception e)
        {
            var json = JsonSerializer.Serialize(dto);
            _logger.LogError(e, "Fail on update UserInvite entity. DTO: " + json);
            throw;
        }
    }

    public async Task DeleteOrderInviteAsync(Guid orderId, Guid inviteId)
    {
        var model = await _inviteRepository.GetOrderInviteAsync(orderId, inviteId);

        try
        {
            _inviteRepository.Remove(model);
            await _inviteRepository.SaveChangesAsync();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Fail on delete UserInvite entity." +
                                $" OrderId:{orderId} inviteId:{inviteId}");
            throw;
        }

    }

    public async Task UserExitFromOrder(Guid orderId, Guid userId)
    {

        try
        {
            //Get data User and Order ivitation
            var model = await _inviteRepository.GetOrderInviteAsync(orderId, userId);
            var user = (await _userRepository.FindAsync(x => x.Id == userId)).FirstOrDefault();

            //Get All order's Admins Emails
            var listAdminsInvites = _inviteRepository.GetAll().
                          Where(x => x.OrderId == orderId && x.RoleType == UserRoleTypeEnum.Admin)
                          .ToList();

            //Delete Currant User WorkSpace Invitations 
            _inviteRepository.Remove(model);
            await _inviteRepository.SaveChangesAsync();

            //Send Email For Every Admin's Order
            foreach (var inviteAdmin in listAdminsInvites)
            {
                await SendAdminUserExitsOrdersInfoMessageAsync(inviteAdmin.UserEmail,
                                                        model.Order.Name,
                                                        user.UserName);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Fail on Exit User from order." +
                                $" OrderId:{orderId} userId:{userId}");
            throw;
        }
    }

    #endregion


    #region Notification message

    private async Task SendWorkspaceInviteConfirmMessageAsync(string email, string code, string workspaceName)
    {
        var confirmUrl = _appSettings.ServerName + $"/auth/confirm-invite?email={email}&code={code}";
        await _emailService.SendEmailAsync(
            email,
            "Вы приглашены в МастерФаб",
            "Здравствуйте!" +
            "<br>" +
            $"Вы были приглашены к рабочему пространству {workspaceName}." +
            "<br>" +
            $"Перейдите по ссылке, чтобы начать работу: <a href='{confirmUrl}'>присоединиться</a>." +
            "<br>" +
            "<br>" +
            "С уважением, команда МастерФаб." +
            "<br>" +
            "<br>" +
            "Hello!" +
            "<br>" +
            $"You have been invited to the workspace {workspaceName}." +
            "<br>" +
            $"Follow the link to get started: <a href='{confirmUrl}'>join</a>." +
            "<br>" +
            "<br>" +
            "Sincerely, the SPPA team."
        );
    }

    private async Task SendWorkspaceInviteInfoMessageAsync(string email, string workspaceName, Guid workspaceId)
    {
        var workspaceUrl = _appSettings.ServerName + $"/workspaces/{workspaceId}/";
        await _emailService.SendEmailAsync(
            email,
            $"Вы приглашены в рабочее пространство {workspaceName}",
            "Здравствуйте!" +
            "<br>" +
            $"Вы были приглашены к рабочему пространству {workspaceName}." +
            "<br>" +
            $"Перейдите по ссылке, чтобы начать работу: <a href='{workspaceUrl}'>перейти</a>." +
            "<br>" +
            "<br>" +
            "С уважением, команда МастерФаб." +
            "<br>" +
            "<br>" +
            "Hello!" +
            "<br>" +
            $"You have been invited to the workspace {workspaceName}." +
            "<br>" +
            $"Follow the link to get started: <a href='{workspaceUrl}'>continue</a>." +
            "<br>" +
            "<br>" +
            "Sincerely, the SPPA team."
        );
    }

    private async Task SendOrderInviteConfirmMessageAsync(string email, string code, string orderName)
    {
        var confirmUrl = _appSettings.ServerName + $"/auth/confirm-invite?email={email}&code={code}";
        await _emailService.SendEmailAsync(
            email,
            "Вы приглашены в МастерФаб",
            "Здравствуйте!" +
            "<br>" +
            $"Вы были приглашены к заказу {orderName}." +
            "<br>" +
            $"Перейдите по ссылке, чтобы начать работу: <a href='{confirmUrl}'>присоединиться</a>." +
            "<br>" +
            "<br>" +
            "С уважением, команда МастерФаб." +
            "<br>" +
            "<br>" +
            "Hello!" +
            "<br>" +
            $"You have been invited to the order {orderName}." +
            "<br>" +
            $"Follow the link to get started: <a href='{confirmUrl}'>join</a>." +
            "<br>" +
            "<br>" +
            "Sincerely, the SPPA team."
        );
    }

    private async Task SendOrderInviteInfoMessageAsync(string email, string orderName, Guid orderId)
    {
        var orderUrl = _appSettings.ServerName + $"/orders/{orderId}/bom";
        await _emailService.SendEmailAsync(
            email,
            $"Вы приглашены в заказ {orderName}",
            "Здравствуйте!" +
            "<br>" +
            $"Вы были приглашены к заказу {orderName}." +
            "<br>" +
            $"Перейдите по ссылке, чтобы начать работу: <a href='{orderUrl}'>перейти</a>." +
            "<br>" +
            "<br>" +
            "С уважением, команда МастерФаб." +
            "<br>" +
            "<br>" +
            "Hello!" +
            "<br>" +
            $"You have been invited to the order {orderName}." +
            "<br>" +
            $"Follow the link to get started: <a href='{orderUrl}'>continue</a>." +
            "<br>" +
            "<br>" +
            "Sincerely, the SPPA team."
        );
    }
    private async Task SendAdminsUserExitWorkspaceInfoMessageAsync(string email, string workspaceName, string UserName)
    {
        await _emailService.SendEmailAsync(
            email,
            $"Уведомление о выходе пользователя: {UserName} из  Рабочее пространство: {workspaceName}",
            "Здравствуйте!" +
            "<br>" +
            $"Уведомление о выходе пользователя:{UserName} из Рабочее пространство {workspaceName}." +
            "<br>" +
            "<br>" +
            "<br>" +
            "С уважением, команда МастерФаб." +
            "<br>" +
            "<br>" +
            "Hello!" +
            "<br>" +
            $"Notification   User :{UserName} Exit from Workspace {workspaceName}." +
            "<br>" +
            "<br>" +
            "<br>" +
            "Sincerely, the SPPA team."
        );
    }
    private async Task SendAdminUserExitsOrdersInfoMessageAsync(string email, string orderName, string UserName)
    {
        await _emailService.SendEmailAsync(
            email,
            $"Уведомление о выходе пользователя: {UserName} из заказ: {orderName}",
            "Здравствуйте!" +
            "<br>" +
            $"Уведомление о выходе пользователя:{UserName} из заказ {orderName}." +
            "<br>" +
            "<br>" +
            "<br>" +
            "С уважением, команда МастерФаб." +
            "<br>" +
            "<br>" +
            "Hello!" +
            "<br>" +
            $"Notification   User :{UserName} Exit from Order {orderName}." +
            "<br>" +
            "<br>" +
            "<br>" +
            "Sincerely, the SPPA team."
        );
    }

    #endregion

}

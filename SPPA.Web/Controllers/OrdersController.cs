using SPPA.Logic.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using SPPA.Web.Identity;
using SPPA.Logic.Dto.Orders;
using SPPA.Web.Filters;
using AutoMapper;
using SPPA.Domain.Entities.Users;
using SPPA.Logic.Dto.Users.OrderRoles;
using SPPA.Logic.Dto.Users.OrderInvites;

namespace SPPA.Web.Controllers;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("api")]
[ApiController]
[Produces(MediaTypeNames.Application.Json)]
public class OrdersController : ControllerBase
{
    private readonly ILogger<OrdersController> _logger;
    private readonly OrderService _orderService;
    private readonly RoleService _roleService;
    private readonly InviteService _inviteService;
    private readonly IMapper _mapper;


    public OrdersController(
        ILogger<OrdersController> logger,
        OrderService orderService,
        RoleService roleService,
        InviteService inviteService,
    IMapper mapper
    )
    {
        _logger = logger;
        _orderService = orderService;
        _roleService = roleService;
        _inviteService = inviteService;
        _mapper = mapper;
    }

    #region Order

    [HttpGet("workspaces/{workspaceId}/orders")]
    [CheckWorkspacePermission(UserRoleTypeEnum.Viewer)]
    public async Task<IEnumerable<OrderPreviewDto>> GetAllOrders(
        [FromRoute] Guid workspaceId
    )
    {
        var userId = HttpContext.User.GetUserId();
        return await _orderService.GetAllForUserAsync(workspaceId, userId);
    }

    [HttpGet("orders/{orderId}")]
    [CheckOrderPermission(UserRoleTypeEnum.Viewer)]
    public async Task<OrderDto> GetOrder(
        [FromRoute] Guid orderId
        )
    {
        var userId = HttpContext.User.GetUserId();
        return await _orderService.GetAsync(orderId, userId);
    }

    [HttpPost("workspaces/{workspaceId}/orders")]
    [CheckWorkspacePermission(UserRoleTypeEnum.Editor)]
    public async Task<OrderDto> PostOrder(
        [FromRoute] Guid workspaceId,
        [FromBody] OrderCreateDto dto
        )
    {
        var userId = HttpContext.User.GetUserId();
        return await _orderService.CreateAsync(dto, workspaceId, userId);
    }

    [HttpPut("orders/{orderId}")]
    [CheckOrderPermission(UserRoleTypeEnum.Editor)]
    public async Task<OrderDto> PutOrder(
        [FromRoute] Guid orderId,
        [FromBody] OrderUpdateDto dto
    )
    {
        var userId = HttpContext.User.GetUserId();
        return await _orderService.UpdateAsync(dto, orderId, userId);
    }

    // TODO Patch
    //[HttpPatch("{orderId}")]
    //public async Task<OrderDto> PatchWorkspace(
    //    [FromRoute] Guid workspaceId,
    //    [FromRoute] Guid orderId,
    //    [FromBody] OrderCreateUpdateDto dto
    //)
    //{
    //    return await _orderService.UpdateAsync(dto, workspaceId, orderId);
    //}

    [HttpDelete("orders/{orderId}")]
    [CheckOrderPermission(UserRoleTypeEnum.Admin)]
    public async Task<IActionResult> DeleteOrder(
        [FromRoute] Guid orderId
        )
    {
        await _orderService.DeleteAsync(orderId);
        return Ok();
    }

    #endregion

    #region Role

    [HttpGet("orders/{orderId}/roles")]
    [CheckOrderPermission(UserRoleTypeEnum.Admin)]
    public async Task<IEnumerable<OrderRoleDto>> GetAllRoles(
        [FromRoute] Guid orderId
        )
    {
        return await _roleService.GetAllOrdersRolesAsync(orderId);
    }

    //[HttpGet("orders/{orderId}/roles/{roleId}")]
    //[CheckOrderPermission(UserRoleTypeEnum.Admin)]
    //public async Task<OrderRoleDto> GetRole(
    //    [FromRoute] Guid orderId,
    //    [FromRoute] Guid roleId
    //)
    //{
    //    return await _roleService.GetOrderRoleAsync(null, orderId, roleId);
    //}

    //[HttpPost("orders/{orderId}/roles/")]
    //[CheckOrderPermission(UserRoleTypeEnum.Admin)]
    //public async Task<OrderRoleDto> PostRole(
    //    [FromRoute] Guid orderId,
    //    [FromBody] OrderRoleCreateDto dto
    //)
    //{
    //    return await _roleService.CreateOrderRoleAsync(orderId, dto);
    //}

    [HttpPut("orders/{orderId}/roles/{roleId}")]
    [CheckOrderPermission(UserRoleTypeEnum.Admin)]
    public async Task<OrderRoleDto> PutRole(
        [FromRoute] Guid orderId,
        [FromRoute] Guid roleId,
        [FromBody] OrderRoleUpdateDto dto
    )
    {
        return await _roleService.UpdateOrderRoleAsync(orderId, roleId, dto);
    }

    [HttpDelete("orders/{orderId}/roles/{roleId}")]
    [CheckOrderPermission(UserRoleTypeEnum.Admin)]
    public async Task<IActionResult> DeleteRole(
        [FromRoute] Guid orderId,
        [FromRoute] Guid roleId
    )
    {
        await _roleService.DeleteOrderRoleAsync(orderId, roleId);
        return Ok();
    }

    #endregion

    #region Invite

    [HttpGet("orders/{orderId}/invites")]
    [CheckOrderPermission(UserRoleTypeEnum.Admin)]
    public async Task<IEnumerable<OrderInviteDto>> GetAllInvites(
        [FromRoute] Guid orderId
    )
    {
        return await _inviteService.GetAllOrderInvitesAsync(orderId);
    }

    //[HttpGet("orders/{orderId}/invites/{inviteId}")]
    //[CheckOrderPermission(UserRoleTypeEnum.Admin)]
    //public async Task<OrderInviteDto> GetInvite(
    //    [FromRoute] Guid orderId,
    //    [FromRoute] Guid inviteId
    //)
    //{
    //    return await _inviteService.GetOrderInviteAsync(null, orderId, inviteId);
    //}

    [HttpPost("orders/{orderId}/invites/")]
    [CheckOrderPermission(UserRoleTypeEnum.Admin)]
    public async Task<OrderInviteCreateResponseDto> PostInvite(
        [FromRoute] Guid orderId,
        [FromBody] OrderInviteCreateDto dto
    )
    {
        return await _inviteService.CreateOrderInviteAsync(dto, orderId);
    }

    [HttpPut("orders/{orderId}/invites/{inviteId}")]
    [CheckOrderPermission(UserRoleTypeEnum.Admin)]
    public async Task<OrderInviteDto> PutInvite(
        [FromRoute] Guid orderId,
        [FromRoute] Guid inviteId,
        [FromBody] OrderInviteUpdateDto dto
    )
    {
        return await _inviteService.UpdateOrderInviteAsync(dto, orderId, inviteId);
    }

    [HttpDelete("orders/{orderId}/invites/{inviteId}")]
    [CheckOrderPermission(UserRoleTypeEnum.Admin)]
    public async Task<IActionResult> DeleteInvite(
        [FromRoute] Guid orderId,
        [FromRoute] Guid inviteId
    )
    {
        await _inviteService.DeleteOrderInviteAsync(orderId, inviteId);
        return Ok();
    }

    [HttpDelete("orders/{orderId}/UserExit/{userId}")]
    [CheckOrderPermission(UserRoleTypeEnum.Editor)]
    public async Task<IActionResult> UserExitFromOrder(
        [FromRoute] Guid orderId,
        [FromRoute] Guid userId
    )
    {
        await _inviteService.UserExitFromOrder(orderId, userId);
        return Ok();
    }
    #endregion

    #region Reports

    [HttpGet("orders/{orderId}/reports")]
    [CheckOrderPermission(UserRoleTypeEnum.Viewer)]
    public IEnumerable<OrderReportInfoDto> GetReports(
        [FromRoute] Guid orderId
    )
    {
        return _orderService.GetReports(orderId);
    }

    #endregion

}

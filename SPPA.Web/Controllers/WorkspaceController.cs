using SPPA.Logic.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using AutoMapper;
using SPPA.Domain.Entities.Users;
using SPPA.Logic.Dto.Users.WorkspaceInvites;
using SPPA.Web.Identity;
using SPPA.Logic.Dto.WorkSpaces;
using SPPA.Logic.UseCases.Workspaces;
using SPPA.Web.Filters;
using MediatR;
using SPPA.Logic.Dto.Users.WorkspaceRoles;

namespace SPPA.Web.Controllers;

//[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("api/workspaces")]
[ApiController]
//[Produces(MediaTypeNames.Application.Json)]
public class WorkspacesController : ControllerBase
{
    private readonly WorkspaceService _workspaceService;
    private readonly InviteService _inviteService;
    private readonly RoleService _roleService;
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public WorkspacesController(
        WorkspaceService workspaceService,
        InviteService inviteService,
        RoleService roleService,
        IMediator mediator,
        IMapper mapper
    )
    {
        _workspaceService = workspaceService;
        _inviteService = inviteService;
        _roleService = roleService;
        _mediator = mediator;
        _mapper = mapper;
    }

    #region Workspace

    [HttpGet("")]
    public async Task<IEnumerable<WorkspacePreviewDto>> GetAllWorkspaces()
    {
        var userId = HttpContext.User.GetUserId();
        var records = await _mediator.Send(new WorkspaceGetAllForUserRequest(userId));
        var dto = _mapper.Map<IEnumerable<WorkspacePreviewDto>>(records);
        return dto;
    }

    [HttpGet("{workspaceId}")]
    [CheckWorkspacePermission(UserRoleTypeEnum.Viewer)]
    public async Task<WorkspaceDto> GetWorkspace([FromRoute] Guid workspaceId)
    {
        var userId = HttpContext.User.GetUserId();
        return await _workspaceService.GetAsync(workspaceId, userId);
    }

    [HttpPost("")]
    public async Task<WorkspaceDto> PostWorkspace([FromBody] WorkspaceCreateUpdateDto dto)
    {
        var userId = HttpContext.User.GetUserId();
        return await _workspaceService.CreateAsync(dto, userId);
    }

    [HttpPut("{workspaceId}")]
    [CheckWorkspacePermission(UserRoleTypeEnum.Editor)]
    public async Task<WorkspaceDto> PutWorkspace([FromRoute] Guid workspaceId, [FromBody] WorkspaceCreateUpdateDto dto)
    {
        var userId = HttpContext.User.GetUserId();
        return await _workspaceService.UpdateAsync(dto, workspaceId, userId);
    }

    // TODO Patch
    //[HttpPatch("{id}")]
    //public async Task<WorkspacePreviewDto> Patch([FromRoute] Guid id, [FromBody] WorkspacePreviewDto dto)
    //{
    //    dto.Id = id;
    //    return await _workspaceService.UpdateAsync(dto);
    //}

    [HttpDelete("{workspaceId}")]
    [CheckWorkspacePermission(UserRoleTypeEnum.Admin)]
    public async Task<IActionResult> DeleteWorkspace([FromRoute] Guid workspaceId)
    {
        await _workspaceService.DeleteAsync(workspaceId);
        return Ok();
    }

    #endregion

    #region Role

    [HttpGet("{workspaceId}/roles")]
    [CheckWorkspacePermission(UserRoleTypeEnum.Admin)]
    public async Task<IEnumerable<WorkspaceRoleDto>> GetAllRoles([FromRoute] Guid workspaceId)
    {
        return await _roleService.GetAllWorkspaceRolesAsync(workspaceId);
    }

    //[HttpGet("{workspaceId}/roles/{roleId}")]
    //[CheckWorkspacePermission(UserRoleTypeEnum.Admin)]
    //public async Task<WorkspaceRoleDto> GetRole([FromRoute] Guid workspaceId, [FromRoute] Guid roleId)
    //{
    //    return await _roleService.GetWorkspaceRoleAsync(workspaceId, roleId);
    //}

    //[HttpPost("{workspaceId}/roles/")]
    //public async Task<WorkspaceRoleDto> CreateRole(
    //    [FromRoute] Guid workspaceId,
    //    [FromBody] WorkspaceCreateUpdateDto dto
    //)
    //{
    //    var userId = HttpContext.User.GetUserId();
    //    return await _roleService.CreateWorkspaceRoleAsync(dto, workspaceId, userId);
    //}

    [HttpPut("{workspaceId}/roles/{roleId}")]
    [CheckWorkspacePermission(UserRoleTypeEnum.Admin)]
    public async Task<WorkspaceRoleDto> UpdateRole(
        [FromRoute] Guid workspaceId,
        [FromRoute] Guid roleId,
        [FromBody] WorkspaceRoleUpdateDto dto
    )
    {
        return await _roleService.UpdateWorkspaceRoleAsync(dto, workspaceId, roleId);
    }

    [HttpDelete("{workspaceId}/roles/{roleId}")]
    [CheckWorkspacePermission(UserRoleTypeEnum.Admin)]
    public async Task<IActionResult> DeleteRole([FromRoute] Guid workspaceId, [FromRoute] Guid roleId)
    {
        await _roleService.DeleteWorkspaceRoleAsync(workspaceId, roleId);
        return Ok();
    }

    #endregion

    #region Invite

    [HttpGet("{workspaceId}/invites")]
    [CheckWorkspacePermission(UserRoleTypeEnum.Admin)]
    public async Task<IEnumerable<WorkspaceInviteDto>> GetAllInvites([FromRoute] Guid workspaceId)
    {
        return await _inviteService.GetAllWorkspaceInvitesAsync(workspaceId);
    }

    //[HttpGet("{workspaceId}/invites/{inviteId}")]
    //[CheckWorkspacePermission(UserRoleTypeEnum.Admin)]
    //public async Task<WorkspaceInviteDto> GetInvite([FromRoute] Guid workspaceId, [FromRoute] Guid inviteId)
    //{
    //    return await _inviteService.GetWorkspaceInviteAsync(workspaceId, inviteId);
    //}

    [HttpPost("{workspaceId}/invites/")]
    [CheckWorkspacePermission(UserRoleTypeEnum.Admin)]
    public async Task<WorkspaceInviteCreateResponseDto> CreateInvite(
        [FromRoute] Guid workspaceId,
        [FromBody] WorkspaceInviteCreateDto dto
    )
    {
        return await _inviteService.CreateWorkspaceInviteAsync(dto, workspaceId);
    }

    [HttpPut("{workspaceId}/invites/{inviteId}")]
    [CheckWorkspacePermission(UserRoleTypeEnum.Admin)]
    public async Task<WorkspaceInviteDto> UpdateInvite(
        [FromRoute] Guid workspaceId,
        [FromRoute] Guid inviteId,
        [FromBody] WorkspaceInviteUpdateDto dto
    )
    {
        return await _inviteService.UpdateWorkspaceInviteAsync(dto, workspaceId, inviteId);
    }

    [HttpDelete("{workspaceId}/invites/{inviteId}")]
    [CheckWorkspacePermission(UserRoleTypeEnum.Admin)]
    public async Task<IActionResult> DeleteInvite([FromRoute] Guid workspaceId, [FromRoute] Guid inviteId)
    {
        await _inviteService.DeleteWorkspaceInviteAsync(workspaceId, inviteId);
        return Ok();
    }

    [HttpDelete("{workspaceId}/UserExit/{userId}")]
    //[CheckWorkspacePermission(UserRoleTypeEnum.Editor)]
    public async Task<IActionResult> UserExitFromOrder(
        [FromRoute] Guid workspaceId,
        [FromRoute] Guid userId
    )
    {
        await _inviteService.UserExitFromWorkspace(workspaceId, userId);
        return Ok();
    }
    #endregion

}

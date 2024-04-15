using Microsoft.Extensions.Logging;
using System.Text.Json;
using AutoMapper;
using SPPA.Domain.Entities.Users;
using SPPA.Domain.Extensions;
using SPPA.Logic.Dto.WorkSpaces;
using SPPA.Domain.Repository.Users;
using SPPA.Domain.Repository.Workspaces;
using SPPA.Domain.Entities.WorkSpaces;

namespace SPPA.Logic.Services;

public class WorkspaceService
{
    private readonly ILogger<WorkspaceService> _logger;
    private readonly IMapper _mapper;
    private readonly IWorkspaceRepository _workspaceRepository;
    private readonly IRoleRepository _roleRepository;

    public WorkspaceService(
        ILogger<WorkspaceService> logger,
        IMapper mapper,
        IWorkspaceRepository workspaceRepository,
        IRoleRepository roleRepository
    )
    {
        _logger = logger;
        _mapper = mapper;
        _workspaceRepository = workspaceRepository;
        _roleRepository = roleRepository;
    }

    #region Workspace

    public async Task<WorkspaceDto> CreateAsync(WorkspaceCreateUpdateDto dto, Guid userId)
    {
        try
        {
            var model = _mapper.Map<Workspace>(dto);
            _workspaceRepository.Add(model);
            await _workspaceRepository.SaveChangesAsync();

            // TODO все за одни запрос
            _roleRepository.Add(new UserRole(userId, UserRoleTypeEnum.Admin, workspaceId: model.Id));
            await _roleRepository.SaveChangesAsync();

            var modelDto = _mapper.Map<WorkspaceDto>(model);
            modelDto.UserRole = model.Roles.GetUserRole(userId);
            return modelDto;
        }
        catch (Exception e)
        {
            var json = JsonSerializer.Serialize(dto);
            _logger.LogError(e, "Fail on create Workspace entity. DTO: " + json);
            throw;
        }
    }

    public async Task<WorkspaceDto> GetAsync(Guid id, Guid userId)
    {
        var model = await _workspaceRepository.GetWorkspaceWitRolesAsync(id);

        var modelDto = _mapper.Map<WorkspaceDto>(model);
        modelDto.UserRole = model.Roles.GetUserRole(userId);
        if (modelDto.UserRole == null)
        {
            modelDto.UserRole = UserRoleTypeEnum.Viewer;
        }

        return modelDto;
    }

    public async Task<WorkspaceDto> UpdateAsync(WorkspaceCreateUpdateDto dto, Guid workspaceId, Guid userId)
    {
        var model = await _workspaceRepository.GetWorkspaceWitRolesAsync(workspaceId);
        _mapper.Map(dto, model);

        try
        {
            _workspaceRepository.Update(model);
            await _workspaceRepository.SaveChangesAsync();

            var modelDto = _mapper.Map<WorkspaceDto>(model);
            modelDto.UserRole = model.Roles.GetUserRole(userId);
            return modelDto;
        }
        catch (Exception e)
        {
            var json = JsonSerializer.Serialize(dto);
            _logger.LogError(e, "Fail on update Workspace entity. DTO: " + json);
            throw;
        }
    }

    public async Task DeleteAsync(Guid id)
    {
        var model = await _workspaceRepository.GetByIdAsync(id);

        try
        {
            _workspaceRepository.Remove(model);
            await _workspaceRepository.SaveChangesAsync();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Fail on delete Workspace entity. Id: " + id);
            throw;
        }

    }

    #endregion

}

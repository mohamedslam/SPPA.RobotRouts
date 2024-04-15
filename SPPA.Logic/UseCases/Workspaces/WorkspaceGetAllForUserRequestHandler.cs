using SPPA.Domain.Entities.WorkSpaces;
using SPPA.Domain.Repository.Workspaces;
using MediatR;

namespace SPPA.Logic.UseCases.Workspaces;

public class WorkspaceGetAllForUserRequestHandler
    : IRequestHandler<WorkspaceGetAllForUserRequest, IEnumerable<Workspace>>
{

    public readonly IWorkspaceRepository _workspaceRepository;

    public WorkspaceGetAllForUserRequestHandler(
        IWorkspaceRepository workspaceRepository
    )
    {
        _workspaceRepository = workspaceRepository;
    }

    public async Task<IEnumerable<Workspace>> Handle(WorkspaceGetAllForUserRequest request, CancellationToken cancellationToken)
    {
        return await _workspaceRepository.GetAllForUserAsync(request.UserId);
    }
}


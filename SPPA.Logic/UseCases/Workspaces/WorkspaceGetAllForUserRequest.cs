using SPPA.Domain.Entities.WorkSpaces;
using MediatR;

namespace SPPA.Logic.UseCases.Workspaces;

public class WorkspaceGetAllForUserRequest : IRequest<IEnumerable<Workspace>>
{

    public Guid UserId { get; }

    public WorkspaceGetAllForUserRequest(Guid userId)
    {
        UserId = userId;
    }

}

using SPPA.Domain.Entities.WorkSpaces;
using MediatR;

namespace SPPA.Logic.UseCases.Workspaces;

public class WorkspaceCreateRequest : IRequest<IEnumerable<Workspace>>
{

    public Guid UserId { get; }



    public WorkspaceCreateRequest(Guid userId)
    {
        UserId = userId;
    }

}

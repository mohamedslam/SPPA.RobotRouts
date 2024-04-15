using SPPA.Domain.Entities.WorkSpaces;

namespace SPPA.Domain.Repository.Workspaces
{
    public interface IWorkspaceRepository : IBaseRepository<Workspace>
    {
        Task<IEnumerable<Workspace>> GetAllForUserAsync(Guid userId);

        Task<Workspace> GetWorkspaceWitRolesAsync(Guid workspaceId);
    }
}

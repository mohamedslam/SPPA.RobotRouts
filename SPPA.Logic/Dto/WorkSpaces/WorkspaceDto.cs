using System.ComponentModel.DataAnnotations;
using SPPA.Domain.Entities.Users;

namespace SPPA.Logic.Dto.WorkSpaces;

/// <summary>
/// Рабочее пространство отдельной компании, тенант
/// </summary>
public class WorkspaceDto
{
    public Guid WorkspaceId { get; set; }

    [Required]
    public string Name { get; set; }

    /// <summary>
    /// Количество пользователей
    /// </summary>
    public int UserCount { get; set; }

    /// <summary>
    /// Права текущего пользователя
    /// </summary>
    public UserRoleTypeEnum? UserRole { get; set; }

    private WorkspaceDto()
    {
    }

}


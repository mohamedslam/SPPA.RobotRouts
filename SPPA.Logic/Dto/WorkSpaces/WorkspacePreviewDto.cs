using System.ComponentModel.DataAnnotations;

namespace SPPA.Logic.Dto.WorkSpaces;

/// <summary>
/// Рабочее пространство отдельной компании, тенант
/// </summary>
public class WorkspacePreviewDto
{
    public Guid WorkspaceId { get; set; }

    [Required]
    public string Name { get; set; }

    private WorkspacePreviewDto()
    {
    }

}


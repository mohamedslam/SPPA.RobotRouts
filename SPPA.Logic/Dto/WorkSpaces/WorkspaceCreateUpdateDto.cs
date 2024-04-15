using System.ComponentModel.DataAnnotations;

namespace SPPA.Logic.Dto.WorkSpaces;

/// <summary>
/// Рабочее пространство отдельной компании, тенант
/// </summary>
public class WorkspaceCreateUpdateDto
{
    [Required]
    public string Name { get; set; }

    public WorkspaceCreateUpdateDto(string name)
    {
        Name = name;
    }
}


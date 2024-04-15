using System.ComponentModel.DataAnnotations;
using SPPA.Domain.Entities.Orders;
using SPPA.Domain.Entities.Users;
using SPPA.Logic.Dto.Orders.File;

namespace SPPA.Logic.Dto.Orders;

/// <summary>
/// Заказ или проект
/// </summary>
public class OrderDto
{
    public Guid OrderId { get; set; }

    /// <summary>
    /// Название заказа
    /// </summary>
    [Required]
    public string Name { get; set; }

    /// <summary>
    /// Шифр
    /// </summary>
    [Required]

    public string? Code { get; set; }

    /// <summary>
    /// Заказчик проекта
    /// </summary>
    [Required]
    public string? Customer { get; set; }

    /// <summary>
    /// Перемещено в архив
    /// </summary>
    public bool Archived { get; set; }

    /// <summary>
    /// Статус выполнения
    /// </summary>
    public OrderProgressStatusEnum Status { get; set; }

    /// <summary>
    /// Пользовательские свойства
    /// </summary>
    public List<OrderCustomFieldDto> CustomFields { get; set; }

    public Guid WorkspaceId { get; set; }

    /// <summary>
    /// Количество пользователей
    /// </summary>
    public int UserCount { get; set; }

    /// <summary>
    /// Права текущего пользователя
    /// </summary>
    public UserRoleTypeEnum? UserRole { get; set; }

    public IEnumerable<OrderFileDto> IfcFiles { get; set; }

    public bool CanLeave { get; set; } = true;

    private OrderDto()
    {
        CustomFields = new List<OrderCustomFieldDto>();
    }
}
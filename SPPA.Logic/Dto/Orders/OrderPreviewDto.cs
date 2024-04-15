using System.ComponentModel.DataAnnotations;
using SPPA.Domain.Entities.Orders;

namespace SPPA.Logic.Dto.Orders;

/// <summary>
/// Заказ или проект
/// </summary>
public class OrderPreviewDto
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
    public string? Code { get; set; }

    /// <summary>
    /// Заказчик проекта
    /// </summary>
    public string? Customer { get; set; }

    /// <summary>
    /// Перемещено в архив
    /// </summary>
    public bool Archived { get; set; }

    /// <summary>
    /// Статус выполнения
    /// </summary>
    public OrderProgressStatusEnum Status { get; set; }

    public Guid WorkspaceId { get; set; }

    ///// <summary>
    ///// Права текущего пользователя
    ///// </summary>
    //public UserRoleTypeEnum? UserRole { get; set; }

    private OrderPreviewDto()
    {
    }

}


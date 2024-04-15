using System.ComponentModel.DataAnnotations;
using SPPA.Domain.Entities.Orders;
using SPPA.Domain.Entities.Users;

namespace SPPA.Logic.Dto.Orders;

/// <summary>
/// Заказ или проект
/// </summary>
public class OrderReportDto
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
    public Dictionary<string, string> CustomFields { get; set; }
}
using System.ComponentModel.DataAnnotations;
using SPPA.Domain.Entities.Orders;

namespace SPPA.Logic.Dto.Orders;

/// <summary>
/// Заказ или проект
/// </summary>
public class OrderUpdateDto
{
    /// <summary>
    /// Название заказа
    /// </summary>
    [Required]
    // TODO [MinLength(5, ErrorMessage = "server-error.validation.min-length;{1}")]
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

    /// <summary>
    /// Пользовательские свойства
    /// </summary>
    public List<OrderCustomFieldDto> CustomFields { get; set; }

    private OrderUpdateDto()
    {
        CustomFields = new List<OrderCustomFieldDto>();
    }

}


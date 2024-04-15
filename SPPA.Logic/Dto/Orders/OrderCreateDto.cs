using System.ComponentModel.DataAnnotations;

namespace SPPA.Logic.Dto.Orders;

/// <summary>
/// Заказ или проект
/// </summary>
public class OrderCreateDto
{
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
    /// Пользовательские свойства
    /// </summary>
    public List<OrderCustomFieldDto> CustomFields { get; set; }

    private OrderCreateDto()
    {
        CustomFields = new List<OrderCustomFieldDto>();
    }

}


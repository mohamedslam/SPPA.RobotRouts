namespace SPPA.Logic.Dto.Orders;

/// <summary>
/// Информация о отчете для заказа
/// </summary>
public class OrderReportInfoDto
{
    /// <summary>
    /// Отображаемое имя
    /// </summary>
    public Dictionary<string, string> DisplayName { get; set; }

    /// <summary>
    /// Относительный путь для отображения в новом окне
    /// </summary>
    public string Url { get; set; }

    public OrderReportInfoDto(Dictionary<string, string> displayName, string url)
    {
        DisplayName = displayName;
        Url = url;
    }
}


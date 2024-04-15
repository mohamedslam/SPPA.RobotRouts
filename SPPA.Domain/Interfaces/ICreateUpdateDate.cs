namespace SPPA.Domain.Interfaces;

public interface ICreateUpdateDate
{
    /// <summary>
    /// Дата и время создания записи
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Дата и время последнего редактирования записи
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; }
}
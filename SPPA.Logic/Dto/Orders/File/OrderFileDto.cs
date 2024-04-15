using SPPA.Domain.Entities.Orders.Files;

namespace SPPA.Logic.Dto.Orders.File;

public class OrderFileDto
{
    public Guid FileId { get; set; }

    public string FileName { get; set; }

    public OrderFileTypeEnum FileType { get; set; }

    public DateTimeOffset LastModified { get; set; }

    /// <summary>
    /// In bytes
    /// </summary>
    public long FileSize { get; set; }

    public string? Position { get; set; }

}
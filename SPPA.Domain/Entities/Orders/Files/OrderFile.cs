using SPPA.Domain.Interfaces;

namespace SPPA.Domain.Entities.Orders.Files;

public class OrderFile : IEntityGuid
{
    public Guid Id { get; set; }

    public string OriginalFileName { get; set; }

    public string FilePath { get; private set; }

    public OrderFileTypeEnum FileType { get; set; }

    public DateTimeOffset LastModified { get; set; }

    /// <summary>
    /// In bytes
    /// </summary>
    public long FileSize { get; set; }

    public Guid OrderId { get; set; }
    public Order Order { get; set; }

    public string? Position { get; set; }

    public List<OrderFileElementRef> ElementsRef { get; set; }

    public OrderFile(
        Guid orderId,
        string originalFileName,
        OrderFileTypeEnum fileType
    )
    {
        Id = Guid.NewGuid();
        OrderId = orderId;
        LastModified = DateTimeOffset.UtcNow;
        OriginalFileName = originalFileName;
        FileType = fileType;

        ElementsRef = new List<OrderFileElementRef>();
    }

    public void SetFilePath(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new Exception("Error on set FilePath in import file method. File not found.");
        }

        FilePath = filePath;
        FileSize = (new FileInfo(filePath)).Length;
    }

}

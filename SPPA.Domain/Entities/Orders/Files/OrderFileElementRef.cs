using SPPA.Domain.Interfaces;

namespace SPPA.Domain.Entities.Orders.Files;

public class OrderFileElementRef : IEntityGuid
{
    public Guid Id { get; set; }

    public Guid ElementId { get; set; }
    public Guid FileId { get; set; }
    public OrderFile File { get; set; }

    public Guid OrderId { get; set; }
    public Order Order { get; set; }

    public OrderFileElementRef(
        Guid orderId,
        Guid elementId,
        Guid fileId
    )
    {
        Id = Guid.NewGuid();
        OrderId = orderId;
        ElementId = elementId;
        FileId = fileId;
    }

}

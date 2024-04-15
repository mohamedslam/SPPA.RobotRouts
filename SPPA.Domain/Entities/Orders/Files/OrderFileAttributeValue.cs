namespace SPPA.Domain.Entities.Orders.Files;

public class OrderFileAttributeValue
{
    public Guid Id { get; set; }

    public Guid AttributeId { get; set; }
    public OrderFileAttribute Attribute { get; set; }

    public Guid OrderId { get; set; }
    public Order Order { get; set; }

    public string Value { get; set; }

    [Obsolete("Only for mapper or EF")]
    public OrderFileAttributeValue()
    {
        Id = Guid.NewGuid();
    }

    public OrderFileAttributeValue(
        Guid orderId,
        Guid attributeId,
        string value
    )
    {
        Id = Guid.NewGuid();
        OrderId = orderId;
        AttributeId = attributeId;
        Value = value;
    }

}


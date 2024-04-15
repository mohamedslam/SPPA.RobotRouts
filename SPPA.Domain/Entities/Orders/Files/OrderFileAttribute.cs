namespace SPPA.Domain.Entities.Orders.Files;

public class OrderFileAttribute
{
    public Guid Id { get; set; }

    public string Key { get; set; }

    public string Label { get; set; }

    public Guid OrderId { get; set; }
    public Order Order { get; set; }


    [Obsolete("Only for mapper or EF", true)]
    private OrderFileAttribute()
    {
        Id = Guid.NewGuid();
    }

    public OrderFileAttribute(
        Guid id,
        string key,
        string label
    )
    {
        Id = id;
        Key = key;
        Label = label;
    }

}


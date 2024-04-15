using SPPA.Logic.Dto.Orders;

namespace SPPA.Logic.Dto.Reports.Orders;

public class ReportOrderDto
{
    public ReportOrderDto(OrderReportDto order, IEnumerable<ElementDto> elements)
    {
        Order = order;
        Elements = elements;
    }

    public OrderReportDto Order { get; set; }
    public IEnumerable<ElementDto> Elements { get; set; }
}


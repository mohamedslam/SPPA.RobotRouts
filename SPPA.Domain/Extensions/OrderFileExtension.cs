using SPPA.Domain.Entities.Orders.Files;

namespace SPPA.Domain.Extensions;

public static class OrderFileExtension
{

    public static string GetMimeType(this OrderFileTypeEnum fileFormat)
    {
        switch (fileFormat)
        {
            case OrderFileTypeEnum.Ifc:
                return "application/ifc";
            case OrderFileTypeEnum.PartDstv:
                return "application/dstv";
            case OrderFileTypeEnum.PartDrawing:
            case OrderFileTypeEnum.AssemblyDrawing:
                return "application/pdf";
            case OrderFileTypeEnum.CommonFile:
                return "application/other";
            default:
                return "application/other";
        }
    }
}


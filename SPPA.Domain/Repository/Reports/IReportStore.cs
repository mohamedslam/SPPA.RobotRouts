using SPPA.Domain.Models;

namespace SPPA.Domain.Repository.Reports;

public interface IReportStore
{
    IEnumerable<ReportInfo> GetReportList();
    string? GetReportFileById(string id);
}

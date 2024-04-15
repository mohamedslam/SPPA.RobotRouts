using System.Text.Json;
using SPPA.Domain.Models;
using SPPA.Domain.Repository.Reports;
using RussianTransliteration;

namespace SPPA.Repository.Reports;

public class StimulsoftReportStore : IReportStore
{
    private static readonly string _folderPath = Path.Combine("Templates", "Reports");
    private static readonly string _fileList = Path.Combine(_folderPath, "FileList.json");

    private readonly Dictionary<string, ReportInfo> _reportFiles;

    public StimulsoftReportStore()
    {
        var _fileListJson = File.ReadAllText(_fileList);
        var files = JsonSerializer.Deserialize<ReportStoreModel[]>(_fileListJson);

        _reportFiles = new Dictionary<string, ReportInfo>();
        foreach (var file in files)
        {
            var report = GetFileInfo(file);
            _reportFiles.Add(report.Id, report);
        }
    }

    public IEnumerable<ReportInfo> GetReportList()
    {
        return _reportFiles.Values.OrderBy(x => x.Id);
    }

    public string? GetReportFileById(string id)
    {
        if (!_reportFiles.ContainsKey(id))
            return null;

        return _reportFiles[id].FilePath;
    }

    private ReportInfo GetFileInfo(ReportStoreModel fileModel)
    {
        var engLabel = fileModel.Label.ContainsKey("en")
            ? fileModel.Label["en"]
            : fileModel.Label.First().Value;
        var normalizeEngLabel = engLabel.ToLowerInvariant().Replace(' ', '-');

        var fileInfo = new ReportInfo
        {
            FilePath = Path.Combine(_folderPath, fileModel.FileName),
            DisplayName = fileModel.Label,
            Id = RussianTransliterator.GetTransliteration(normalizeEngLabel)
        };

        return fileInfo;
    }

}

public class ReportStoreModel
{
    public string FileName { get; set; }

    public Dictionary<string, string> Label { get; set; }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPPA.Logic.Utils;

public class TmpFile : IDisposable
{
    public string FilePath { get; private set; }

    public string OriginalFileName { get; private set; }

    private TmpFile()
    {
    }

    public static async Task<TmpFile> Create(string originalFileName, Stream fileReadStream)
    {
        var result = new TmpFile();
        result.OriginalFileName = originalFileName;
        var fileExtension = Path.GetExtension(originalFileName);
        result.FilePath = Path.GetTempFileName() + fileExtension;
        using (var fileStream = new FileStream(result.FilePath, FileMode.Create))
        {
            await fileReadStream.CopyToAsync(fileStream);
        }

        return result;
    }

    public void Dispose()
    {
        try
        {
            File.Delete(FilePath);
        }
        catch (Exception e)
        {
            // TODO ILogger.Error("Failed on delete TMP file. File name:"+_tempFilePath);
            Console.WriteLine("Failed on delete TMP file. File name:" + FilePath);
            Console.WriteLine(e);
            throw;
        }
    }
}


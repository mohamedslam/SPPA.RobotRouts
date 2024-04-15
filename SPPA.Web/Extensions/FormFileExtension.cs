using SPPA.Logic.Utils;

namespace SPPA.Web.Extensions;

public static class FormFileExtension
{

    public static async Task<TmpFile> ToTmpFileAsync(this IFormFile formFile)
    {
        using (var readStream = formFile.OpenReadStream())
        {
            var result = await TmpFile.Create(formFile.FileName, readStream);
            return result;
        }
    }

}

using HSEDataBase.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HSEDataBase.Services;

/// <summary>
/// Сервис, отвечающий за загрузку таблиц на сервер.
/// </summary>
public class FileUploadService : IFileUploadService
{
    public string UploadFile(IFormFile formFile)
    {
        if (formFile.Length > 0 && (formFile.FileName.EndsWith(".txt") || formFile.FileName.EndsWith(".json")))
        {
            var path = Path.GetTempFileName();
            using var fs = new FileStream(path, FileMode.Open);
            formFile.CopyTo(fs);
            return path;
        }

        throw new FileLoadException("Попытка загрузки пустого файла или файла с неверным расширением. Используйте расширение txt или json");
    }
}
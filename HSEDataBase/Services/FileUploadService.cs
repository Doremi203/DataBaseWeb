using HSEDataBase.Interfaces;

namespace HSEDataBase.Services;

/// <summary>
/// Сервис, отвечающий за загрузку таблиц на сервер.
/// </summary>
public class FileUploadService : IFileUploadService
{
    /// <summary>
    /// Метод загрузки таблицы на сервер.
    /// </summary>
    /// <param name="formFile">Форма, содержащая файл.</param>
    /// <param name="type">Тип таблицы.</param>
    /// <returns>Возвращает true, если файл успешно загружен и false, если нет.</returns>
    public bool UploadFile(IFormFile formFile, string type)
    {
        try
        {
            if (formFile.Length > 0 && (formFile.FileName.EndsWith(".txt") || formFile.FileName.EndsWith(".json")))
            {
                using var fs = new FileStream(@$"Data/{type}.json", FileMode.Create);
                formFile.CopyTo(fs);
                return true;
            }

            return false;
        }
        catch
        {
            return false;
        }
    }
}
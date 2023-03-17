namespace HSEDataBase.Interfaces;

/// <summary>
/// Интерфейс сервиса, отвечающего за загрузку таблиц на сервер.
/// </summary>
public interface IFileUploadService
{
    bool UploadFile(IFormFile file, string type);
}
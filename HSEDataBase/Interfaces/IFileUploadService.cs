using Microsoft.AspNetCore.Mvc;

namespace HSEDataBase.Interfaces;

/// <summary>
/// Интерфейс сервиса, отвечающего за загрузку таблиц на сервер.
/// </summary>
public interface IFileUploadService
{
    /// <summary>
    /// Метод создания получения пути temp файла используемого для загрузки информации из файла на сервер.
    /// </summary>
    /// <param name="file">Форма содержащая файл.</param>
    /// <returns>Путь созданного temp файла.</returns>
    string UploadFile(IFormFile file);
}
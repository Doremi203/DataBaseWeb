using System.Text;
using HSEDataBase.Interfaces;
using HSEDataBase.Models;
using Microsoft.AspNetCore.Mvc;

namespace HSEDataBase.Controllers;

/// <summary>
/// Контролер отвечающий за запросы связанные с файлами.
/// </summary>
public class FileController : Controller
{
    private readonly IFileUploadService _fileUploadService;
    
    public FileController(IFileUploadService fileUploadService)
    {
        _fileUploadService = fileUploadService;
    }

    public IActionResult Index(string type)
    {
        return RedirectToAction("Index",$"{type}");
    }
    
    /// <summary>
    /// Get запрос загрузки таблицы на сервер.
    /// </summary>
    /// <param name="type">Тип таблицы загружаемой на сервер.</param>
    /// <returns></returns>
    public IActionResult Upload(string type)
    {
        return View(new FileViewModel{Type = type});
    }

    /// <summary>
    /// HHTP запрос загрузки таблицы на сервер.
    /// </summary>
    /// <param name="formFile">Форма, содержащая файл.</param>
    /// <param name="type">Тип таблицы.</param>
    /// <returns></returns>
    /// <exception cref="FileLoadException">Исключение выбрасывается, если загрузка файла не удалась</exception>
    [HttpPost]
    public IActionResult Upload(IFormFile formFile, string type)
    {
        try
        {
            if (!_fileUploadService.UploadFile(formFile, type))
                throw new FileLoadException();
            return RedirectToAction("Load",$"{type}");
        }
        catch
        {
            return View("DataBaseError", new DataBaseErrorViewModel{Message = "Ошибка загрузки файла", Type = type});
        }
    }
    
    /// <summary>
    /// Get запрос скачивания таблицы.
    /// </summary>
    /// <param name="type">Тип таблицы.</param>
    /// <returns></returns>
    public IActionResult Download(string type)
    {
        try
        {
            var fs = new FileStream(@$"Data/{type}.json", FileMode.Open, FileAccess.Read);
            System.IO.File.Delete(@$"Data/{type}.json");
            return File(fs, "text/plain", $"{type}.json");
        }
        catch
        {
            return View("DataBaseError", new DataBaseErrorViewModel{Message = "Ошибка скачивания файла", Type = type});
        }
    }
}
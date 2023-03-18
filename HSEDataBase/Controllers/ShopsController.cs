using DataAccess.Exceptions;
using DataAccess.Interfaces;
using DataAccess.Models;
using HSEDataBase.Interfaces;
using HSEDataBase.Models;
using Microsoft.AspNetCore.Mvc;

namespace HSEDataBase.Controllers;

/// <summary>
/// Контролер запросов связанных с таблицей магазинов.
/// </summary>
public class ShopsController : Controller
{
    private readonly IDataBase _dataBase;
    private readonly IFileUploadService _fileUploadService;

    public ShopsController(IDataBase dataBase, IFileUploadService fileUploadService)
    {
        _dataBase = dataBase;
        _fileUploadService = fileUploadService;
    }
    // GET
    public IActionResult Index()
    {
        ViewData["Title"] = "Shops table";
        try
        {
            return View(new ShopViewModel{Shops = _dataBase.GetTable<Shop>()});
        }
        catch (DataBaseException)
        {
            return View(new ShopViewModel());
        }
    }

    /// <summary>
    /// Get запрос создания таблицы магазинов.
    /// </summary>
    /// <returns></returns>
    public IActionResult Create()
    {
        try
        {
            _dataBase.CreateTable<Shop>();
            return RedirectToAction("Index");
        }
        catch (DataBaseException e)
        {
            return View("DataBaseError", new DataBaseErrorViewModel{Message = e.Message});
        }
    }

    /// <summary>
    /// Get запрос добавления магазина в таблицу.
    /// </summary>
    /// <returns></returns>
    public IActionResult Insert()
    {
        return View();
    }

    /// <summary>
    /// Post запрос добавления продажи в магазина.
    /// </summary>
    /// <param name="form">Форма, содержащая данные для добавления магазина в таблицу.</param>
    /// <returns></returns>
    [HttpPost]
    public IActionResult Insert(IFormCollection form)
    {
        try
        {
            var shop = new Shop(int.Parse(form["Id"].ToString()), form["Name"].ToString(), form["City"].ToString(), form["Country"].ToString());
            _dataBase.InsertInto(() => shop);
            return RedirectToAction("Index");
        }
        catch (DataBaseException e)
        {
            return View("DataBaseError", new DataBaseErrorViewModel{Message = e.Message});
        }
    }

    /// <summary>
    /// Get запрос загрузки файла магазинов.
    /// </summary>
    /// <returns></returns>
    public IActionResult Upload()
    {
        return View();
    }

    /// <summary>
    /// Post запрос загрузки файла продаж.
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    public IActionResult Upload(IFormFile formFile)
    {
        try
        {
            var path = _fileUploadService.UploadFile(formFile);
            _dataBase.Deserialize<Shop>(path);
            System.IO.File.Delete(path);
            return RedirectToAction("Index");
        }
        catch (DataBaseException e)
        {
            return View("DataBaseError", new DataBaseErrorViewModel{Message = e.Message});
        }
        catch (FileLoadException e)
        {
            return View("DataBaseError", new DataBaseErrorViewModel{Message = e.Message});
        }
        catch (Exception)
        {
            return View("DataBaseError", new DataBaseErrorViewModel{Message = "Ошибка загрузки файла"});
        }
    }
    
    /// <summary>
    /// Get запрос сохранения файла с магазинами.
    /// </summary>
    /// <returns></returns>
    public IActionResult Download()
    {
        try
        {
            var path = Path.GetTempFileName();
            _dataBase.Serialize<Shop>(path);
            var fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            System.IO.File.Delete(path);
            return File(fs, "text/plain", "Shops.json");
        }
        catch (DataBaseException e)
        {
            return View("DataBaseError", new DataBaseErrorViewModel{Message = e.Message});
        }
        catch (Exception)
        {
            return View("DataBaseError", new DataBaseErrorViewModel{Message = "Ошибка скачивания файла"});
        }
    }
}
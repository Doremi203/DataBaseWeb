using DataAccess.Exceptions;
using DataAccess.Interfaces;
using DataAccess.Models;
using HSEDataBase.Interfaces;
using HSEDataBase.Models;
using Microsoft.AspNetCore.Mvc;

namespace HSEDataBase.Controllers;

/// <summary>
/// Контролер запросов связанных с таблицей продаж.
/// </summary>
public class SalesController : Controller
{
    private readonly IDataBase _dataBase;
    private readonly IFileUploadService _fileUploadService;

    public SalesController(IDataBase dataBase, IFileUploadService fileUploadService)
    {
        _dataBase = dataBase;
        _fileUploadService = fileUploadService;
    }
    // GET
    public IActionResult Index()
    {
        ViewData["Title"] = "Sales table";
        try
        {
            return View(new SaleViewModel{Sales = _dataBase.GetTable<Sale>()});
        }
        catch (DataBaseException)
        {
            return View(new SaleViewModel());
        }
    }
    
    /// <summary>
    /// Get запрос создания таблицы продаж.
    /// </summary>
    /// <returns></returns>
    public IActionResult Create()
    {
        try
        {
            _dataBase.CreateTable<Sale>();
            return RedirectToAction("Index");
        }
        catch (DataBaseException e)
        {
            return View("DataBaseError", new DataBaseErrorViewModel{Message = e.Message});
        }
    }

    /// <summary>
    /// Get запрос добавления продажи в таблицу.
    /// </summary>
    /// <returns></returns>
    public IActionResult Insert()
    {
        return View();
    }
    
    /// <summary>
    /// Post запрос добавления продажи в таблицу.
    /// </summary>
    /// <param name="form">Форма, содержащая данные для добавления продажи в таблицу.</param>
    /// <returns></returns>
    [HttpPost]
    public IActionResult Insert(IFormCollection form)
    {
        try
        {
            var sale = new Sale(int.Parse(form["Id"].ToString()), int.Parse(form["BuyerId"].ToString()), int.Parse(form["ShopId"].ToString()), int.Parse(form["GoodId"].ToString()), long.Parse(form["GoodCount"].ToString()));
            _dataBase.InsertInto(() => sale);
            return RedirectToAction("Index");
        }
        catch (DataBaseException e)
        {
            return View("DataBaseError", new DataBaseErrorViewModel{Message = e.Message});
        }
    }
    
    /// <summary>
    /// Get запрос загрузки файла продаж.
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
            _dataBase.Deserialize<Sale>(path);
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
    /// Get запрос сохранения файла с продажами.
    /// </summary>
    /// <returns></returns>
    public IActionResult Download()
    {
        try
        {
            var path = Path.GetTempFileName();
            _dataBase.Serialize<Sale>(path);
            var fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            System.IO.File.Delete(path);
            return File(fs, "text/plain", "Sales.json");
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
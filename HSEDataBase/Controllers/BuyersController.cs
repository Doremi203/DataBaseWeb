using DataAccess.Exceptions;
using DataAccess.Interfaces;
using DataAccess.Models;
using HSEDataBase.Interfaces;
using HSEDataBase.Models;
using Microsoft.AspNetCore.Mvc;

namespace HSEDataBase.Controllers;

/// <summary>
/// Контролер запросов связанных с таблицей покупателей.
/// </summary>
public class BuyersController : Controller
{
    private readonly IDataBase _dataBase;
    private readonly IFileUploadService _fileUploadService;

    public BuyersController(IDataBase dataBase, IFileUploadService fileUploadService)
    {
        _dataBase = dataBase;
        _fileUploadService = fileUploadService;
    }
    
    // GET
    public IActionResult Index()
    {
        ViewData["Title"] = "Buyers table";
        try
        {
            return View(new BuyerViewModel{Buyers = _dataBase.GetTable<Buyer>()});
        }
        catch (DataBaseException)
        {
            return View(new BuyerViewModel());
        }
    }
    
    /// <summary>
    /// Get запрос создания таблицы покупателей.
    /// </summary>
    /// <returns></returns>
    public IActionResult Create()
    {
        try
        {
            _dataBase.CreateTable<Buyer>();
            return RedirectToAction("Index");
        }
        catch (DataBaseException e)
        {
            return View("DataBaseError", new DataBaseErrorViewModel{Message = e.Message});
        }
    }

    /// <summary>
    /// Get запрос добавления покупателя в таблицу.
    /// </summary>
    /// <returns></returns>
    public IActionResult Insert()
    {
        return View();
    }
    
    /// <summary>
    /// Post запрос добавления покупателя в таблицу.
    /// </summary>
    /// <param name="form">Форма, содержащая данные для добавления покупателя в таблицу.</param>
    /// <returns></returns>
    [HttpPost]
    public IActionResult Insert(IFormCollection form)
    {
        try
        {
            var buyer = new Buyer(int.Parse(form["Id"].ToString()), form["Name"].ToString(), form["Surname"].ToString(), form["City"].ToString(), form["Country"].ToString());
            _dataBase.InsertInto(() => buyer);
            return RedirectToAction("Index");
        }
        catch (DataBaseException e)
        {
            return View("DataBaseError", new DataBaseErrorViewModel{Message = e.Message});
        }
    }

    /// <summary>
    /// Get запрос загрузки файла покупателей.
    /// </summary>
    /// <returns></returns>
    public IActionResult Upload()
    {
        return View();
    }

    /// <summary>
    /// Post запрос загрузки файла покупателей.
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    public IActionResult Upload(IFormFile formFile)
    {
        try
        {
            var path = _fileUploadService.UploadFile(formFile);
            _dataBase.Deserialize<Buyer>(path);
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
    /// Get запрос сохранения файла с покупателями.
    /// </summary>
    /// <returns></returns>
    public IActionResult Download()
    {
        try
        {
            var path = Path.GetTempFileName();
            _dataBase.Serialize<Buyer>(path);
            var fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            System.IO.File.Delete(path);
            return File(fs, "text/plain", "Buyers.json");
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
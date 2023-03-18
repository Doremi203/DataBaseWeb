using DataAccess.Exceptions;
using DataAccess.Interfaces;
using DataAccess.Models;
using HSEDataBase.Interfaces;
using HSEDataBase.Models;
using Microsoft.AspNetCore.Mvc;

namespace HSEDataBase.Controllers;

/// <summary>
/// Контролер запросов связанных с таблицей товаров.
/// </summary>
public class GoodsController : Controller
{
    private readonly IDataBase _dataBase;
    private readonly IFileUploadService _fileUploadService;

    public GoodsController(IDataBase dataBase, IFileUploadService fileUploadService)
    {
        _dataBase = dataBase;
        _fileUploadService = fileUploadService;
    }
    
    // GET
    public IActionResult Index()
    {
        ViewData["Title"] = "Goods table";
        try
        {
            return View(new GoodViewModel{Goods = _dataBase.GetTable<Good>()});
        }
        catch (DataBaseException)
        {
            return View(new GoodViewModel());
        }
    }
    
    /// <summary>
    /// Get запрос создания таблицы товаров.
    /// </summary>
    /// <returns></returns>
    public IActionResult Create()
    {
        try
        {
            _dataBase.CreateTable<Good>();
            return RedirectToAction("Index");
        }
        catch (DataBaseException e)
        {
            return View("DataBaseError", new DataBaseErrorViewModel{Message = e.Message});
        }
    }

    /// <summary>
    /// Get запрос добавления товара в таблицу.
    /// </summary>
    /// <returns></returns>
    public IActionResult Insert()
    {
        return View();
    }
    
    /// <summary>
    /// Post запрос добавления товара в таблицу.
    /// </summary>
    /// <param name="form">Форма, содержащая данные для добавления товара в таблицу.</param>
    /// <returns></returns>
    [HttpPost]
    public IActionResult Insert(IFormCollection form)
    {
        try
        {
            var good = new Good(int.Parse(form["Id"].ToString()), form["Name"].ToString(), int.Parse(form["ShopId"].ToString()), form["Category"].ToString(), int.Parse(form["Price"].ToString()));
            _dataBase.InsertInto(() => good);
            return RedirectToAction("Index");
        }
        catch (DataBaseException e)
        {
            return View("DataBaseError", new DataBaseErrorViewModel{Message = e.Message});
        }
    }

    /// <summary>
    /// Get запрос загрузки файла товаров.
    /// </summary>
    /// <returns></returns>
    public IActionResult Upload()
    {
        return View();
    }

    /// <summary>
    /// Post запрос загрузки файла товаров.
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    public IActionResult Upload(IFormFile formFile)
    {
        try
        {
            var path = _fileUploadService.UploadFile(formFile);
            _dataBase.Deserialize<Good>(path);
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
    /// Get запрос сохранения файла с товарами.
    /// </summary>
    /// <returns></returns>
    public IActionResult Download()
    {
        try
        {
            var path = Path.GetTempFileName();
            _dataBase.Serialize<Good>(path);
            var fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            System.IO.File.Delete(path);
            return File(fs, "text/plain", "Goods.json");
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
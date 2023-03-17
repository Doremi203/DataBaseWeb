using DataAccess.Exceptions;
using DataAccess.Interfaces;
using DataAccess.Models;
using HSEDataBase.Models;
using Microsoft.AspNetCore.Mvc;

namespace HSEDataBase.Controllers;

/// <summary>
/// Контролер запросов связанных с таблицей покупателей.
/// </summary>
public class BuyersController : Controller
{
    private readonly IDataBase _dataBase;

    public BuyersController(IDataBase dataBase)
    {
        _dataBase = dataBase;
    }
    
    // GET
    public IActionResult Index()
    {
        ViewData["Title"] = "Buyers table";
        try
        {
            return View(new BuyerViewModel{Buyers = _dataBase.GetTable<Buyer>()});
        }
        catch
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
    public IActionResult Load()
    {
        try
        {
            _dataBase.Deserialize<Buyer>(@"Data/Buyers.json");
            System.IO.File.Delete(@"Data/Buyers.json");
            return RedirectToAction("Index");
        }
        catch
        {
            return View("DataBaseError", new DataBaseErrorViewModel{Message = "Ошибка Десериализации"});
        }
    }

    /// <summary>
    /// Get запрос сохранения файла с покупателями.
    /// </summary>
    /// <returns></returns>
    public IActionResult Save()
    {
        try
        {
            _dataBase.Serialize<Buyer>(@"Data/Buyers.json");
            return RedirectToAction("Download","File", new {type = "Buyers"});
        }
        catch
        {
            return View("DataBaseError", new DataBaseErrorViewModel{Message = "Ошибка сериализации"});
        }
    }
}
using DataAccess.Exceptions;
using DataAccess.Interfaces;
using DataAccess.Models;
using HSEDataBase.Models;
using Microsoft.AspNetCore.Mvc;

namespace HSEDataBase.Controllers;

/// <summary>
/// Контролер запросов связанных с таблицей продаж.
/// </summary>
public class SalesController : Controller
{
    private readonly IDataBase _dataBase;

    public SalesController(IDataBase dataBase)
    {
        _dataBase = dataBase;
    }
    // GET
    public IActionResult Index()
    {
        ViewData["Title"] = "Sales table";
        try
        {
            return View(new SaleViewModel{Sales = _dataBase.GetTable<Sale>()});
        }
        catch
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
    public IActionResult Load()
    {
        try
        {
            _dataBase.Deserialize<Sale>(@"Data/Sales.json");
            System.IO.File.Delete(@"Data/Sales.json");
            return RedirectToAction("Index");
        }
        catch
        {
            return View("DataBaseError", new DataBaseErrorViewModel{Message = "Ошибка Десериализации"});
        }
    }

    /// <summary>
    /// Get запрос сохранения файла с продажами.
    /// </summary>
    /// <returns></returns>
    public IActionResult Save()
    {
        try
        {
            _dataBase.Serialize<Sale>(@"Data/Sales.json");
            return RedirectToAction("Download","File", new {type = "Sales"});
        }
        catch
        {
            return View("DataBaseError", new DataBaseErrorViewModel{Message = "Ошибка сериализации"});
        }
    }
}
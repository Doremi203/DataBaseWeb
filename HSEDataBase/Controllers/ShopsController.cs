using DataAccess.Exceptions;
using DataAccess.Interfaces;
using DataAccess.Models;
using HSEDataBase.Models;
using Microsoft.AspNetCore.Mvc;

namespace HSEDataBase.Controllers;

/// <summary>
/// Контролер запросов связанных с таблицей магазинов.
/// </summary>
public class ShopsController : Controller
{
    private readonly IDataBase _dataBase;

    public ShopsController(IDataBase dataBase)
    {
        _dataBase = dataBase;
    }
    // GET
    public IActionResult Index()
    {
        ViewData["Title"] = "Shops table";
        try
        {
            return View(new ShopViewModel{Shops = _dataBase.GetTable<Shop>()});
        }
        catch
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
    public IActionResult Load()
    {
        try
        {
            _dataBase.Deserialize<Shop>(@"Data/Shops.json");
            System.IO.File.Delete(@"Data/Shops.json");
            return RedirectToAction("Index");
        }
        catch
        {
            return View("DataBaseError", new DataBaseErrorViewModel{Message = "Ошибка Десериализации"});
        }
    }

    /// <summary>
    /// Get запрос сохранения файла с магазинами.
    /// </summary>
    /// <returns></returns>
    public IActionResult Save()
    {
        try
        {
            _dataBase.Serialize<Shop>(@"Data/Shops.json");
            return RedirectToAction("Download","File", new {type = "Shops"});
        }
        catch
        {
            return View("DataBaseError", new DataBaseErrorViewModel{Message = "Ошибка сериализации"});
        }
    }
}
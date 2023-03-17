using DataAccess.Exceptions;
using DataAccess.Interfaces;
using DataAccess.Models;
using HSEDataBase.Models;
using Microsoft.AspNetCore.Mvc;

namespace HSEDataBase.Controllers;

/// <summary>
/// Контролер запросов связанных с таблицей товаров.
/// </summary>
public class GoodsController : Controller
{
    private readonly IDataBase _dataBase;

    public GoodsController(IDataBase dataBase)
    {
        _dataBase = dataBase;
    }
    
    // GET
    public IActionResult Index()
    {
        ViewData["Title"] = "Goods table";
        try
        {
            return View(new GoodViewModel{Goods = _dataBase.GetTable<Good>()});
        }
        catch
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
    /// Get запрос загрузки файла покупателей.
    /// </summary>
    /// <returns></returns>
    public IActionResult Load()
    {
        try
        {
            _dataBase.Deserialize<Good>(@"Data/Goods.json");
            System.IO.File.Delete(@"Data/Goods.json");
            return RedirectToAction("Index");
        }
        catch
        {
            return View("DataBaseError", new DataBaseErrorViewModel{Message = "Ошибка Десериализации"});
        }
    }

    /// <summary>
    /// Get запрос сохранения файла с товарами.
    /// </summary>
    /// <returns></returns>
    public IActionResult Save()
    {
        try
        {
            _dataBase.Serialize<Good>(@"Data/Goods.json");
            return RedirectToAction("Download","File", new {type = "Goods"});
        }
        catch
        {
            return View("DataBaseError", new DataBaseErrorViewModel{Message = "Ошибка сериализации"});
        }
    }
}
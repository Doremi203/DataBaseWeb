using DataAccess.Exceptions;
using DataAccess.Interfaces;
using HSEDataBase.Models;
using Microsoft.AspNetCore.Mvc;

namespace HSEDataBase.Controllers;

/// <summary>
/// Контролер запросов связанных с домашней страницей.
/// </summary>
public class HomeController : Controller
{
    private readonly IDataAccessLayer _dataAccessLayer;
    private readonly IDataBase _dataBase;

    // GET
    public HomeController(IDataAccessLayer dataAccessLayer, IDataBase dataBase)
    {
        _dataAccessLayer = dataAccessLayer;
        _dataBase = dataBase;
    }
    public IActionResult Index()
    {
        return View();
    }
    
    public IActionResult AllGoodsOfLongestNameBuyer()
    {
        try
        {
            var resGoods = _dataAccessLayer.GetAllGoodsOfLongestNameBuyer(_dataBase);
            ViewData["Title"] = "Cписок всех товаров, купленных покупателем с самым длинным именем";
            return View("../Goods/Index",new GoodViewModel{Goods = resGoods});
        }
        catch (DataBaseException)
        {
            return View("DataBaseError", new DataBaseErrorViewModel{Message = "Ошибка, недостаточно информации для вывода"});
        }
    }

    public IActionResult MostExpensiveGoodCategory()
    {
        try
        {
            var resCategory = _dataAccessLayer.GetMostExpensiveGoodCategory(_dataBase);
            ViewData["ResultType"] = "Категория самого дорого товара";
            ViewData["Result"] = resCategory ?? throw new DataBaseException();
            return View("Result");
        }
        catch (DataBaseException)
        {
            return View("DataBaseError", new DataBaseErrorViewModel{Message = "Недостаточно данных, чтобы вычислить категорию"});
        }
    }

    public IActionResult MinimumSalesCity()
    {
        try
        {
            var resCity = _dataAccessLayer.GetMinimumSalesCity(_dataBase);
            ViewData["ResultType"] = "Город с наименьшими продажами";
            ViewData["Result"] = resCity ?? throw new DataBaseException();
            return View("Result");
        }
        catch (DataBaseException)
        {
            return View("DataBaseError", new DataBaseErrorViewModel{Message = "Недостаточно данных, чтобы вычислить город"});
        }
    }

    public IActionResult MostPopularGoodBuyers()
    {
        try
        {
            var resBuyers = _dataAccessLayer.GetMostPopularGoodBuyers(_dataBase);
            ViewData["Title"] = "Cписок покупателей, которые купили самый популярный товар";
            return View("../Buyers/Index",new BuyerViewModel{Buyers = resBuyers});
        }
        catch (DataBaseException)
        {
            return View("DataBaseError", new DataBaseErrorViewModel{Message = "Недостаточно данных, чтобы составить список покупателей"});
        }
    }

    public IActionResult MinimumNumberOfShopsInCountry()
    {
        try
        {
            var resNumber = _dataAccessLayer.GetMinimumNumberOfShopsInCountry(_dataBase);
            ViewData["ResultType"] = "Минимальное число магазинов";
            ViewData["Result"] = resNumber;
            return View("Result");
        }
        catch (DataBaseException)
        {
            return View("DataBaseError", new DataBaseErrorViewModel{Message = "Недостаточно данных, чтобы вычислить число магазинов"});
        }
    }

    public IActionResult OtherCitySales()
    {
        try
        {
            var resSales = _dataAccessLayer.GetOtherCitySales(_dataBase);
            ViewData["Title"] = "Cписок покупок, совершенных покупателями во всех городах, отличных от города их проживания";
            return View("../Sales/Index",new SaleViewModel{Sales = resSales});
        }
        catch (DataBaseException)
        {
            return View("DataBaseError", new DataBaseErrorViewModel{Message = "Недостаточно данных, чтобы составить список продаж"});
        }
    }

    public IActionResult TotalSalesValue()
    {
        try
        {
            var resNumber = _dataAccessLayer.GetTotalSalesValue(_dataBase);
            ViewData["ResultType"] = "Общее число выручки с продаж";
            ViewData["Result"] = resNumber;
            return View("Result");
        }
        catch (DataBaseException)
        {
            return View("DataBaseError", new DataBaseErrorViewModel { Message = "Недостаточно данных, чтобы вычислить число выручки" });
        }
        catch (NullReferenceException)
        {
            return View("DataBaseError", new DataBaseErrorViewModel { Message = "В данных присутсвует некорректная покупка, вычисления невозможны" });
        }
    }
}
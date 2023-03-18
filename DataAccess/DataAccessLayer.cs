using DataAccess.Exceptions;
using DataAccess.Interfaces;
using DataAccess.Models;

namespace DataAccess;

public class DataAccessLayer : IDataAccessLayer
{

    public DataAccessLayer() { }

    /// <summary>
    /// Метод получения последовательности товаров покупателя с самым длинным именем.
    /// </summary>
    /// <param name="dataBase">База данных из которой берутся данные.</param>
    /// <returns>Последовательность товаров покупателя с самым длинным именем.</returns>
    public IEnumerable<Good> GetAllGoodsOfLongestNameBuyer(IDataBase dataBase)
    {
        var buyers = dataBase.GetTable<Buyer>().ToList();
        var sales = dataBase.GetTable<Sale>().ToList();
        var goods = dataBase.GetTable<Good>().ToList();
        
        if (!buyers.Any() || !sales.Any() || !goods.Any())
            return new List<Good>();
        
        var buyerWithLongestName = buyers
            .OrderBy(x => x.Name)
            .Last(buyer => buyer.Name.Length == buyers.Max(buyer1 => buyer1.Name.Length));

        return goods.Where(good => sales
            .Where(sale => sale.BuyerId == buyerWithLongestName.Id)
            .Select(sale => sale.GoodId).Contains(good.Id));
    }

    /// <summary>
    /// Метод получения категории самого дорого товара.
    /// </summary>
    /// <param name="dataBase">База данных из которой берутся данные.</param>
    /// <returns>Возвращает категорию самого дорогого товара.</returns>
    public string? GetMostExpensiveGoodCategory(IDataBase dataBase)
    {
        var goods = dataBase.GetTable<Good>().ToList();
        return !goods.Any() ? null : goods.First(good => good.Price == goods.Max(good1 => good1.Price)).Category;
    }
    
    /// <summary>
    /// Метод получения города с минимальными продажами.
    /// </summary>
    /// <param name="dataBase">База данных из которой берутся данные.</param>
    /// <returns>Возвращает город с минимальными продажами.</returns>
    public string? GetMinimumSalesCity(IDataBase dataBase)
    {
        var shops = dataBase.GetTable<Shop>().ToList();

        if (!shops.Any())
            return null;

        List<Good> goods;
        List<Sale> sales;
        try
        {
            goods = dataBase.GetTable<Good>().ToList();
            sales = dataBase.GetTable<Sale>().ToList();
        }
        catch (DataBaseException)
        {
            return shops.First().City; 
        }

        if (!sales.Any() || !goods.Any())
            return shops.First().City;

        // Поиск магазина в котором не было покупок вообще
        var x = shops.Find(shop => !sales.Select(sale => sale.ShopId).Contains(shop.Id));
        if (x is not null)
            return x.City;

        var q = shops.Join(
            sales.Join(goods, sale => sale.GoodId, good => good.Id,
                (sale, good) => new { sale.ShopId, soldValue = good.Price * sale.GoodCount }), shop => shop.Id, arg => arg.ShopId,
            (shop, enumerable) => new { shop.City, enumerable.soldValue}).GroupBy(arg => arg.City)
            .Select(grouping => new { grouping.Key , sum = grouping.Sum(arg => arg.soldValue)}).ToList();
        return q.First(arg => arg.sum == q.Min(arg1 => arg1.sum)).Key;
        
    }

    /// <summary>
    /// Метод получения последовательности покупателей, купившыих самый популярный товар.
    /// </summary>
    /// <param name="dataBase">База данных из которой берутся данные.</param>
    /// <returns>Последовательность покупателей, купившыих самый популярный товар.</returns>
    public IEnumerable<Buyer> GetMostPopularGoodBuyers(IDataBase dataBase)
    {
        var sales = dataBase.GetTable<Sale>().ToList();
        var buyers = dataBase.GetTable<Buyer>().ToList();
        
        if (!buyers.Any() || !sales.Any())
            return new List<Buyer>();

        var totalGoodCountBySaleId = sales.GroupBy(sale => sale.GoodId, sale => sale.GoodCount,
            (id, longs) => new { Id = id, totalGoodCount = longs.Sum()}).ToList();

        var mostPopularGood =
            totalGoodCountBySaleId.First(pair => pair.totalGoodCount == totalGoodCountBySaleId.Max(arg => arg.totalGoodCount)).Id;

        var buyersOfMostPopularGoodIds = sales.GroupBy(sale => sale.GoodId)
            .Where(grouping => grouping.Key == mostPopularGood)
            .SelectMany(grouping => grouping.Select(sale => sale.BuyerId));
        
        return buyers.Where(buyer => buyersOfMostPopularGoodIds.Contains(buyer.Id));
    }

    /// <summary>
    /// Метод получения минимального числа магазинов по странам.
    /// </summary>
    /// <param name="dataBase">База данных из которой берутся данные.</param>
    /// <returns>Минимальное число магазинов по странам.</returns>
    public int GetMinimumNumberOfShopsInCountry(IDataBase dataBase)
    {
        var shops = dataBase.GetTable<Shop>().ToList();
        if (!shops.Any())
            return 0;
        var shopsByCountry = shops.GroupBy(shop => shop.Country);
        return shopsByCountry.Min(grouping => grouping.Count());
    }

    /// <summary>
    /// Метод получения последовательности покупок, совершенных покупателями во всех городах, отличных от города их проживания.
    /// </summary>
    /// <param name="dataBase">База данных из которой берутся данные.</param>
    /// <returns>Последовательность покупок, совершенных покупателями во всех городах, отличных от города их проживания.</returns>
    public IEnumerable<Sale> GetOtherCitySales(IDataBase dataBase)
    {
        var sales = dataBase.GetTable<Sale>().ToList();
        var shops = dataBase.GetTable<Shop>().ToList();
        var buyers = dataBase.GetTable<Buyer>().ToList();

        if (!buyers.Any() || !sales.Any() || !shops.Any())
            return new List<Sale>();
        
        return sales.Where(sale => buyers
            .Find(buyer => buyer.Id == sale.BuyerId)?.City != shops.Find(shop => shop.Id == sale.ShopId)?.City);
    }

    /// <summary>
    /// Метод получения общей суммы продаж
    /// </summary>
    /// <param name="dataBase">База данных из которой берутся данные.</param>
    /// <returns>Общая сумма продаж</returns>
    public long GetTotalSalesValue(IDataBase dataBase)
    {
        var sales = dataBase.GetTable<Sale>().ToList();

        if (!sales.Any())
            return 0;

        return sales.Sum(sale => sale.GoodCount * dataBase.GetTable<Good>().ToList()
            .Find(good => good.Id == sale.GoodId)!.Price);
    }
}
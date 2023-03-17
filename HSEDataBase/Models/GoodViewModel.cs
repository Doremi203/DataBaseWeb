using DataAccess.Models;

namespace HSEDataBase.Models;

/// <summary>
/// Класс модели, отвечающей за отображение таблицы товаров.
/// </summary>
public class GoodViewModel
{
    public IEnumerable<Good>? Goods { get; init; }
}
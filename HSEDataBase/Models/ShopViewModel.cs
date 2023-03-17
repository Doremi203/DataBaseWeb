using DataAccess.Models;

namespace HSEDataBase.Models;

/// <summary>
/// Класс модели, отвечающей за отображение таблицы магазинов.
/// </summary>
public class ShopViewModel
{
    public IEnumerable<Shop>? Shops { get; init; }
}
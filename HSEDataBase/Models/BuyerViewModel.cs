using DataAccess.Models;

namespace HSEDataBase.Models;

/// <summary>
/// Класс модели, отвечающей за отображение таблицы покупателей.
/// </summary>
public class BuyerViewModel
{
    public IEnumerable<Buyer>? Buyers { get; init; }
}
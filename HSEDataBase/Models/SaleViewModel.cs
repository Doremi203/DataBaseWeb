using DataAccess.Models;

namespace HSEDataBase.Models;

/// <summary>
/// Класс модели, отвечающей за отображение таблицы продаж.
/// </summary>
public class SaleViewModel
{
    public IEnumerable<Sale>? Sales { get; init; }
}
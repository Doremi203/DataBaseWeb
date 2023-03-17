namespace HSEDataBase.Models;

/// <summary>
/// Класс модели, отвечающей за отображение информации об ошибки.
/// </summary>
public class DataBaseErrorViewModel
{
    /// <summary>
    /// Хранит информацию откуда было брошено исключение.
    /// </summary>
    public string? Type { get; init; }
    public string? Message { get; init; }
}
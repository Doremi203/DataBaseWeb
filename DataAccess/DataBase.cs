using System.Text.Json;
using DataAccess.Exceptions;
using DataAccess.Interfaces;

namespace DataAccess;

public class DataBase : IDataBase
{
    private readonly Dictionary<Type, Dictionary<int, IEntity>> _tables = new();
    
    public DataBase() { }

    /// <summary>
    /// Создание таблицы данного типа.
    /// </summary>
    /// <typeparam name="T">Типизирует таблицу.</typeparam>
    /// <exception cref="DataBaseException">Исключение выбрасывается, если таблица такого типа уже существует.</exception>
    public void CreateTable<T>() where T : IEntity
    {
        if (!_tables.TryAdd(typeof(T), new Dictionary<int, IEntity>()))
            throw new DataBaseException("Таблица такого типа уже существует");
    }

    /// <summary>
    /// Добавляет строку с информацией в таблицу данного типа.
    /// </summary>
    /// <param name="getEntity">Возвращает сущность, добавляемую в таблицу.</param>
    /// <typeparam name="T">Определяет таблицу в которую добавляется строка.</typeparam>
    /// <exception cref="DataBaseException">Выбрасывается, при попытке добавить строку в несуществующую таблицу или если элемент с таким ID уже был добавлен.</exception>
    
    public void InsertInto<T>(Func<T> getEntity) where T : IEntity
    {
        if (!_tables.ContainsKey(typeof(T)))
            throw new DataBaseException("Нельзя добавить строку в несуществующую таблицу");

        var item = getEntity();
        
        if (_tables[typeof(T)].ContainsKey(item.Id))
            throw new DataBaseException("Нельзя добавить элемент с таким же Id");

        _tables[typeof(T)].Add(item.Id, item);
    }

    /// <summary>
    /// Получить таблицу данного типа.
    /// </summary>
    /// <typeparam name="T">Определяет таблицу какого типа нужно вернуть.</typeparam>
    /// <returns>Таблица типа T в виде IEnumerable.</returns>
    /// <exception cref="DataBaseException">Исключение выбрасывается при попытке получить несуществующую таблицу.</exception>
    
    public IEnumerable<T>GetTable<T>() where T : IEntity
    {
        if (!_tables.ContainsKey(typeof(T)))
            throw new DataBaseException("Нельзя получить несуществующую таблицу");
        return _tables[typeof(T)].Values.Cast<T>();
    }

    /// <summary>
    /// Сериализация таблицы данного типа.
    /// </summary>
    /// <param name="path">Путь по которому сериализуется файл.</param>
    /// <typeparam name="T">Определяет таблицу какого типа нужно сериализовать.</typeparam>
    /// <exception cref="DataBaseException">Исключение выбрасывается при попытке сериализации несуществующей таблицы.</exception>
    
    public void Serialize<T>(string path) where T : IEntity
    {
        if (!_tables.ContainsKey(typeof(T)))
            throw new DataBaseException("Нельзя сериализовать несуществующую таблицу");
        
        var jsonString = JsonSerializer.Serialize(_tables[typeof(T)].Values.Cast<T>());
        File.WriteAllText(path,jsonString);
    }

    /// <summary>
    /// Десериализация таблицы данного типа.
    /// </summary>
    /// <param name="path">Путь по кторому десериализуется файл.</param>
    /// <typeparam name="T">Определяет таблицу какого типа нужно десериализовать.</typeparam>
    
    public void Deserialize<T>(string path) where T : IEntity
    {
        var jsonString = File.ReadAllText(path);

        var x = JsonSerializer.Deserialize<IEnumerable<T>>(jsonString)!;
        _tables[typeof(T)] = x.ToDictionary(entity => entity.Id, entity => (IEntity)entity);
    }
}
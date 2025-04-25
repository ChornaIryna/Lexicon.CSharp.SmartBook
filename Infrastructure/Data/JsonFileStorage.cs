using SmartBook.Core.Entities;
using System.Text.Json;

namespace SmartBook.Infrastructure.Data;
public class JsonFileStorage(string filePath)
{
    private readonly string _filePath = filePath;
    private static readonly JsonSerializerOptions _cachedJsonSerializerOptions = new() { WriteIndented = true };

    public List<Book> LoadBooksFromFile()
    {
        if (!File.Exists(_filePath))
            return [];

        var json = File.ReadAllText(_filePath);
        return JsonSerializer.Deserialize<List<Book>>(json) ?? [];
    }

    public void SaveBooksToFile(List<Book> books)
    {
        var json = JsonSerializer.Serialize(books, _cachedJsonSerializerOptions);
        File.WriteAllText(_filePath, json);
    }
}

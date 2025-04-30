using SmartBook.Core.Entities;
using System.Text.Json;

namespace SmartBook.Infrastructure.Data;
public class JsonFileStorage(string filePath)
{
    private readonly string _filePath = filePath;
    private static readonly JsonSerializerOptions _cachedJsonSerializerOptions = new() { WriteIndented = true };
    public List<Book> LoadBooksFromFile()
    {
        string booksFilePath = Path.Combine(_filePath, "books.json");
        if (!File.Exists(booksFilePath))
            return [];

        var json = File.ReadAllText(booksFilePath);
        return JsonSerializer.Deserialize<List<Book>>(json) ?? [];
    }

    public List<User> LoadUsersFromFile()
    {
        string usersFilePath = Path.Combine(_filePath, "users.json");
        if (!File.Exists(usersFilePath))
            return [];
        var json = File.ReadAllText(usersFilePath);
        return JsonSerializer.Deserialize<List<User>>(json) ?? [];
    }

    public void SaveBooksToFile(List<Book> books)
    {
        string booksFilePath = Path.Combine(_filePath, "books.json");
        var json = JsonSerializer.Serialize(books, _cachedJsonSerializerOptions);
        File.WriteAllText(booksFilePath, json);
    }

    public void SaveUsersToFile(List<User> users)
    {
        string usersFilePath = Path.Combine(_filePath, "users.json");
        var json = JsonSerializer.Serialize(users, _cachedJsonSerializerOptions);
        File.WriteAllText(usersFilePath, json);
    }
}

using SmartBook.Core.Entities;
using SmartBook.Core.Exceptions;
using SmartBook.Core.Interfaces;
using SmartBook.Infrastructure.Data;

namespace SmartBook.Infrastructure.Repositories;

public class LibraryRepository : ILibraryRepository
{
    private readonly JsonFileStorage _jsonFileStorage;
    private List<Book> _books;

    public LibraryRepository(string filePath)
    {
        _jsonFileStorage = new JsonFileStorage(filePath);
        _books = _jsonFileStorage.LoadBooksFromFile();
    }
    public void AddBook(Book book)
    {
        _books.Add(book);
        SaveChanges();
    }

    public IEnumerable<Book> GetAllBooks() => _books.OrderBy(b => b.Title);
    public Book? GetBookByISBN(string isbn) => _books.FirstOrDefault(b => b.ISBN.Equals(isbn, StringComparison.OrdinalIgnoreCase));

    public void RemoveBook(string isbn)
    {
        var book = GetBookByISBN(isbn);
        if (book != null)
        {
            _books.Remove(book);
            SaveChanges();
        }
        else
            throw new BookNotFoundException(isbn);
    }

    public IEnumerable<Book> SearchBooks(string searchTerm) => _books
        .Where(b => b.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    b.Author.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    b.ISBN.Equals(searchTerm, StringComparison.OrdinalIgnoreCase));

    public void UpdateBookStatus(string isbn, bool isBorrowed)
    {
        var book = GetBookByISBN(isbn);
        if (book != null)
        {
            _books.Remove(book);
            book.IsBorrowed = isBorrowed;
            _books.Add(book);
            SaveChanges();
        }
        else
            throw new BookNotFoundException(isbn);
    }

    private void SaveChanges() => _jsonFileStorage.SaveBooksToFile(_books);
}

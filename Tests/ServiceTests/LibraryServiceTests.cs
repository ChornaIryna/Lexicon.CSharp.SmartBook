using SmartBook.Core.Entities;
using SmartBook.Core.Exceptions;
using SmartBook.Core.Interfaces;
using SmartBook.Core.Services;

namespace SmartBook.Tests.ServiceTests;
public class FakeLibraryRepository : ILibraryRepository
{
    private readonly List<Book> _books = new List<Book>();

    public void AddBook(Book book) => _books.Add(book);

    public IEnumerable<Book> GetAllBooks() => _books;

    public Book GetBookByISBN(string isbn) => _books.FirstOrDefault(b => b.ISBN == isbn);

    public IEnumerable<Book> SearchBooks(string searchTerm)
    {
        return _books.Where(b => b.Title.Contains(searchTerm) || b.Author.Contains(searchTerm));
    }

    public void RemoveBook(string isbn)
    {
        var book = _books.FirstOrDefault(b => b.ISBN == isbn);
        if (book != null)
            _books.Remove(book);
    }

    public void UpdateBookStatus(string isbn, bool isBorrowed)
    {
        var book = _books.FirstOrDefault(b => b.ISBN == isbn);
        _books.Remove(book);
        book.IsBorrowed = isBorrowed;
        _books.Add(book);
    }
}

public class LibraryServiceTests
{
    [Fact]
    public void AddBook_ShouldAddBookToRepository()
    {
        var repo = new FakeLibraryRepository();
        var service = new LibraryService(repo);
        var book = new Book("Test", "Author", "ISBN123", "Category");

        service.AddBook(book);

        Assert.Contains(book, repo.GetAllBooks());
    }

    [Fact]
    public void AddBook_DuplicateISBN_ShouldThrowException()
    {
        var repo = new FakeLibraryRepository();
        var service = new LibraryService(repo);
        var book = new Book("Test", "Author", "ISBN123", "Category");

        service.AddBook(book);
        var duplicateBook = new Book("Another Title", "Another Author", "ISBN123", "Category");

        Assert.Throws<DuplicateISBNException>(() => service.AddBook(duplicateBook));
    }

    [Fact]
    public void SearchBooks_ShouldReturnCorrectBooks()
    {
        var repo = new FakeLibraryRepository();
        var service = new LibraryService(repo);
        var book1 = new Book("C# Programming", "Author1", "ISBN1", "Education");
        var book2 = new Book("Java Programming", "Author2", "ISBN2", "Education");
        var book3 = new Book("C# in Depth", "Author3", "ISBN3", "Education");

        repo.AddBook(book1);
        repo.AddBook(book2);
        repo.AddBook(book3);

        var results = service.SearchBooks("C#");

        Assert.Contains(book1, results);
        Assert.Contains(book3, results);
        Assert.DoesNotContain(book2, results);
    }

    [Fact]
    public void RemoveBook_ShouldRemoveBookFromRepository()
    {
        var repo = new FakeLibraryRepository();
        var service = new LibraryService(repo);
        var book = new Book("Test", "Author", "ISBN123", "Category");
        service.AddBook(book);
        service.RemoveBook("ISBN123");
        Assert.DoesNotContain(book, repo.GetAllBooks());
    }

    [Fact]
    public void RemoveBook_BookNotFound_ShouldThrowException()
    {
        var repo = new FakeLibraryRepository();
        var service = new LibraryService(repo);
        var book = new Book("Test", "Author", "ISBN123", "Category");
        service.AddBook(book);
        Assert.Throws<BookNotFoundException>(() => service.RemoveBook("NonExistentISBN"));
    }

    [Fact]
    public void UpdateBookStatus_ShouldUpdateBookStatus()
    {
        var repo = new FakeLibraryRepository();
        var service = new LibraryService(repo);
        var book = new Book("Test", "Author", "ISBN123", "Category");
        service.AddBook(book);
        service.UpdateBookStatus("ISBN123", true);
        Assert.True(repo.GetBookByISBN("ISBN123").IsBorrowed);
    }
}

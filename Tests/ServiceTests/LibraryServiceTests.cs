using SmartBook.Core.DTOs;
using SmartBook.Core.Entities;
using SmartBook.Core.Exceptions;
using SmartBook.Core.Interfaces;
using SmartBook.Core.Services;

namespace SmartBook.Tests.ServiceTests;
public class FakeLibraryRepository : ILibraryRepository
{
    private readonly List<Book> _books = [];
    private readonly List<User> _users = [];

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

    public void AddUser(User user) => _users.Add(user);

    public void UpdateBookStatus(string isbn, Guid userId)
    {
        var book = _books.FirstOrDefault(book => book.ISBN == isbn);
        var user = _users.FirstOrDefault(user => user.Id.Equals(userId));

        if (book.IsBorrowed && book.BorrowedBy.Equals(user.Id))
            throw new BookIsBorrowedException(book);

        book.IsBorrowed = !book.IsBorrowed;
        book.BorrowedBy = book.IsBorrowed ? userId : null;

    }

    public IEnumerable<User> GetAllUsers() => _users;

    public User GetUserById(Guid id) => _users.FirstOrDefault(u => u.Id.Equals(id));

    public IEnumerable<UserWithBooksDto> GetAllUsersWithBooks()
    {
        Dictionary<Guid, List<Book>> booksByUser = _books
           .Where(b => b.BorrowedBy.HasValue)
           .GroupBy(b => b.BorrowedBy.Value)
           .ToDictionary(g => g.Key, g => g.ToList());

        var usersWithBooks = _users
            .Where(u => booksByUser.ContainsKey(u.Id))
            .Select(u => new UserWithBooksDto
            {
                UserId = u.Id,
                UserName = u.Name,
                BorrowedBooks = booksByUser[u.Id]
            });

        return usersWithBooks;
    }

    public IEnumerable<Book> GetBooksByUserId(Guid userId)
    {
        return _books.Where(b => b.BorrowedBy == userId);
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
        var user = new User("Test User");
        var book = new Book("Test", "Author", "ISBN123", "Category");
        service.AddUser(user);
        service.AddBook(book);
        service.UpdateBookStatus("ISBN123", user.Id);
        Assert.True(book.IsBorrowed);
        Assert.Equal(user.Id, book.BorrowedBy);
    }

    [Fact]
    public void UpdateBookStatus_BookNotFound_ShouldThrowException()
    {
        var repo = new FakeLibraryRepository();
        var service = new LibraryService(repo);
        var user = new User("Test User");
        service.AddUser(user);
        Assert.Throws<BookNotFoundException>(() => service.UpdateBookStatus("NonExistentISBN", user.Id));
    }

    [Fact]
    public void UpdateBookStatus_UserNotFound_ShouldThrowException()
    {
        var repo = new FakeLibraryRepository();
        var service = new LibraryService(repo);
        var book = new Book("Test", "Author", "ISBN123", "Category");
        service.AddBook(book);
        Assert.Throws<UserNotFoundException>(() => service.UpdateBookStatus("ISBN123", Guid.NewGuid()));
    }

    [Fact]
    public void UpdateBookStatus_BookAlreadyBorrowed_ShouldThrowException()
    {
        var repo = new FakeLibraryRepository();
        var service = new LibraryService(repo);
        var user1 = new User("User One");
        var user2 = new User("User Two");
        var book = new Book("Test", "Author", "ISBN123", "Category");
        service.AddUser(user1);
        service.AddUser(user2);
        service.AddBook(book);
        service.UpdateBookStatus("ISBN123", user1.Id);
        Assert.Throws<BookIsBorrowedException>(() => service.UpdateBookStatus("ISBN123", user2.Id));
    }

    [Fact]
    public void GetAllUsersWithBorrowedBooks_ShouldReturnCorrectUsers()
    {
        var repo = new FakeLibraryRepository();
        var service = new LibraryService(repo);
        var user1 = new User("User One");
        var user2 = new User("User Two");
        var book1 = new Book("Test", "Author", "ISBN123", "Category");
        var book2 = new Book("Another Test", "Another Author", "ISBN456", "Category");
        service.AddUser(user1);
        service.AddUser(user2);
        service.AddBook(book1);
        service.AddBook(book2);
        service.UpdateBookStatus("ISBN123", user1.Id);
        service.UpdateBookStatus("ISBN456", user1.Id);
        var usersWithBooks = service.GetAllUsersWithBorrowedBooks();
        Assert.Contains(usersWithBooks, u => u.UserId == user1.Id && u.BorrowedBooks.Count == 2);
        Assert.DoesNotContain(usersWithBooks, u => u.UserId == user2.Id);
    }

    [Fact]
    public void GetBooksByUserId_ShouldReturnCorrectBooks()
    {
        var repo = new FakeLibraryRepository();
        var service = new LibraryService(repo);
        var user = new User("Test User");
        var book1 = new Book("Test", "Author", "ISBN123", "Category");
        var book2 = new Book("Another Test", "Another Author", "ISBN456", "Category");
        service.AddUser(user);
        service.AddBook(book1);
        service.AddBook(book2);
        service.UpdateBookStatus("ISBN123", user.Id);
        service.UpdateBookStatus("ISBN456", user.Id);
        var books = service.GetBooksByUserId(user.Id);
        Assert.Contains(books, b => b.ISBN == "ISBN123");
        Assert.Contains(books, b => b.ISBN == "ISBN456");
    }
}

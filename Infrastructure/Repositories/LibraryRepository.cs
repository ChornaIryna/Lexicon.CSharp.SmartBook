using SmartBook.Core.DTOs;
using SmartBook.Core.Entities;
using SmartBook.Core.Exceptions;
using SmartBook.Core.Interfaces;
using SmartBook.Infrastructure.Data;

namespace SmartBook.Infrastructure.Repositories;

public class LibraryRepository : ILibraryRepository
{
    private readonly JsonFileStorage _jsonFileStorage;
    private readonly List<Book> _books;
    private readonly List<User> _users;


    public LibraryRepository(string filePath)
    {
        _jsonFileStorage = new JsonFileStorage(filePath);
        _books = _jsonFileStorage.LoadBooksFromFile();
        _users = _jsonFileStorage.LoadUsersFromFile();
    }
    public void AddBook(Book book)
    {
        var (valid, result) = book.IsValid();
        if (!valid)
            throw new InvalidBookException(result);
        if (_books.Any(b => b.ISBN.Equals(book.ISBN, StringComparison.OrdinalIgnoreCase)))
            throw new DuplicateISBNException(book.ISBN);
        _books.Add(book);
        SaveBookChanges();
    }

    public void AddUser(User user)
    {
        var (valid, result) = user.IsValid();
        if (!valid)
            throw new InvalidUserException(result);
        if (_users.Any(u => u.Id == user.Id))
            throw new UserAlreadyExistsException(user);
        _users.Add(user);
        SaveUserChanges();
    }

    public IEnumerable<Book> GetAllBooks()
    {
        _ = _books ?? throw new InvalidOperationException("Books collection is null. Ensure the repository is initialized correctly.");
        _ = _books.Count > 0 ? _books : throw new EmptyCollectionException("Library does not have any books.");
        return _books.OrderBy(b => b.Title);
    }

    public IEnumerable<User> GetAllUsers()
    {
        _ = _users ?? throw new InvalidOperationException("Users collection is null. Ensure the repository is initialized correctly.");
        _ = _users.Count > 0 ? _users : throw new EmptyCollectionException("Library does not have any users.");
        return _users.OrderBy(u => u.Name);
    }

    public IEnumerable<UserWithBooksDto> GetAllUsersWithBooks()
    {
        if (_users == null || _books == null)
            throw new InvalidOperationException("Users or books collection is null. Ensure the repository is initialized correctly.");

        Dictionary<Guid, List<Book>> booksByUser = GetBooksGroupedByUser();
        var usersWithBooks = GetAllUsers()
            .Where(u => booksByUser.ContainsKey(u.Id))
            .Select(u => new UserWithBooksDto
            {
                UserId = u.Id,
                UserName = u.Name,
                BorrowedBooks = booksByUser[u.Id]
            });

        return usersWithBooks;
    }

    public Book? GetBookByISBN(string isbn) => _books.FirstOrDefault(b => b.ISBN.Equals(isbn, StringComparison.OrdinalIgnoreCase));

    public IEnumerable<Book> GetBooksByUserId(Guid userId)
    {
        _ = GetAllUsers().FirstOrDefault(u => u.Id == userId) ?? throw new UserNotFoundException(userId.ToString());
        return GetAllBooks().Where(b => b.BorrowedBy == userId);
    }

    public User? GetUserById(Guid id) => _users.FirstOrDefault(u => u.Id == id);

    public void RemoveBook(string isbn)
    {
        var book = GetAllBooks().FirstOrDefault(book => book.ISBN.Equals(isbn, StringComparison.OrdinalIgnoreCase)) ?? throw new BookNotFoundException(isbn);
        if (book.IsBorrowed)
            throw new BookIsBorrowedException(book);

        _books.Remove(book);
        SaveBookChanges();

    }

    public IEnumerable<Book> SearchBooks(string searchTerm) => GetAllBooks()
        .Where(b => b.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    b.Author.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    b.ISBN.Equals(searchTerm, StringComparison.OrdinalIgnoreCase));

    public void UpdateBookStatus(string isbn, Guid userId)
    {
        var user = GetAllUsers().FirstOrDefault(u => u.Id == userId) ?? throw new UserNotFoundException(userId.ToString());
        var bookToUpdate = GetAllBooks().FirstOrDefault(book => book.ISBN.Equals(isbn, StringComparison.OrdinalIgnoreCase)) ?? throw new BookNotFoundException(isbn);
        if (bookToUpdate.IsBorrowed && !user.Id.Equals(bookToUpdate.BorrowedBy))
            throw new BookIsBorrowedException(bookToUpdate);
        bookToUpdate.IsBorrowed = !bookToUpdate.IsBorrowed;
        bookToUpdate.BorrowedBy = bookToUpdate.IsBorrowed ? user.Id : null;

        SaveBookChanges();
    }

    private Dictionary<Guid, List<Book>> GetBooksGroupedByUser()
    {
        return GetAllBooks()
            .Where(b => b.BorrowedBy.HasValue)
            .GroupBy(b => b.BorrowedBy.Value)
            .ToDictionary(g => g.Key, g => g.ToList());
    }

    private void SaveBookChanges() => _jsonFileStorage.SaveBooksToFile(_books);
    private void SaveUserChanges() => _jsonFileStorage.SaveUsersToFile(_users);
}

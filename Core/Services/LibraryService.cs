using SmartBook.Core.DTOs;
using SmartBook.Core.Entities;
using SmartBook.Core.Exceptions;
using SmartBook.Core.Interfaces;

namespace SmartBook.Core.Services;
public class LibraryService
{
    private readonly ILibraryRepository _libraryRepository;
    public LibraryService(ILibraryRepository libraryRepository)
    {
        _libraryRepository = libraryRepository;
    }

    public void AddBook(Book book)
    {
        var (isValid, validationMessage) = book.IsValid();
        if (!isValid)
            throw new InvalidBookException(validationMessage);
        if (_libraryRepository.GetBookByISBN(book.ISBN) != null)
            throw new DuplicateISBNException(book.ISBN);

        _libraryRepository.AddBook(book);
    }

    public void RemoveBook(string isbn)
    {
        _ = _libraryRepository.GetBookByISBN(isbn) ?? throw new BookNotFoundException(isbn);
        _libraryRepository.RemoveBook(isbn);
    }

    public IEnumerable<Book> GetAllBooks() => _libraryRepository.GetAllBooks();

    public IEnumerable<Book> SearchBooks(string searchTerm) => _libraryRepository.SearchBooks(searchTerm);

    public void UpdateBookStatus(string isbn, Guid userId)
    {
        var book = _libraryRepository.GetBookByISBN(isbn) ?? throw new BookNotFoundException(isbn);
        _ = _libraryRepository.GetAllUsers().FirstOrDefault(u => u.Id == userId) ?? throw new UserNotFoundException(userId.ToString());
        if (book.IsBorrowed && book.BorrowedBy != userId)
            throw new BookIsBorrowedException(book);
        _libraryRepository.UpdateBookStatus(isbn, userId);
    }
    public void AddUser(User user)
    {
        var (isValid, validationMessage) = user.IsValid();
        if (!isValid)
            throw new InvalidUserException(validationMessage);
        if (_libraryRepository.GetUserById(user.Id) != null)
            throw new UserAlreadyExistsException(user);

        _libraryRepository.AddUser(user);
    }

    public IEnumerable<User> GetAllUsers() => _libraryRepository.GetAllUsers();
    public IEnumerable<UserWithBooksDto> GetAllUsersWithBorrowedBooks() => _libraryRepository.GetAllUsersWithBooks();

    public User GetUserById(Guid userId) => _libraryRepository.GetUserById(userId);

    public IEnumerable<Book> GetBooksByUserId(Guid userId) => _libraryRepository.GetBooksByUserId(userId);
}

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

    public void AddBook(Book book) => _libraryRepository.AddBook(book);

    public void RemoveBook(string isbn) => _libraryRepository.RemoveBook(isbn);

    public IEnumerable<Book> GetAllBooks() => _libraryRepository.GetAllBooks();

    public IEnumerable<Book> SearchBooks(string searchTerm) => _libraryRepository.SearchBooks(searchTerm);

    public void UpdateBookStatus(string isbn, Guid userId) => _libraryRepository.UpdateBookStatus(isbn, userId);

    public void AddUser(User user) => _libraryRepository.AddUser(user);

    public IEnumerable<User> GetAllUsers() => _libraryRepository.GetAllUsers();
    public IEnumerable<UserWithBooksDto> GetAllUsersWithBorrowedBooks() => _libraryRepository.GetAllUsersWithBooks() ?? [];

    public User GetUserById(Guid userId) => _libraryRepository
        .GetAllUsers()
        .FirstOrDefault(u => u.Id == userId) ?? throw new UserNotFoundException(userId.ToString());

    public IEnumerable<Book> GetBooksByUserId(Guid userId) => _libraryRepository.GetBooksByUserId(userId);
}

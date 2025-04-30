using SmartBook.Core.DTOs;
using SmartBook.Core.Entities;

namespace SmartBook.Core.Interfaces;
public interface ILibraryRepository
{
    void AddBook(Book book);
    void RemoveBook(string isbn);
    void UpdateBookStatus(string isbn, Guid userId);
    Book? GetBookByISBN(string isbn);
    IEnumerable<Book> SearchBooks(string searchTerm);
    IEnumerable<Book> GetAllBooks();
    void AddUser(User user);
    IEnumerable<User> GetAllUsers();
    User? GetUserById(Guid id);
    IEnumerable<UserWithBooksDto> GetAllUsersWithBooks();
    IEnumerable<Book> GetBooksByUserId(Guid userId);
}

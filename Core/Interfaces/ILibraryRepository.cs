using SmartBook.Core.Entities;

namespace SmartBook.Core.Interfaces;
public interface ILibraryRepository
{
    void AddBook(Book book);
    void RemoveBook(string isbn);
    void UpdateBookStatus(string isbn, bool isBorrowed);
    Book? GetBookByISBN(string isbn);
    IEnumerable<Book> SearchBooks(string searchTerm);
    IEnumerable<Book> GetAllBooks();
}

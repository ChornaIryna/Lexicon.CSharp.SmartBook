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

    public void UpdateBookStatus(string isbn, bool isBorrowed)
    {
        _ = _libraryRepository.GetBookByISBN(isbn) ?? throw new BookNotFoundException(isbn);
        _libraryRepository.UpdateBookStatus(isbn, isBorrowed);
    }

}

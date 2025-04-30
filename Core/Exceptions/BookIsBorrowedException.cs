using SmartBook.Core.Entities;

namespace SmartBook.Core.Exceptions;
public class BookIsBorrowedException : Exception
{
    public BookIsBorrowedException(Book book) : base($"Book with ISBN '{book.ISBN}' is borrowed.")
    { }
}

namespace SmartBook.Core.Exceptions;
public class BookNotFoundException : Exception
{
    public BookNotFoundException(string isbn)
        : base($"Book with ISBN '{isbn}' not found.")
    {
    }
}

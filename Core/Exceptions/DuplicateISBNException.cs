namespace SmartBook.Core.Exceptions;
public class DuplicateISBNException : Exception
{
    public DuplicateISBNException(string isbn)
        : base($"A book with ISBN '{isbn}' already exists. Please check the ISBN and try again.")
    {
    }
}

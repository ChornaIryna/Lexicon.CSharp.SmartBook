namespace SmartBook.Core.Exceptions;
public class DuplicateSIBNException : Exception
{
    public DuplicateSIBNException(string isbn)
        : base($"A book with ISBN '{isbn}' already exists. Please check the ISBN and try again.")
    {
    }
}

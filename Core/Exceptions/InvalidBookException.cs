namespace SmartBook.Core.Exceptions;
public class InvalidBookException : Exception
{
    public InvalidBookException(string message)
        : base($"The book is invalid. Please check the details. {message}")
    { }
}

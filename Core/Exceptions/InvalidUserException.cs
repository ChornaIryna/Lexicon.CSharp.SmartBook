namespace SmartBook.Core.Exceptions;
public class InvalidUserException : Exception
{
    public InvalidUserException(string message)
        : base($"The user 'Name' is invalid. {message}")
    { }
}

namespace SmartBook.Core.Exceptions;
public class EmptyCollectionException : Exception
{
    public EmptyCollectionException(string message)
        : base($"Collection is empty. {message}")
    { }
}

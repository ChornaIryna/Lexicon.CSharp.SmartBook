namespace SmartBook.Core.Exceptions;
public class UserNotFoundException : Exception
{
    public UserNotFoundException(string userId)
        : base($"User with user ID '{userId}' not found.")
    { }
}

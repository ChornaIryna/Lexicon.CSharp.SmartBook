using SmartBook.Core.Entities;

namespace SmartBook.Core.Exceptions;
public class UserAlreadyExistsException : Exception
{
    public UserAlreadyExistsException(User user)
        : base($"User '{user.Name}' with ID '{user.Id}' already exists.")
    { }
}

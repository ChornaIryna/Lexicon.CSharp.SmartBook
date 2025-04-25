using SmartBook.Core.Entities;

namespace SmartBook.Core.Interfaces;
internal interface IValidation
{
    bool IsValid(Book book);
    bool IsValid(string text);
    bool IsValidISBN(Guid guid);
}

using SmartBook.Core.Entities;
using System.Text;

namespace SmartBook.Core.DTOs;
public class UserWithBooksDto
{

    public Guid UserId { get; set; }
    public string? UserName { get; set; }
    public List<Book>? BorrowedBooks { get; set; }

    public override string ToString()
    {
        StringBuilder? borrowedBooksBuilder = new();
        if (BorrowedBooks != null && BorrowedBooks.Count > 0)
        {
            borrowedBooksBuilder.AppendLine();
            foreach (var book in BorrowedBooks)
            {
                borrowedBooksBuilder.AppendLine($"|- {book}");
            }
        }
        else
            borrowedBooksBuilder.Append($"No borrowed books.");

        return $"""
             ____________________________________
            | User: {UserName}
            |   Id: {UserId}
            |Books: {borrowedBooksBuilder}
            """;
    }
}

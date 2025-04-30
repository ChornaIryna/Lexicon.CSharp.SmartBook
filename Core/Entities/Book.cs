using SmartBook.Core.Interfaces;

namespace SmartBook.Core.Entities;
public class Book : IValidation
{
    public string Title { get; set; }
    public string Author { get; set; }
    public string ISBN { get; set; }
    public string Category { get; set; }
    public bool IsBorrowed { get; set; }
    public Guid? BorrowedBy { get; set; }

    public Book()
    { }

    public Book(string title, string author, string isbn, string category)
    {
        Title = title;
        Author = author;
        ISBN = isbn;
        Category = category;
        IsBorrowed = false;
        BorrowedBy = null;
    }

    public override string ToString() => $" '{Title}' by {Author} | ISBN: {ISBN} | Category: {Category} | Status: {(IsBorrowed ? "Borrowed" : "Available")}";

    public (bool valid, string? validationResult) IsValid()
    {
        if (string.IsNullOrWhiteSpace(Title))
            return (false, "Title is required.");
        if (string.IsNullOrWhiteSpace(Author))
            return (false, "Author is required.");
        if (string.IsNullOrWhiteSpace(ISBN))
            return (false, "ISBN is required.");
        if (string.IsNullOrWhiteSpace(Category))
            return (false, "Category is required.");
        return (true, null);
    }
}

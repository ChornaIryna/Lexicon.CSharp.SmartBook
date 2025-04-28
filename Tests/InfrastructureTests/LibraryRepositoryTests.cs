using SmartBook.Core.Entities;
using SmartBook.Infrastructure.Repositories;

namespace SmartBook.Tests.InfrastructureTests;
public class LibraryRepositoryTests
{
    private readonly string testFilePath = "test_library.json";

    public LibraryRepositoryTests()
    {
        // Ensure a clean state for each test run.
        if (File.Exists(testFilePath))
            File.Delete(testFilePath);
    }

    [Fact]
    public void AddBook_ShouldPersistBook()
    {
        var repo = new LibraryRepository(testFilePath);
        var book = new Book("Test", "Author", "ISBN123", "Category");

        repo.AddBook(book);
        var allBooks = repo.GetAllBooks().ToList();

        Assert.Single(allBooks);
        Assert.Equal("Test", allBooks.First().Title);
    }

    [Fact]
    public void RemoveBook_ShouldDeleteBookFromRepository()
    {
        var repo = new LibraryRepository(testFilePath);
        var book = new Book("Test", "Author", "ISBN123", "Category");

        repo.AddBook(book);
        repo.RemoveBook("ISBN123");

        Assert.Empty(repo.GetAllBooks());
    }
}

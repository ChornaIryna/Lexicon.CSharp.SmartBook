using SmartBook.Core.Entities;
using SmartBook.Core.Exceptions;
using SmartBook.Infrastructure.Repositories;

namespace SmartBook.Tests.InfrastructureTests;
public class LibraryRepositoryTests
{
    private readonly string testFilePath = Path.Combine(Directory.GetCurrentDirectory(), "TestData");
    public LibraryRepositoryTests()
    {
        if (!Directory.Exists(testFilePath))
            Directory.CreateDirectory(testFilePath);
        if (File.Exists(Path.Combine(testFilePath, "books.json")))
            File.Delete(Path.Combine(testFilePath, "books.json"));
        if (File.Exists(Path.Combine(testFilePath, "users.json")))
            File.Delete(Path.Combine(testFilePath, "users.json"));
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
    public void AddBook_ShouldThrowException_WhenBookIsInvalid()
    {
        var repo = new LibraryRepository(testFilePath);
        var book = new Book("", "Author", "ISBN123", "Category");
        Assert.Throws<InvalidBookException>(() => repo.AddBook(book));
    }

    [Fact]
    public void GetAllBooks_ShouldReturnAllBooks()
    {
        var repo = new LibraryRepository(testFilePath);
        var book1 = new Book("Test1", "Author1", "ISBN123", "Category1");
        var book2 = new Book("Test2", "Author2", "ISBN456", "Category2");
        repo.AddBook(book1);
        repo.AddBook(book2);
        var allBooks = repo.GetAllBooks().ToList();
        Assert.Equal(2, allBooks.Count);
        Assert.Contains(allBooks, b => b.Title == "Test1");
        Assert.Contains(allBooks, b => b.Title == "Test2");
    }


    [Fact]
    public void RemoveBook_ShouldDeleteBookFromRepository()
    {
        var repo = new LibraryRepository(testFilePath);
        var book1 = new Book("Test1", "Author1", "ISBN123", "Category");
        var book2 = new Book("Test2", "Author2", "ISBN234", "Category");

        repo.AddBook(book1);
        repo.AddBook(book2);
        repo.RemoveBook("ISBN123");

        Assert.DoesNotContain(repo.GetAllBooks().ToList(), b => b.ISBN == "ISBN123");
    }

    [Fact]
    public void RemoveBook_ShouldThrowException_WhenBookNotFound()
    {
        var repo = new LibraryRepository(testFilePath);
        repo.AddBook(new Book("Test", "Author", "ISBN123", "Category"));
        Assert.Throws<BookNotFoundException>(() => repo.RemoveBook("ISBN1"));
    }

    [Fact]
    public void RemoveBook_ShouldThrowException_WhenBooksCollectionIsEmpty()
    {
        var repo = new LibraryRepository(testFilePath);
        Assert.Throws<EmptyCollectionException>(() => repo.RemoveBook("ISBN1"));
    }

    [Fact]
    public void UpdateBookStatus_ShouldUpdateBookStatusToBorrowed_IfBookIsAvailable()
    {
        var repo = new LibraryRepository(testFilePath);
        var book = new Book("Test", "Author", "ISBN123", "Category");
        var user = new User("User One");
        repo.AddBook(book);
        repo.AddUser(user);
        repo.UpdateBookStatus("ISBN123", user.Id);
        var updatedBook = repo.GetBookByISBN("ISBN123");
        Assert.Equal(user.Id, updatedBook?.BorrowedBy);
    }

    [Fact]
    public void UpdateBookStatus_ShouldUpdateBookStatusToAvailable_IfBookIsBorrowedByUser()
    {
        var repo = new LibraryRepository(testFilePath);
        var book = new Book("Test", "Author", "ISBN123", "Category");
        var user = new User("User One");
        repo.AddBook(book);
        repo.AddUser(user);
        repo.UpdateBookStatus("ISBN123", user.Id);
        repo.UpdateBookStatus("ISBN123", user.Id);
        var updatedBook = repo.GetBookByISBN("ISBN123");
        Assert.Null(updatedBook?.BorrowedBy);
        Assert.Equal(false, updatedBook?.IsBorrowed);
    }

    [Fact]
    public void UpdateBookStatus_ShouldThrowException_WhenBookNotFound()
    {
        var repo = new LibraryRepository(testFilePath);
        var user = new User("User One");
        var book = new Book("Test", "Author", "ISBN123", "Category");
        repo.AddUser(user);
        repo.AddBook(book);
        Assert.Throws<BookNotFoundException>(() => repo.UpdateBookStatus("ISBN1", user.Id));
    }

    [Fact]
    public void UpdateBookStatus_ShouldThrowException_WhenUserNotFound()
    {
        var repo = new LibraryRepository(testFilePath);
        var book = new Book("Test", "Author", "ISBN123", "Category");
        var user = new User("User One");
        repo.AddBook(book);
        repo.AddUser(user);
        Assert.Throws<UserNotFoundException>(() => repo.UpdateBookStatus("ISBN123", Guid.NewGuid()));
    }

    [Fact]
    public void UpdateBookStatus_ShouldThrowException_WhenBookIsAlreadyBorrowed()
    {
        var repo = new LibraryRepository(testFilePath);
        var book = new Book("Test", "Author", "ISBN123", "Category");
        var user1 = new User("User One");
        var user2 = new User("User Two");
        repo.AddBook(book);
        repo.AddUser(user1);
        repo.AddUser(user2);
        repo.UpdateBookStatus("ISBN123", user1.Id);
        Assert.Throws<BookIsBorrowedException>(() => repo.UpdateBookStatus("ISBN123", user2.Id));
    }

    [Fact]
    public void SearchBooks_ShouldReturnMatchingBooks()
    {
        var repo = new LibraryRepository(testFilePath);
        var book1 = new Book("Test1", "Author1", "ISBN123", "Category1");
        var book2 = new Book("Test2", "Author2", "ISBN456", "Category2");
        repo.AddBook(book1);
        repo.AddBook(book2);
        var searchResults = repo.SearchBooks("Test").ToList();
        Assert.Equal(2, searchResults.Count);
        Assert.Contains(searchResults, b => b.Title == "Test1");
        Assert.Contains(searchResults, b => b.Title == "Test2");
    }

    [Fact]
    public void SearchBooks_ShouldReturnEmptyList_WhenNoMatchingBooks()
    {
        var repo = new LibraryRepository(testFilePath);
        var book1 = new Book("Test1", "Author1", "ISBN123", "Category1");
        var book2 = new Book("Test2", "Author2", "ISBN456", "Category2");
        repo.AddBook(book1);
        repo.AddBook(book2);
        var searchResults = repo.SearchBooks("NonExistent").ToList();
        Assert.Empty(searchResults);
    }

    [Fact]
    public void AddUser_ShouldPersistUser()
    {
        var repo = new LibraryRepository(testFilePath);
        var user = new User("Test User");
        repo.AddUser(user);
        var allUsers = repo.GetAllUsers().ToList();
        Assert.Single(allUsers);
        Assert.Equal("Test User", allUsers.First().Name);
    }

    [Fact]
    public void AddUser_ShouldThrowException_WhenUserIsInvalid()
    {
        var repo = new LibraryRepository(testFilePath);
        var user = new User("");
        Assert.Throws<InvalidUserException>(() => repo.AddUser(user));
    }

    [Fact]
    public void AddUser_ShouldThrowException_WhenUserNameContainsNumber()
    {
        var repo = new LibraryRepository(testFilePath);
        var user = new User("User 1");
        Assert.Throws<InvalidUserException>(() => repo.AddUser(user));
    }

    [Fact]
    public void AddUser_ShouldThrowException_WhenUserAlreadyExists()
    {
        var repo = new LibraryRepository(testFilePath);
        var user = new User("Test User");
        repo.AddUser(user);
        Assert.Throws<UserAlreadyExistsException>(() => repo.AddUser(user));
    }

    [Fact]
    public void GetAllUsers_ShouldReturnAllUsers()
    {
        var repo = new LibraryRepository(testFilePath);
        var user1 = new User("User One");
        var user2 = new User("User Two");
        repo.AddUser(user1);
        repo.AddUser(user2);
        var allUsers = repo.GetAllUsers().ToList();
        Assert.Equal(2, allUsers.Count);
        Assert.Contains(allUsers, u => u.Name == "User One");
        Assert.Contains(allUsers, u => u.Name == "User Two");
    }
}

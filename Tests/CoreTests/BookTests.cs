using SmartBook.Core.Entities;

namespace SmartBook.Tests.CoreTests;
public class BookTests
{
    [Fact]
    public void NewBook_ShouldHaveCorrectProperties()
    {
        var book = new Book("Test Title", "Test Author", "12345", "Test Category");

        Assert.Equal("Test Title", book.Title);
        Assert.Equal("Test Author", book.Author);
        Assert.Equal("12345", book.ISBN);
        Assert.Equal("Test Category", book.Category);
        Assert.False(book.IsBorrowed);
    }

    [Fact]
    public void IsValid_ShouldReturnTrue_WhenAllPropertiesAreValid()
    {
        var book = new Book("Test Title", "Test Author", "12345", "Test Category");
        Assert.True(book.IsValid().valid);
    }

    [Fact]
    public void IsValid_ShouldReturnFalse_WhenTitleIsEmpty()
    {
        var book = new Book("", "Test Author", "12345", "Test Category");
        Assert.False(book.IsValid().valid);
    }
}

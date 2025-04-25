using SmartBook.ConsoleUI.Helpers;
using SmartBook.Core.Entities;
using SmartBook.Core.Services;

namespace SmartBook.ConsoleUI.UI;
internal class UserInterface(LibraryService libraryService)
{
    private static bool _isRunning = true;
    private readonly Dictionary<int, Action> _menuActions = [];
    private readonly LibraryService _libraryService = libraryService;

    internal void Run()
    {
        InitializeMenuActions();
        while (_isRunning)
        {
            ShowMenu();
            HandleUserChoice(GetUserChoice());
        }
    }

    private void InitializeMenuActions()
    {
        _menuActions.Clear();
        _menuActions.Add((int)MenuOptions.AddBook, AddBook);
        _menuActions.Add((int)MenuOptions.RemoveBook, RemoveBook);
        _menuActions.Add((int)MenuOptions.ListAllBooks, ListAllBooks);
        _menuActions.Add((int)MenuOptions.SearchBooks, SearchBooks);
        _menuActions.Add((int)MenuOptions.ChangeBookStatus, ChangeBookStatus);
    }

    private static void ShowMenu()
    {
        ConsoleHelper.ClearConsole();
        Console.WriteLine($"""
        |__ Main Menu __|
        
        {(int)MenuOptions.AddBook}. Add Book
        {(int)MenuOptions.RemoveBook}. Remove Book
        {(int)MenuOptions.ListAllBooks}. List All Books
        {(int)MenuOptions.SearchBooks}. Search Books
        {(int)MenuOptions.ChangeBookStatus}. Mark Book Borrowed/Available
        {(int)MenuOptions.Exit}. Exit
        """);
    }

    private void HandleUserChoice(int choice)
    {
        if (choice == (int)MenuOptions.Exit)
        {
            _isRunning = false;
            return;
        }

        if (_menuActions.TryGetValue(choice, out Action? action))
        {
            try
            {
                action.Invoke();
            }
            catch (Exception ex)
            {
                ConsoleHelper.PrintError(ex.Message);
                ConsoleHelper.HandleReturn();
            }
        }
        else
        {
            ConsoleHelper.PrintWarning($"Invalid choice. Please try again. Your input should be a number between 0 and {_menuActions.Count} ");
            ConsoleHelper.HandleReturn();
        }
    }

    private int GetUserChoice()
    {
        Console.Write("Enter your choice: ");
        string? input = Console.ReadLine()?.Trim();
        if (int.TryParse(input, out int choice) && Enum.IsDefined(typeof(MenuOptions), choice))
            return choice;
        else
        {
            ConsoleHelper.PrintWarning($"Invalid choice. Please try again. Your input should be a number between 0 and {_menuActions.Count} ");
            return GetUserChoice();
        }
    }

    private void AddBook()
    {
        ConsoleHelper.ClearConsole();
        ConsoleHelper.WriteActionTitle("|__ Add New Book __|", "Enter the book information:");
        Console.Write(" - Title: ");
        string? title = Console.ReadLine()?.Trim();
        Console.Write(" - Author: ");
        string? author = Console.ReadLine()?.Trim();
        Console.Write(" - ISBN: ");
        string? isbn = Console.ReadLine()?.Trim();
        Console.Write(" - Category: ");
        string? category = Console.ReadLine()?.Trim();

        var book = new Book(title, author, isbn, category);
        if (book.IsValid())
        {
            try
            {
                _libraryService.AddBook(book);
                ConsoleHelper.PrintSuccess($"Book '{title}' by {author} added successfully.");
            }
            catch (Exception ex)
            {
                ConsoleHelper.PrintError(ex.Message);
            }
        }
        else
            ConsoleHelper.PrintWarning("All fields are required. Please try again.");
        ConsoleHelper.HandleReturn();
    }

    private void RemoveBook()
    {
        ConsoleHelper.ClearConsole();
        ConsoleHelper.WriteActionTitle("|__ Remove a Book __|", "Enter the ISBN of the book to remove:");
        Console.Write(" - ISBN: ");
        string? isbn = Console.ReadLine()?.Trim();
        try
        {
            _libraryService.RemoveBook(isbn);
            ConsoleHelper.PrintSuccess($"Book with ISBN '{isbn}' removed successfully.");
        }
        catch (Exception ex)
        {
            ConsoleHelper.PrintError(ex.Message);
        }
        ConsoleHelper.HandleReturn();
    }

    private void ListAllBooks()
    {
        ConsoleHelper.ClearConsole();
        ConsoleHelper.WriteActionTitle("|__ List All Books __|", "Here are all the books in the library:");
        var books = _libraryService.GetAllBooks();
        if (books.Any())
        {
            foreach (var book in books)
                Console.WriteLine(book);
        }
        else
            ConsoleHelper.PrintWarning("No books found.");
        ConsoleHelper.HandleReturn();
    }

    private void SearchBooks()
    {
        ConsoleHelper.ClearConsole();
        ConsoleHelper.WriteActionTitle("|__ Search Books __|", "Enter the search term (title, author, or ISBN):");
        Console.Write(" - Search Term: ");
        string? searchTerm = Console.ReadLine()?.Trim();
        var books = _libraryService.SearchBooks(searchTerm);
        ConsoleHelper.WriteActionTitle("=== Search Results ===", $"Found {books.Count()} book(s):");
        if (books.Any())
        {
            foreach (var book in books)
                Console.WriteLine(book);
        }
        else
            ConsoleHelper.PrintWarning("No books found.");
        ConsoleHelper.HandleReturn();
    }

    private void ChangeBookStatus()
    {
        ConsoleHelper.ClearConsole();
        ConsoleHelper.WriteActionTitle("|__ Change Book Status __|", "Enter the ISBN of the book to change its status:");
        Console.Write(" - ISBN: ");
        string? isbn = Console.ReadLine()?.Trim();
        Console.Write(" - Status (if borrowed type '0' | if available type '1' ) : ");
        string? statusInput = Console.ReadLine()?.Trim();
        if (int.TryParse(statusInput, out int status) && (status == 0 || status == 1))
        {
            try
            {
                _libraryService.UpdateBookStatus(isbn, status == 0);
                ConsoleHelper.PrintSuccess($"Book with ISBN '{isbn}' status updated successfully.");
            }
            catch (Exception ex)
            {
                ConsoleHelper.PrintError(ex.Message);
            }
        }
        else
            ConsoleHelper.PrintWarning("Invalid status. Please enter '0' for borrowed or '1' for available.");
        ConsoleHelper.HandleReturn();
    }
}

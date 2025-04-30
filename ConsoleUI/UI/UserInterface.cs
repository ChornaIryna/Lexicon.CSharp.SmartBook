using SmartBook.ConsoleUI.Helpers;
using SmartBook.Core.Entities;
using SmartBook.Core.Services;
using System.Text;

namespace SmartBook.ConsoleUI.UI;

/// <summary>
/// This class handles the user interface for the library system.
/// </summary>
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

    #region Menu Handling
    private void InitializeMenuActions()
    {
        _menuActions.Clear();
        _menuActions.Add((int)MenuOptions.AddBook, AddBook);
        _menuActions.Add((int)MenuOptions.RemoveBook, RemoveBook);
        _menuActions.Add((int)MenuOptions.ListAllBooks, ListAllBooks);
        _menuActions.Add((int)MenuOptions.SearchBooks, SearchBooks);
        _menuActions.Add((int)MenuOptions.ChangeBookStatus, ChangeBookStatus);
        _menuActions.Add((int)MenuOptions.AddUser, AddUser);
        _menuActions.Add((int)MenuOptions.ListAllUsers, ListAllUsers);
        _menuActions.Add((int)MenuOptions.ListBorrowedBooksByUser, ListBorrowedBooksByUser);
        _menuActions.Add((int)MenuOptions.ListAllBorrowedBooks, ListAllBorrowedBooks);
        _menuActions.Add((int)MenuOptions.Exit, ExitApplication);
    }

    private static void ShowMenu()
    {
        ConsoleHelper.ClearConsole();
        Console.WriteLine($"""
        |__ Main Menu __|
        
        {(int)MenuOptions.AddBook}. Add New Book
        {(int)MenuOptions.RemoveBook}. Remove Book
        {(int)MenuOptions.ListAllBooks}. List All Books
        {(int)MenuOptions.SearchBooks}. Search Books
        {(int)MenuOptions.ChangeBookStatus}. Borrow Or Return A Book
        {(int)MenuOptions.AddUser}. Add New User
        {(int)MenuOptions.ListAllUsers}. List All Users
        {(int)MenuOptions.ListBorrowedBooksByUser}. List Books Borrowed By A User
        {(int)MenuOptions.ListAllBorrowedBooks}. List All Borrowed Books
        {(int)MenuOptions.Exit}. {MenuOptions.Exit}
        """);
    }

    private void HandleUserChoice(int choice)
    {
        if (_menuActions.TryGetValue(choice, out Action? action))
        {
            try
            {
                action.Invoke();
            }
            catch (Exception ex)
            {
                ConsoleHelper.PrintError(ex.Message);
            }
        }
        else
            ConsoleHelper.PrintWarning($"Invalid choice. Please try again.");
    }

    private int GetUserChoice()
    {
        while (true)
        {
            string? input = ConsoleHelper.GetInput("Enter your choice: ");
            if (int.TryParse(input, out int choice) && _menuActions.ContainsKey(choice))
                return choice;
            ConsoleHelper.PrintWarning($"Invalid choice. Please try again. Your input should be a number between 0 and {_menuActions.Count - 1} ", false);
        }
    }
    #endregion Menu 

    #region Book Management
    internal void AddBook()
    {
        ConsoleHelper.ClearConsole();
        ConsoleHelper.PrintActionTitle("|__ Add New Book __|", "Enter the book information:");
        string title = GetValidatedInput(" - Title: ");
        string author = GetValidatedInput(" - Author: ");
        string isbn = GetValidatedInput(" - ISBN: ");
        string category = GetValidatedInput(" - Category: ");

        var book = new Book(title, author, isbn, category);
        var (valid, message) = book.IsValid();
        if (!valid)
            ConsoleHelper.PrintWarning($"All fields are required. {message}");
        try
        {
            _libraryService.AddBook(book);
            ConsoleHelper.PrintSuccess($"Book '{book.Title}' by {book.Author} added successfully.");
        }
        catch (Exception ex)
        {
            ConsoleHelper.PrintError(ex.Message);
        }
    }

    private void RemoveBook()
    {
        ConsoleHelper.ClearConsole();
        ConsoleHelper.PrintActionTitle("|__ Remove Book __|", "Enter the ISBN of the book to remove:");
        string? isbn = GetValidatedInput(" - ISBN: ");
        try
        {
            _libraryService.RemoveBook(isbn);
            ConsoleHelper.PrintSuccess($"Book with ISBN '{isbn}' removed successfully.");
        }
        catch (Exception ex)
        {
            ConsoleHelper.PrintError(ex.Message);
        }
    }

    private void ListAllBooks()
    {
        ConsoleHelper.ClearConsole();
        var books = _libraryService.GetAllBooks();
        var subtitle = books.Any() ? $"Found {books.Count()} book(s):" : "No books found.";
        ConsoleHelper.PrintActionTitle("|__ List All Books __|", subtitle);
        if (books.Any())
            PrintInfoList(books);
    }

    private void SearchBooks()
    {
        ConsoleHelper.ClearConsole();
        ConsoleHelper.PrintActionTitle("|__ Search Books __|", "Enter the search term (title, author, or ISBN):");
        string searchTerm = GetValidatedInput(" - Search Term: ");
        var books = _libraryService.SearchBooks(searchTerm);
        if (books.Any())
        {
            ConsoleHelper.PrintActionTitle("=== Search Results ===", $"Found {books.Count()} book(s):");
            PrintInfoList(books);
        }
        else
            ConsoleHelper.PrintWarning("No books was found.");
    }

    private void ChangeBookStatus()
    {
        ConsoleHelper.ClearConsole();
        ConsoleHelper.PrintActionTitle("|__ Borrow Or Return A Book __|", "Enter the book's ISBN and the user ID (GUID):");
        string isbn = GetValidatedInput(" - ISBN: ");
        string userIdInput = GetValidatedInput(" - User ID (GUID): ");
        if (Guid.TryParse(userIdInput, out Guid userId))
        {
            try
            {
                _libraryService.UpdateBookStatus(isbn, userId);
                ConsoleHelper.PrintSuccess($"Book with ISBN '{isbn}' status updated successfully.");
            }
            catch (Exception ex)
            {
                ConsoleHelper.PrintError(ex.Message);
            }
        }
        else
            ConsoleHelper.PrintWarning("Invalid User ID. Please enter a valid GUID.");

    }

    private void ListAllBorrowedBooks()
    {
        ConsoleHelper.ClearConsole();
        var usersWithBooks = _libraryService.GetAllUsersWithBorrowedBooks();
        var subtitle = usersWithBooks.Any() ? $"Found {usersWithBooks.Count()} user(s) with borrowed books:" : "No borrowed books found.";
        ConsoleHelper.PrintActionTitle("|__ List All Borrowed Books __|", subtitle);

        if (usersWithBooks.Any())
        {
            PrintInfoList(usersWithBooks, false);
            ExportToFile(usersWithBooks);
        }
    }

    private static void ExportToFile(IEnumerable<object>? items)
    {
        if (items == null || !items.Any())
            return;
        var exportChoice = GetValidatedInput("Do you want to export the items to a file? (y/n): ");
        if (exportChoice?.ToLower() != "y")
        {
            ConsoleHelper.PrintWarning("Export cancelled.");
            return;
        }

        const string fileName = "report.txt";
        try
        {
            using StreamWriter writer = new(fileName, false);
            foreach (var item in items)
            {
                writer.WriteLine($"{item}");
                writer.WriteLine();
            }
            ConsoleHelper.PrintSuccess($"Items successfully exported to '{fileName}'.");
        }
        catch (Exception ex)
        {
            ConsoleHelper.PrintError($"Failed to export items: {ex.Message}");
        }
    }

    #endregion Book Management

    #region User Management
    private void AddUser()
    {
        ConsoleHelper.ClearConsole();
        ConsoleHelper.PrintActionTitle("|__ Add New User __|", "Enter the user information:");
        string name = GetValidatedInput(" - Name: ");
        var user = new User(name);
        try
        {
            _libraryService.AddUser(user);
            ConsoleHelper.PrintSuccess($"User '{name}' added successfully.");
        }
        catch (Exception ex)
        {
            ConsoleHelper.PrintError(ex.Message);
        }
    }

    private void ListAllUsers()
    {
        ConsoleHelper.ClearConsole();
        var users = _libraryService.GetAllUsers();
        var subtitle = users.Any() ? $"Found {users.Count()} user(s):" : "No users found.";
        ConsoleHelper.PrintActionTitle("|__ List All Users __|", subtitle);
        if (users.Any())
            PrintInfoList(users);
    }

    private void ListBorrowedBooksByUser()
    {
        ConsoleHelper.ClearConsole();
        string? userIdInput = GetValidatedInput("Enter User ID (GUID): ");
        if (!Guid.TryParse(userIdInput, out Guid userId))
        {
            ConsoleHelper.PrintWarning("Invalid User ID. Please enter a valid GUID.");
            return;
        }
        try
        {
            var user = _libraryService.GetUserById(userId);
            IEnumerable<Book> booksBorrowedByUser = _libraryService.GetBooksByUserId(user.Id);
            var subtitle = booksBorrowedByUser.Any() ? $"Found {booksBorrowedByUser.Count()} book(s) borrowed by {user.Name}:" : $"No books found for user {user.Name}.";
            ConsoleHelper.ClearConsole();
            ConsoleHelper.PrintActionTitle($"|__ List Books Borrowed By A User __|", subtitle);
            if (booksBorrowedByUser.Any())
                PrintInfoList(booksBorrowedByUser);

        }
        catch (Exception ex)
        {
            ConsoleHelper.PrintError(ex.Message);
        }
    }
    #endregion User Management


    private void ExitApplication()
    {
        ConsoleHelper.ClearConsole();
        ConsoleHelper.PrintActionTitle("|__ Exiting the application...__|", "Goodbye!");
        _isRunning = false;
    }

    private static void PrintInfoList(IEnumerable<object>? items, bool showReturnPrompt = true)
    {
        if (items == null || !items.Any())
        {
            ConsoleHelper.PrintWarning("No items to display.");
            return;
        }

        StringBuilder stringBuilder = new();
        foreach (var item in items)
            stringBuilder.AppendLine(item?.ToString() ?? "Unknown item");

        ConsoleHelper.PrintInfo(stringBuilder.ToString(), showReturnPrompt);
    }

    private static string GetValidatedInput(string prompt)
    {
        while (true)
        {
            string? input = ConsoleHelper.GetInput(prompt);
            if (!string.IsNullOrWhiteSpace(input))
                return input;
            ConsoleHelper.PrintWarning("Input cannot be empty. Please try again.", false);
        }
    }
}

using SmartBook.ConsoleUI.UI;
using SmartBook.Core.Interfaces;
using SmartBook.Core.Services;
using SmartBook.Infrastructure.Repositories;

namespace SmartBook.ConsoleUI;

internal class Program
{
    static void Main(string[] args)
    {
        string jsonFilePath = Path.Combine(AppContext.BaseDirectory);
        ILibraryRepository repository = new LibraryRepository(jsonFilePath);
        LibraryService libraryService = new(repository);
        UserInterface ui = new(libraryService);
        ui.Run();
    }
}

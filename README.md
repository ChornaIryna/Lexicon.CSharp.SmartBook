# SmartBook Library

A simple console-based library system built with C#. It demonstrates Clean Architecture, SOLID & DRY principles, along with usage of interfaces, inheritance, and unit testing with xUnit.

---

## Installation

1. Clone the repository: https://github.com/ChornaIryna/Lexicon.CSharp.SmartBook

2. Open the solution file `*.sln` in *Visual Studio **2022***.

3. Restore NuGet packages if prompted.

4. Build the solution to ensure everything is set up correctly.

---

## Running the Application

1. Open `*.sln` in Visual Studio.

2. Set the startup project to `SmartBook.ConsoleUI`.

3. Run the project using `Ctrl + F5` or by clicking the "Start" button in Visual Studio.

4. Follow the menu options in the console to interact with the library system.


---

## Running Tests

1. Open the Test Explorer in Visual Studio:
   - Navigate to __Test > Test Explorer__.
2. Run all tests or specific test cases.


---

## Project Structure

- **SmartBook.Core**: Domain layer containing entities, interfaces, services, and custom exceptions.
- **SmartBook.Infrastructure**: Data access and JSON file handling.
- **SmartBook.ConsoleUI**: Presentation layer with a menu-driven console interface.
- **SmartBook.Tests**: Unit tests for the domain, services, and infrastructure.


			SmartBookSolution/
			├── SmartBook.sln                        
			│  
			├─ SmartBook.ConsoleUI/                          // Presentation layer 
			│  ├── Program.cs                                // Entry point of the application
			│  ├── Helpers/
			│  │   ├── ConsoleHelper.cs                      
			│  └── UI/
			│      ├── MenuOptions.cs                        // Enum for menu options
			│      └── UserInterface.cs                      // Console-based user interface
			│              
			│
			├─ SmartBook.Core/                               // Domain layer 
			│  ├── DTOs/
			│  │   ├── UserWithBooksDto.cs                       
			│  ├── Entities/
			│  │   ├── Book.cs
			│  │   ├── User.cs                       
			│  ├── Exceptions/
			│  │   ├── BookIsBorrowedException.cs     
			│  │   ├── BookNotFoundException.cs      
			│  │   ├── DuplicateISBNException.cs      
			│  │   ├── InvalidBookException.cs      
			│  │   ├── InvalidUserException.cs      
			│  │   ├── UserAlreadyExistsException.cs      
			│  │   ├── UserNotFoundException.cs      
			│  ├── Interfaces/
			│  │   ├── ILibraryRepository.cs         
			│  │   ├── IValidation.cs                
			│  └── Services/
			│      └── LibraryService.cs             
			│                     
			│  
			├─ SmartBook.Infrastructure/                      // Infrastructure layer 
			│  ├── Data/
			│  │   ├── JsonFileStorage.cs                
			│  └── Repositories/
			│      └── LibraryRepository.cs          
			│  
			│
			├──SmartBook.Tests/                               // Test project
			│  ├── ConsoleUITests/
			│  │   ├── ConsoleHelperTests.cs         
			│  ├── CoreTests/
			│  │   ├── BookTests.cs                  
			│  ├── InfrastructureTests/
			│  │   ├── LibraryRepositoryTests.cs     
			│  └── ServiceTests/
			│      └── LibraryServiceTests.cs        
			│
			└── README.md                                    // Documentation



---

## Test Coverage Summary

- **Core Layer**:
  - `BookTests` validate entity behavior and validation logic.
- **Service Layer**:
  - `LibraryServiceTests` ensure business logic correctness, including adding/removing books and managing users.
- **Infrastructure Layer**:
  - `LibraryRepositoryTests` verify data access and JSON file handling.
- **Helpers**:
  - `ConsoleHelperTests` test reusable console I/O methods.

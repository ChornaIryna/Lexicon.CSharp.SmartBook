using SmartBook.Core.Interfaces;

namespace SmartBook.Core.Entities;
public class User(string name) : IValidation
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = name;

    public (bool valid, string? validationResult) IsValid()
    {
        char[] inappropriateChars = { '@', '#', '$', '%', '^', '&', '*', '!', '¤', '/', '=', '?' };

        if (string.IsNullOrWhiteSpace(Name))
            return (false, "Name is required.");

        if (Name.IndexOfAny(inappropriateChars) != -1)
            return (false, "Name contains inappropriate characters.");

        if (Name.Length > 50)
            return (false, "Name exceeds the maximum length of 50 characters.");

        if (Name.Any(char.IsDigit))
            return (false, "Name cannot contain numeric digits.");

        if (Name.StartsWith(' ') || Name.EndsWith(' '))
            return (false, "Name cannot start or end with a space.");

        return (true, Name);
    }

    public override string ToString() => $"""  
       _____________________________________________
       | {Name}   
       | Id: {Id}  
       """;
}

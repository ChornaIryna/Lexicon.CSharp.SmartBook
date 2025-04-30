namespace SmartBook.Core.Interfaces;
internal interface IValidation
{
    (bool valid, string? validationResult) IsValid();
}

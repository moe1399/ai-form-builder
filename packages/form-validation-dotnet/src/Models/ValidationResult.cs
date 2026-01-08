namespace DynamicForms.FormValidation.Models;

/// <summary>
/// Field validation error
/// </summary>
public class FieldValidationError
{
    /// <summary>
    /// Field name or path (e.g., "email" or "contacts[0].phone")
    /// </summary>
    public string Field { get; set; } = string.Empty;

    /// <summary>
    /// Error message
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// The validation rule type that failed
    /// </summary>
    public string Rule { get; set; } = string.Empty;
}

/// <summary>
/// Result of form validation
/// </summary>
public class ValidationResult
{
    /// <summary>
    /// Whether the form is valid
    /// </summary>
    public bool Valid { get; set; }

    /// <summary>
    /// List of validation errors
    /// </summary>
    public List<FieldValidationError> Errors { get; set; } = new();

    /// <summary>
    /// Create a valid result
    /// </summary>
    public static ValidationResult Success() => new() { Valid = true };

    /// <summary>
    /// Create an invalid result with errors
    /// </summary>
    public static ValidationResult Failure(List<FieldValidationError> errors) =>
        new() { Valid = false, Errors = errors };
}

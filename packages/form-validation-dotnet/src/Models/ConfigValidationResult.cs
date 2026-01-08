namespace DynamicForms.FormValidation.Models;

/// <summary>
/// Config validation error
/// </summary>
public class ConfigValidationError
{
    /// <summary>
    /// JSON path to the error location (e.g., "fields[0].name")
    /// </summary>
    public string Path { get; set; } = string.Empty;

    /// <summary>
    /// Error message
    /// </summary>
    public string Message { get; set; } = string.Empty;
}

/// <summary>
/// Result of config validation
/// </summary>
public class ConfigValidationResult
{
    /// <summary>
    /// Whether the config is valid
    /// </summary>
    public bool Valid { get; set; }

    /// <summary>
    /// List of validation errors
    /// </summary>
    public List<ConfigValidationError> Errors { get; set; } = new();

    /// <summary>
    /// The parsed config (only set if valid)
    /// </summary>
    public FormConfig? Config { get; set; }

    /// <summary>
    /// Create a valid result
    /// </summary>
    public static ConfigValidationResult Success(FormConfig config) =>
        new() { Valid = true, Config = config };

    /// <summary>
    /// Create an invalid result with errors
    /// </summary>
    public static ConfigValidationResult Failure(List<ConfigValidationError> errors) =>
        new() { Valid = false, Errors = errors };

    /// <summary>
    /// Create an invalid result with a single error
    /// </summary>
    public static ConfigValidationResult Failure(string path, string message) =>
        new()
        {
            Valid = false,
            Errors = new List<ConfigValidationError>
            {
                new() { Path = path, Message = message }
            }
        };
}

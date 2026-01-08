using System.Text.Json.Serialization;

namespace DynamicForms.FormValidation.Models;

/// <summary>
/// Validation rule types
/// </summary>
public enum ValidationRuleType
{
    [JsonPropertyName("required")]
    Required,

    [JsonPropertyName("email")]
    Email,

    [JsonPropertyName("minLength")]
    MinLength,

    [JsonPropertyName("maxLength")]
    MaxLength,

    [JsonPropertyName("min")]
    Min,

    [JsonPropertyName("max")]
    Max,

    [JsonPropertyName("pattern")]
    Pattern,

    [JsonPropertyName("custom")]
    Custom
}

/// <summary>
/// Validation rule configuration
/// </summary>
public class ValidationRule
{
    /// <summary>
    /// Type of validation
    /// </summary>
    [JsonPropertyName("type")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ValidationRuleType Type { get; set; }

    /// <summary>
    /// Value for the validation (e.g., min/max length, regex pattern)
    /// </summary>
    [JsonPropertyName("value")]
    public object? Value { get; set; }

    /// <summary>
    /// Error message to display when validation fails
    /// </summary>
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Name of a registered custom validator
    /// </summary>
    [JsonPropertyName("customValidatorName")]
    public string? CustomValidatorName { get; set; }

    /// <summary>
    /// Parameters to pass to the custom validator
    /// </summary>
    [JsonPropertyName("customValidatorParams")]
    public Dictionary<string, object>? CustomValidatorParams { get; set; }
}

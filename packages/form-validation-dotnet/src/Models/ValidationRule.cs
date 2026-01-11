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
/// Comparison operators for validation conditions
/// </summary>
public enum ValidationConditionOperator
{
    [JsonPropertyName("equals")]
    Equals,

    [JsonPropertyName("notEquals")]
    NotEquals,

    [JsonPropertyName("isEmpty")]
    IsEmpty,

    [JsonPropertyName("isNotEmpty")]
    IsNotEmpty
}

/// <summary>
/// Condition that determines when a validation rule applies
/// </summary>
public class ValidationCondition
{
    /// <summary>
    /// Field/column to evaluate.
    /// - For standalone fields: field name (e.g., "tenure")
    /// - For table columns: column name for same-row (e.g., "tenure")
    ///   or "$form.fieldName" for form-level fields (e.g., "$form.employmentType")
    /// </summary>
    [JsonPropertyName("field")]
    public string Field { get; set; } = string.Empty;

    /// <summary>
    /// Comparison operator
    /// </summary>
    [JsonPropertyName("operator")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ValidationConditionOperator Operator { get; set; }

    /// <summary>
    /// Value to compare against. Required for 'equals' and 'notEquals'.
    /// Ignored for 'isEmpty' and 'isNotEmpty'.
    /// </summary>
    [JsonPropertyName("value")]
    public object? Value { get; set; }
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

    /// <summary>
    /// Optional condition that must be met for this validation to apply.
    /// If not specified, validation always applies.
    /// </summary>
    [JsonPropertyName("condition")]
    public ValidationCondition? Condition { get; set; }
}

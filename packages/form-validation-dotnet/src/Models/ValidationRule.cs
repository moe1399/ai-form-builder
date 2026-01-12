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

/// <summary>
/// Trigger type for async validation
/// </summary>
public enum AsyncValidationTrigger
{
    [JsonPropertyName("blur")]
    Blur,

    [JsonPropertyName("change")]
    Change
}

/// <summary>
/// Result of async validation
/// </summary>
public class AsyncValidationResult
{
    /// <summary>
    /// Whether the validation passed
    /// </summary>
    [JsonPropertyName("valid")]
    public bool Valid { get; set; }

    /// <summary>
    /// Optional error message when validation fails
    /// </summary>
    [JsonPropertyName("message")]
    public string? Message { get; set; }
}

/// <summary>
/// Async validation configuration using named validator pattern
/// Works across client and server by referencing registered validators
/// </summary>
public class AsyncValidationConfig
{
    /// <summary>
    /// Name of the registered async validator
    /// Must be registered on both client and server
    /// </summary>
    [JsonPropertyName("validatorName")]
    public string ValidatorName { get; set; } = string.Empty;

    /// <summary>
    /// Optional trigger for when validation should run
    /// - 'blur': Validate when field loses focus (default)
    /// - 'change': Validate on every change
    /// </summary>
    [JsonPropertyName("trigger")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public AsyncValidationTrigger? Trigger { get; set; }

    /// <summary>
    /// Debounce delay in milliseconds (default: 300)
    /// Only applies when trigger is 'change'
    /// </summary>
    [JsonPropertyName("debounceMs")]
    public int? DebounceMs { get; set; }

    /// <summary>
    /// Optional parameters to pass to the async validator
    /// </summary>
    [JsonPropertyName("params")]
    public Dictionary<string, object?>? Params { get; set; }
}

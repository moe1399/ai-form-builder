using DynamicForms.FormValidation.Models;

namespace DynamicForms.FormValidation;

/// <summary>
/// Custom validator function signature
/// </summary>
/// <param name="value">The value to validate</param>
/// <param name="parameters">Optional parameters for the validator</param>
/// <param name="fieldConfig">The field configuration</param>
/// <param name="formData">The complete form data</param>
/// <returns>True if valid, false if invalid</returns>
public delegate bool CustomValidatorFn(
    object? value,
    Dictionary<string, object>? parameters,
    FormFieldConfig? fieldConfig,
    Dictionary<string, object?>? formData
);

/// <summary>
/// Async validator function signature
/// </summary>
/// <param name="value">The value to validate</param>
/// <param name="parameters">Optional parameters for the validator</param>
/// <param name="fieldConfig">The field configuration</param>
/// <param name="formData">The complete form data</param>
/// <returns>AsyncValidationResult indicating whether validation passed</returns>
public delegate Task<AsyncValidationResult> AsyncValidatorFn(
    object? value,
    Dictionary<string, object?>? parameters,
    FormFieldConfig? fieldConfig,
    Dictionary<string, object?>? formData
);

/// <summary>
/// Registry for custom validators that can be referenced by name.
/// Register validators here to use them with ValidationRule.CustomValidatorName.
/// </summary>
/// <example>
/// <code>
/// ValidatorRegistry.Instance.Register("australianPhoneNumber", (value, _, _, _) => {
///     if (value == null) return true;
///     var phone = value.ToString()?.Replace(" ", "") ?? "";
///     return Regex.IsMatch(phone, @"^(\+61|0)[2-478]\d{8}$");
/// });
/// </code>
/// </example>
public sealed class ValidatorRegistry
{
    private static readonly Lazy<ValidatorRegistry> _instance =
        new(() => new ValidatorRegistry());

    private readonly Dictionary<string, CustomValidatorFn> _validators = new();

    private ValidatorRegistry() { }

    /// <summary>
    /// Singleton instance of the validator registry
    /// </summary>
    public static ValidatorRegistry Instance => _instance.Value;

    /// <summary>
    /// Register a custom validator by name
    /// </summary>
    /// <param name="name">Validator name</param>
    /// <param name="validator">Validator function</param>
    public void Register(string name, CustomValidatorFn validator)
    {
        if (_validators.ContainsKey(name))
        {
            Console.WriteLine($"ValidatorRegistry: Validator \"{name}\" is being overwritten");
        }
        _validators[name] = validator;
    }

    /// <summary>
    /// Register multiple validators at once
    /// </summary>
    /// <param name="validators">Dictionary of validator names to functions</param>
    public void RegisterAll(Dictionary<string, CustomValidatorFn> validators)
    {
        foreach (var kvp in validators)
        {
            Register(kvp.Key, kvp.Value);
        }
    }

    /// <summary>
    /// Get a validator by name
    /// </summary>
    /// <param name="name">Validator name</param>
    /// <returns>The validator function, or null if not found</returns>
    public CustomValidatorFn? Get(string name)
    {
        return _validators.TryGetValue(name, out var validator) ? validator : null;
    }

    /// <summary>
    /// Check if a validator exists
    /// </summary>
    /// <param name="name">Validator name</param>
    /// <returns>True if the validator exists</returns>
    public bool Has(string name) => _validators.ContainsKey(name);

    /// <summary>
    /// List all registered validator names
    /// </summary>
    /// <returns>List of validator names</returns>
    public List<string> List() => _validators.Keys.ToList();

    /// <summary>
    /// Remove a validator
    /// </summary>
    /// <param name="name">Validator name</param>
    /// <returns>True if the validator was removed</returns>
    public bool Unregister(string name) => _validators.Remove(name);

    /// <summary>
    /// Clear all custom validators
    /// </summary>
    public void Clear() => _validators.Clear();
}

/// <summary>
/// Registry for async validators that can be referenced by name.
/// Register async validators here to use them with AsyncValidationConfig.ValidatorName.
/// </summary>
/// <example>
/// <code>
/// AsyncValidatorRegistry.Instance.Register("checkEmailExists", async (value, _, _, _) => {
///     var email = value?.ToString();
///     var exists = await _emailService.EmailExistsAsync(email);
///     return new AsyncValidationResult { Valid = !exists, Message = exists ? "Email already exists" : null };
/// });
/// </code>
/// </example>
public sealed class AsyncValidatorRegistry
{
    private static readonly Lazy<AsyncValidatorRegistry> _instance =
        new(() => new AsyncValidatorRegistry());

    private readonly Dictionary<string, AsyncValidatorFn> _validators = new();

    private AsyncValidatorRegistry() { }

    /// <summary>
    /// Singleton instance of the async validator registry
    /// </summary>
    public static AsyncValidatorRegistry Instance => _instance.Value;

    /// <summary>
    /// Register an async validator by name
    /// </summary>
    /// <param name="name">Validator name</param>
    /// <param name="validator">Async validator function</param>
    public void Register(string name, AsyncValidatorFn validator)
    {
        if (_validators.ContainsKey(name))
        {
            Console.WriteLine($"AsyncValidatorRegistry: Validator \"{name}\" is being overwritten");
        }
        _validators[name] = validator;
    }

    /// <summary>
    /// Register multiple async validators at once
    /// </summary>
    /// <param name="validators">Dictionary of validator names to functions</param>
    public void RegisterAll(Dictionary<string, AsyncValidatorFn> validators)
    {
        foreach (var kvp in validators)
        {
            Register(kvp.Key, kvp.Value);
        }
    }

    /// <summary>
    /// Get an async validator by name
    /// </summary>
    /// <param name="name">Validator name</param>
    /// <returns>The async validator function, or null if not found</returns>
    public AsyncValidatorFn? Get(string name)
    {
        return _validators.TryGetValue(name, out var validator) ? validator : null;
    }

    /// <summary>
    /// Check if an async validator exists
    /// </summary>
    /// <param name="name">Validator name</param>
    /// <returns>True if the async validator exists</returns>
    public bool Has(string name) => _validators.ContainsKey(name);

    /// <summary>
    /// List all registered async validator names
    /// </summary>
    /// <returns>List of async validator names</returns>
    public List<string> List() => _validators.Keys.ToList();

    /// <summary>
    /// Remove an async validator
    /// </summary>
    /// <param name="name">Validator name</param>
    /// <returns>True if the async validator was removed</returns>
    public bool Unregister(string name) => _validators.Remove(name);

    /// <summary>
    /// Clear all async validators
    /// </summary>
    public void Clear() => _validators.Clear();
}

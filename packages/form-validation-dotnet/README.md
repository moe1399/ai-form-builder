# DynamicForms.FormValidation

Server-side form validation library for .NET compatible with `@moe1399/ngx-dynamic-forms`. Use the same form configuration and validation rules on both client and server.

## Installation

```bash
dotnet add package DynamicForms.FormValidation
```

## Usage

```csharp
using DynamicForms.FormValidation;
using DynamicForms.FormValidation.Models;
using System.Text.RegularExpressions;

// Register custom validators (same logic as Angular/Node.js)
ValidatorRegistry.Instance.Register("australianPhoneNumber", (value, _, _, _) =>
{
    if (value == null) return true;
    var phone = value.ToString()?.Replace(" ", "") ?? "";
    return Regex.IsMatch(phone, @"^(\+61|0)[2-478]\d{8}$");
});

// Use the same form config as your Angular app
var formConfig = new FormConfig
{
    Id = "contact-form",
    Fields = new List<FormFieldConfig>
    {
        new FormFieldConfig
        {
            Name = "email",
            Label = "Email",
            Type = FieldType.Email,
            Validations = new List<ValidationRule>
            {
                new ValidationRule { Type = ValidationRuleType.Required, Message = "Email is required" },
                new ValidationRule { Type = ValidationRuleType.Email, Message = "Invalid email format" }
            }
        },
        new FormFieldConfig
        {
            Name = "phone",
            Label = "Phone",
            Type = FieldType.Text,
            Validations = new List<ValidationRule>
            {
                new ValidationRule
                {
                    Type = ValidationRuleType.Custom,
                    CustomValidatorName = "australianPhoneNumber",
                    Message = "Invalid Australian phone number"
                }
            }
        }
    }
};

// Validate submitted form data
var formData = new Dictionary<string, object?>
{
    { "email", "test@example.com" },
    { "phone", "0412345678" }
};

var result = FormValidator.ValidateForm(formConfig, formData);

if (!result.Valid)
{
    foreach (var error in result.Errors)
    {
        Console.WriteLine($"{error.Field}: {error.Message}");
    }
}
```

## API

### `FormValidator.ValidateForm(config, data)`

Validates form data against a form configuration.

**Parameters:**
- `config: FormConfig` - The form configuration (same structure as `@moe1399/ngx-dynamic-forms`)
- `data: Dictionary<string, object?>` - The form data to validate

**Returns:** `ValidationResult`
```csharp
public class ValidationResult
{
    public bool Valid { get; set; }
    public List<FieldValidationError> Errors { get; set; }
}

public class FieldValidationError
{
    public string Field { get; set; }      // e.g., "email" or "contacts[0].phone"
    public string Message { get; set; }
    public string Rule { get; set; }       // Validation type that failed
}
```

### `FormValidator.ValidateFieldValue(fieldConfig, value, formData?)`

Validates a single field value.

**Parameters:**
- `fieldConfig: FormFieldConfig` - The field configuration
- `value: object?` - The value to validate
- `formData: Dictionary<string, object?>?` - Optional full form data for contextual validation

**Returns:** `ValidationResult`

### `ValidatorRegistry.Instance`

Singleton registry for custom validators.

```csharp
// Register a single validator
ValidatorRegistry.Instance.Register("validatorName", (value, parameters, fieldConfig, formData) =>
{
    return true; // or false
});

// Register multiple validators
ValidatorRegistry.Instance.RegisterAll(new Dictionary<string, CustomValidatorFn>
{
    { "validator1", (v, _, _, _) => true },
    { "validator2", (v, _, _, _) => true }
});

// Check if validator exists
ValidatorRegistry.Instance.Has("validatorName");

// List all registered validators
ValidatorRegistry.Instance.List();

// Remove a validator
ValidatorRegistry.Instance.Unregister("validatorName");

// Clear all validators
ValidatorRegistry.Instance.Clear();
```

## Built-in Validators

| Type | Value | Behavior |
|------|-------|----------|
| `required` | - | Non-empty value |
| `email` | - | Valid email format |
| `minLength` | number | String length >= value |
| `maxLength` | number | String length <= value |
| `min` | number | Numeric value >= value |
| `max` | number | Numeric value <= value |
| `pattern` | regex | Matches regex pattern |
| `custom` | - | Uses customValidatorName |

## ASP.NET Core Example

```csharp
using DynamicForms.FormValidation;
using DynamicForms.FormValidation.Models;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Register validators at startup
ValidatorRegistry.Instance.Register("australianPhoneNumber", (value, _, _, _) =>
{
    if (value == null) return true;
    var phone = value.ToString()?.Replace(" ", "") ?? "";
    return Regex.IsMatch(phone, @"^(\+61|0)[2-478]\d{8}$");
});

// Form config (could be loaded from database)
var formConfig = new FormConfig { /* ... */ };

app.MapPost("/api/submit", async (HttpContext context) =>
{
    var body = await new StreamReader(context.Request.Body).ReadToEndAsync();
    var data = JsonSerializer.Deserialize<Dictionary<string, object?>>(body)
        ?? new Dictionary<string, object?>();

    var result = FormValidator.ValidateForm(formConfig, data);

    if (!result.Valid)
    {
        return Results.BadRequest(new { errors = result.Errors });
    }

    // Process valid form data
    return Results.Ok(new { success = true });
});

app.Run();
```

## Schema Synchronization

The .NET models are **generated** from the TypeScript types via JSON Schema. This ensures they stay in sync with the `@moe1399/ngx-dynamic-forms-validation` package.

> **Note:** Regeneration is a **manual process**. When TypeScript types change, you must run the generation command to update the .NET models.

### Regenerating Models

From the repository root:

```bash
npm run generate:dotnet
```

This single command:
1. Builds the TypeScript validation package (generates JSON Schema)
2. Runs quicktype to generate C# models from the schema
3. Builds the .NET project to verify compilation

### How It Works

1. TypeScript types in `@moe1399/ngx-dynamic-forms-validation` are the source of truth
2. `npm run build` generates `form-config.schema.json` using `ts-json-schema-generator`
3. `npm run generate` uses `quicktype` to generate C# classes from the schema
4. A post-processor fixes naming conventions and adds missing types

### Generated Files

| File | Description |
|------|-------------|
| `src/Models/FormConfig.g.cs` | Auto-generated models (do not edit manually) |
| `src/Models/ValidationResult.cs` | Custom result types (manually maintained) |
| `src/Models/ConfigValidationResult.cs` | Config validation types (manually maintained) |

## JSON Deserialization

The library works with `System.Text.Json` for deserializing form configurations:

```csharp
var json = @"{
    ""id"": ""my-form"",
    ""fields"": [
        {
            ""name"": ""email"",
            ""label"": ""Email"",
            ""type"": ""email"",
            ""validations"": [
                { ""type"": ""required"", ""message"": ""Email is required"" }
            ]
        }
    ]
}";

var config = JsonSerializer.Deserialize<FormConfig>(json, new JsonSerializerOptions
{
    PropertyNameCaseInsensitive = true
});

var result = FormValidator.ValidateForm(config!, formData);
```

## License

MIT

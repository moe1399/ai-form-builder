using System.Text.Json;
using DynamicForms.FormValidation.Models;

namespace DynamicForms.FormValidation;

/// <summary>
/// Loads and validates form configurations from JSON
/// </summary>
public static class ConfigLoader
{
    private static readonly string[] ValidFieldTypes =
    {
        "text", "email", "number", "textarea", "select", "checkbox", "radio",
        "date", "daterange", "table", "info", "datagrid", "phone", "formref"
    };

    private static readonly string[] ValidValidationTypes =
    {
        "required", "email", "minLength", "maxLength", "min", "max", "pattern", "custom"
    };

    private static readonly JsonSerializerOptions DefaultJsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    /// <summary>
    /// Parse and validate a JSON string as FormConfig
    /// </summary>
    /// <param name="json">JSON string containing form configuration</param>
    /// <param name="options">Optional JSON serializer options</param>
    /// <returns>Validation result with parsed config if valid</returns>
    /// <example>
    /// <code>
    /// var json = File.ReadAllText("form-config.json");
    /// var result = ConfigLoader.ParseConfig(json);
    ///
    /// if (!result.Valid)
    /// {
    ///     foreach (var error in result.Errors)
    ///         Console.WriteLine($"{error.Path}: {error.Message}");
    ///     return;
    /// }
    ///
    /// var config = result.Config;
    /// </code>
    /// </example>
    public static ConfigValidationResult ParseConfig(string json, JsonSerializerOptions? options = null)
    {
        FormConfig? config;

        try
        {
            config = JsonSerializer.Deserialize<FormConfig>(json, options ?? DefaultJsonOptions);
        }
        catch (JsonException ex)
        {
            return ConfigValidationResult.Failure("", $"Invalid JSON: {ex.Message}");
        }

        if (config == null)
        {
            return ConfigValidationResult.Failure("", "Config cannot be null");
        }

        return ValidateConfig(config);
    }

    /// <summary>
    /// Load and validate a form config from a file
    /// </summary>
    /// <param name="filePath">Path to JSON config file</param>
    /// <param name="options">Optional JSON serializer options</param>
    /// <returns>Validation result with parsed config if valid</returns>
    public static ConfigValidationResult LoadConfig(string filePath, JsonSerializerOptions? options = null)
    {
        string json;

        try
        {
            json = File.ReadAllText(filePath);
        }
        catch (Exception ex)
        {
            return ConfigValidationResult.Failure("", $"Failed to read file: {ex.Message}");
        }

        return ParseConfig(json, options);
    }

    /// <summary>
    /// Load and validate a form config from a file asynchronously
    /// </summary>
    /// <param name="filePath">Path to JSON config file</param>
    /// <param name="options">Optional JSON serializer options</param>
    /// <returns>Validation result with parsed config if valid</returns>
    public static async Task<ConfigValidationResult> LoadConfigAsync(
        string filePath,
        JsonSerializerOptions? options = null)
    {
        string json;

        try
        {
            using var reader = new StreamReader(filePath);
            json = await reader.ReadToEndAsync();
        }
        catch (Exception ex)
        {
            return ConfigValidationResult.Failure("", $"Failed to read file: {ex.Message}");
        }

        return ParseConfig(json, options);
    }

    /// <summary>
    /// Validate a form config object
    /// </summary>
    /// <param name="config">Form configuration to validate</param>
    /// <returns>Validation result</returns>
    public static ConfigValidationResult ValidateConfig(FormConfig config)
    {
        var errors = new List<ConfigValidationError>();

        if (string.IsNullOrWhiteSpace(config.Id))
        {
            errors.Add(new ConfigValidationError { Path = "id", Message = "Form id is required" });
        }

        if (config.Fields == null || config.Fields.Count == 0)
        {
            errors.Add(new ConfigValidationError { Path = "fields", Message = "Fields must be a non-empty array" });
        }
        else
        {
            // Check for duplicate field names
            var fieldNames = new HashSet<string>();

            for (var i = 0; i < config.Fields.Count; i++)
            {
                var field = config.Fields[i];
                errors.AddRange(ValidateFieldConfig(field, i, "fields"));

                if (!string.IsNullOrEmpty(field.Name))
                {
                    if (fieldNames.Contains(field.Name))
                    {
                        errors.Add(new ConfigValidationError
                        {
                            Path = $"fields[{i}].name",
                            Message = $"Duplicate field name \"{field.Name}\""
                        });
                    }
                    else
                    {
                        fieldNames.Add(field.Name);
                    }
                }
            }
        }

        // Validate sections if present
        if (config.Sections != null && config.Sections.Count > 0)
        {
            var sectionIds = new HashSet<string>();

            for (var i = 0; i < config.Sections.Count; i++)
            {
                var section = config.Sections[i];

                if (string.IsNullOrEmpty(section.Id))
                {
                    errors.Add(new ConfigValidationError
                    {
                        Path = $"sections[{i}].id",
                        Message = "Section id is required"
                    });
                }
                else if (sectionIds.Contains(section.Id))
                {
                    errors.Add(new ConfigValidationError
                    {
                        Path = $"sections[{i}].id",
                        Message = $"Duplicate section id \"{section.Id}\""
                    });
                }
                else
                {
                    sectionIds.Add(section.Id);
                }

                if (string.IsNullOrEmpty(section.Title))
                {
                    errors.Add(new ConfigValidationError
                    {
                        Path = $"sections[{i}].title",
                        Message = "Section title is required"
                    });
                }
            }

            // Validate field sectionIds reference valid sections
            if (config.Fields != null)
            {
                for (var i = 0; i < config.Fields.Count; i++)
                {
                    var field = config.Fields[i];
                    if (!string.IsNullOrEmpty(field.SectionId) && !sectionIds.Contains(field.SectionId!))
                    {
                        errors.Add(new ConfigValidationError
                        {
                            Path = $"fields[{i}].sectionId",
                            Message = $"Field references non-existent section \"{field.SectionId}\""
                        });
                    }
                }
            }
        }

        return errors.Count == 0
            ? ConfigValidationResult.Success(config)
            : ConfigValidationResult.Failure(errors);
    }

    private static List<ConfigValidationError> ValidateFieldConfig(
        FormFieldConfig field,
        int index,
        string basePath)
    {
        var errors = new List<ConfigValidationError>();
        var fieldPath = $"{basePath}[{index}]";

        if (string.IsNullOrWhiteSpace(field.Name))
        {
            errors.Add(new ConfigValidationError
            {
                Path = $"{fieldPath}.name",
                Message = "Field name is required"
            });
        }

        if (string.IsNullOrWhiteSpace(field.Label))
        {
            errors.Add(new ConfigValidationError
            {
                Path = $"{fieldPath}.label",
                Message = "Field label is required"
            });
        }

        var fieldType = field.Type.ToString().ToLowerInvariant();
        if (!ValidFieldTypes.Contains(fieldType))
        {
            errors.Add(new ConfigValidationError
            {
                Path = $"{fieldPath}.type",
                Message = $"Invalid field type \"{fieldType}\". Valid types: {string.Join(", ", ValidFieldTypes)}"
            });
        }

        // Validate type-specific requirements
        if (field.Type == FieldType.Table)
        {
            if (field.TableConfig == null)
            {
                errors.Add(new ConfigValidationError
                {
                    Path = $"{fieldPath}.tableConfig",
                    Message = "Table field requires tableConfig"
                });
            }
            else
            {
                errors.AddRange(ValidateTableConfig(field.TableConfig, $"{fieldPath}.tableConfig"));
            }
        }

        if (field.Type == FieldType.DataGrid)
        {
            if (field.DataGridConfig == null)
            {
                errors.Add(new ConfigValidationError
                {
                    Path = $"{fieldPath}.datagridConfig",
                    Message = "DataGrid field requires datagridConfig"
                });
            }
            else
            {
                errors.AddRange(ValidateDataGridConfig(field.DataGridConfig, $"{fieldPath}.datagridConfig"));
            }
        }

        if (field.Type == FieldType.FormRef)
        {
            // Note: formrefConfig validation would go here
        }

        // Validate validations array
        if (field.Validations != null)
        {
            for (var i = 0; i < field.Validations.Count; i++)
            {
                errors.AddRange(ValidateValidationRule(field.Validations[i], $"{fieldPath}.validations[{i}]"));
            }
        }

        return errors;
    }

    private static List<ConfigValidationError> ValidateValidationRule(
        ValidationRule rule,
        string path)
    {
        var errors = new List<ConfigValidationError>();

        var ruleType = rule.Type.ToString().ToLowerInvariant();
        if (!ValidValidationTypes.Contains(ruleType))
        {
            errors.Add(new ConfigValidationError
            {
                Path = $"{path}.type",
                Message = $"Invalid validation type \"{ruleType}\". Valid types: {string.Join(", ", ValidValidationTypes)}"
            });
        }

        if (string.IsNullOrEmpty(rule.Message))
        {
            errors.Add(new ConfigValidationError
            {
                Path = $"{path}.message",
                Message = "Validation rule message is required"
            });
        }

        // Validate value requirements for specific types
        var typesRequiringValue = new[] { ValidationRuleType.MinLength, ValidationRuleType.MaxLength,
            ValidationRuleType.Min, ValidationRuleType.Max };

        if (typesRequiringValue.Contains(rule.Type) && rule.Value == null)
        {
            errors.Add(new ConfigValidationError
            {
                Path = $"{path}.value",
                Message = $"Validation type \"{ruleType}\" requires a value"
            });
        }

        if (rule.Type == ValidationRuleType.Pattern && rule.Value == null)
        {
            errors.Add(new ConfigValidationError
            {
                Path = $"{path}.value",
                Message = "Pattern validation requires a regex value"
            });
        }

        if (rule.Type == ValidationRuleType.Custom && string.IsNullOrEmpty(rule.CustomValidatorName))
        {
            errors.Add(new ConfigValidationError
            {
                Path = $"{path}.customValidatorName",
                Message = "Custom validation requires customValidatorName"
            });
        }

        return errors;
    }

    private static List<ConfigValidationError> ValidateTableConfig(
        TableConfig config,
        string path)
    {
        var errors = new List<ConfigValidationError>();

        if (config.Columns == null || config.Columns.Count == 0)
        {
            errors.Add(new ConfigValidationError
            {
                Path = $"{path}.columns",
                Message = "Table must have at least one column"
            });
        }
        else
        {
            for (var i = 0; i < config.Columns.Count; i++)
            {
                var col = config.Columns[i];

                if (string.IsNullOrEmpty(col.Name))
                {
                    errors.Add(new ConfigValidationError
                    {
                        Path = $"{path}.columns[{i}].name",
                        Message = "Column name is required"
                    });
                }

                if (string.IsNullOrEmpty(col.Label))
                {
                    errors.Add(new ConfigValidationError
                    {
                        Path = $"{path}.columns[{i}].label",
                        Message = "Column label is required"
                    });
                }

                if (col.Validations != null)
                {
                    for (var j = 0; j < col.Validations.Count; j++)
                    {
                        errors.AddRange(ValidateValidationRule(
                            col.Validations[j],
                            $"{path}.columns[{i}].validations[{j}]"));
                    }
                }
            }
        }

        if (string.IsNullOrEmpty(config.RowMode) ||
            (config.RowMode != "fixed" && config.RowMode != "dynamic"))
        {
            errors.Add(new ConfigValidationError
            {
                Path = $"{path}.rowMode",
                Message = "Table rowMode must be \"fixed\" or \"dynamic\""
            });
        }

        return errors;
    }

    private static List<ConfigValidationError> ValidateDataGridConfig(
        DataGridConfig config,
        string path)
    {
        var errors = new List<ConfigValidationError>();

        if (config.Columns == null || config.Columns.Count == 0)
        {
            errors.Add(new ConfigValidationError
            {
                Path = $"{path}.columns",
                Message = "DataGrid must have at least one column"
            });
        }
        else
        {
            for (var i = 0; i < config.Columns.Count; i++)
            {
                var col = config.Columns[i];

                if (string.IsNullOrEmpty(col.Name))
                {
                    errors.Add(new ConfigValidationError
                    {
                        Path = $"{path}.columns[{i}].name",
                        Message = "Column name is required"
                    });
                }

                if (string.IsNullOrEmpty(col.Label))
                {
                    errors.Add(new ConfigValidationError
                    {
                        Path = $"{path}.columns[{i}].label",
                        Message = "Column label is required"
                    });
                }

                if (col.Validations != null)
                {
                    for (var j = 0; j < col.Validations.Count; j++)
                    {
                        errors.AddRange(ValidateValidationRule(
                            col.Validations[j],
                            $"{path}.columns[{i}].validations[{j}]"));
                    }
                }
            }
        }

        if (config.RowLabels == null || config.RowLabels.Count == 0)
        {
            errors.Add(new ConfigValidationError
            {
                Path = $"{path}.rowLabels",
                Message = "DataGrid must have at least one row label"
            });
        }
        else
        {
            for (var i = 0; i < config.RowLabels.Count; i++)
            {
                var row = config.RowLabels[i];

                if (string.IsNullOrEmpty(row.Id))
                {
                    errors.Add(new ConfigValidationError
                    {
                        Path = $"{path}.rowLabels[{i}].id",
                        Message = "Row label id is required"
                    });
                }

                if (string.IsNullOrEmpty(row.Label))
                {
                    errors.Add(new ConfigValidationError
                    {
                        Path = $"{path}.rowLabels[{i}].label",
                        Message = "Row label is required"
                    });
                }
            }
        }

        return errors;
    }
}

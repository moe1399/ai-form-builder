# Checkboxes and Radio Buttons Editable in Read-Only State

## Description

Checkboxes and radio buttons can be changed when the form is in read-only state when they should not be.

## Affected Components

- `DynamicFormComponent` - Main form component
- Affected field types: `checkbox`, `radio`
- Also affects `formref` embedded fields containing checkboxes/radios

## Root Cause Analysis

### Read-Only State Implementation

The form has a `readOnly` property (`_readOnly` signal) that can be set via `setReadOnly(value: boolean)`. This state is exposed as a data attribute on the form element:

```html
[attr.data-form-readonly]="readOnly"
```

### Correct Implementation for Other Input Types

Most input types correctly respect the read-only state:

| Input Type | Implementation |
|------------|----------------|
| text, email, number, textarea, date, phone | `[readonly]="readOnly"` |
| select dropdowns | `[attr.tabindex]="readOnly ? -1 : null"` |

### The Bug: Missing Read-Only Handling

**Radio buttons** and **checkboxes** completely lack any read-only state handling:

```html
<!-- Radio buttons - NO readonly/disabled binding -->
<input type="radio" [formControlName]="field.name" ... />

<!-- Checkboxes - NO readonly/disabled binding -->
<input type="checkbox" [formControlName]="field.name" ... />
```

This allows users to interact with and change the values of checkboxes and radio buttons even when `readOnly` is `true`.

### Affected Code Locations

| Location | Field Type | Lines |
|----------|------------|-------|
| `dynamic-form.html` | radio | 141-160 |
| `dynamic-form.html` | checkbox (multi-select) | 164-189 |
| `dynamic-form.html` | checkbox (single boolean) | 191-199 |
| `dynamic-form.html` | radio (formref with sections) | 767-781 |
| `dynamic-form.html` | checkbox (formref with sections) | 783-804 |
| `dynamic-form.html` | radio (formref without sections) | 890-904, 1011-1025 |
| `dynamic-form.html` | checkbox (formref without sections) | 906-927, 1027-1048 |

## Recommended Fix

For radio buttons and checkboxes, use the `[disabled]` binding:

```html
<!-- Radio buttons -->
<input
  type="radio"
  [formControlName]="field.name"
  [disabled]="readOnly"
  ...
/>

<!-- Checkboxes -->
<input
  type="checkbox"
  [formControlName]="field.name"
  [disabled]="readOnly"
  ...
/>
```

**Note:** The `readonly` attribute does not work on radio and checkbox inputs in HTML. The `disabled` attribute is the correct approach for preventing interaction.

## Data Attributes

The following data attributes should be preserved/added for styling:

- `data-radio-input` - Radio button inputs
- `data-checkbox-input` - Checkbox inputs
- `data-checkbox-single` - Single boolean checkbox

Consider adding `[attr.data-field-disabled]="readOnly || field.disabled"` to maintain consistency with the headless UI pattern.

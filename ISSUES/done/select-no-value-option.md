# Select Fields Should Have a No-Value Option by Default

## Description

Regular select fields do not have a "no value" option by default, while select columns in table and datagrid field types do. This inconsistency makes it difficult to clear/reset regular select fields to an empty state.

## Current Implementation

### Regular Select Field (Missing No-Value Option)

```html
@case ('select') {
  <select
    [id]="field.name"
    [formControlName]="field.name"
    [attr.data-input-type]="field.type"
    [attr.tabindex]="readOnly ? -1 : null">
    @if (field.placeholder) {
      <option value="" disabled>{{ field.placeholder }}</option>
    }
    @for (option of field.options; track option.value) {
      <option [value]="option.value">{{ option.label }}</option>
    }
  </select>
}
```

**Characteristics:**
- Only adds a placeholder option if `field.placeholder` is defined
- Placeholder option is `disabled` (cannot be selected)
- No way to clear/reset selection once a value is chosen
- Empty state (`null`/`undefined`/`""`) must be set programmatically

### Table Select Column (Has No-Value Option)

```html
@case ('select') {
  <select
    [formControlName]="column.name"
    data-table-input
    [attr.data-input-type]="column.type"
    [attr.tabindex]="readOnly ? -1 : null">
    @if (column.placeholder) {
      <option value="" disabled>{{ column.placeholder }}</option>
    }
    <option value="">--</option>  <!-- Always present -->
    @for (option of column.options; track option.value) {
      <option [value]="option.value">{{ option.label }}</option>
    }
  </select>
}
```

**Characteristics:**
- Adds placeholder option if `column.placeholder` is defined (disabled)
- **ALWAYS has `<option value="">--</option>`** for clearing selection
- Users can reset to empty state by selecting "--"

### DataGrid Select Column (Also Has No-Value Option)

```html
@case ('select') {
  <select
    [formControlName]="column.name"
    data-datagrid-input
    [attr.data-input-type]="column.type"
    [attr.tabindex]="readOnly ? -1 : null">
    <option value="">--</option>  <!-- Always present -->
    @for (option of column.options; track option.value) {
      <option [value]="option.value">{{ option.label }}</option>
    }
  </select>
}
```

**Characteristics:**
- **Always has `<option value="">--</option>`** (no placeholder check)
- Consistent clearing behavior across all datagrid selects

## Problem

### Inconsistency

| Context | Placeholder Option | Empty "--" Option | Can Clear Selection? |
|---------|-------------------|-------------------|---------------------|
| Regular Select | Optional (if configured) | ❌ No | Only via API |
| Table Select | Optional (if configured) | ✅ Always | Yes, via "--" |
| DataGrid Select | ❌ No | ✅ Always | Yes, via "--" |

### User Experience Issues

1. **Cannot clear selection** - Once a user selects an option in a regular select, they cannot deselect it via the UI
2. **Inconsistent behavior** - Same field type (`select`) behaves differently in different contexts
3. **Confusing** - Users expect to be able to "undo" a selection by choosing a blank option
4. **Required vs optional** - Optional fields especially need a way to be cleared

### Edge Cases

- **Required fields** - The empty option can be validated as invalid (standard HTML5 behavior)
- **Preselected values** - Still need ability to clear to "no selection"
- **Placeholder disabled** - Disabled placeholder cannot be selected once another option is chosen

## Recommended Fix

### Add Default Empty Option to Regular Select

```html
@case ('select') {
  <select
    [id]="field.name"
    [formControlName]="field.name"
    [attr.data-input-type]="field.type"
    [attr.tabindex]="readOnly ? -1 : null">
    @if (field.placeholder) {
      <option value="" disabled>{{ field.placeholder }}</option>
    }
    <option value="">--</option>  <!-- Add this line -->
    @for (option of field.options; track option.value) {
      <option [value]="option.value">{{ option.label }}</option>
    }
  </select>
}
```

### Configuration Option (Alternative)

If automatic behavior is not desired, add a configuration option:

```typescript
interface SelectFieldConfig extends BaseFieldConfig {
  type: 'select';
  options: { value: string; label: string }[];
  placeholder?: string;
  allowEmpty?: boolean;  // New config option
  emptyLabel?: string;   // Defaults to '--'
}
```

```html
@if (field.allowEmpty !== false) {  <!-- true by default -->
  <option value="">{{ field.emptyLabel ?? '--' }}</option>
}
```

### Validation Handling

- **Required fields**: Empty option still fails `required` validation (expected behavior)
- **Optional fields**: Empty option passes validation, allows clearing selection
- **No breaking changes**: Existing forms with preselected values continue to work

## Affected Code Locations

| File | Lines | Description |
|------|-------|-------------|
| `projects/ngx-dynamic-forms/src/lib/components/dynamic-form/dynamic-form.html` | 127-140 | Regular select field rendering |
| `projects/ngx-dynamic-forms/src/lib/components/dynamic-form/dynamic-form.html` | 272-286 | Table select column (reference - already correct) |
| `projects/ngx-dynamic-forms/src/lib/components/dynamic-form/dynamic-form.html` | 606-617 | DataGrid select column (reference - already correct) |

## Consistency with Other Implementations

This change aligns regular select fields with:
- ✅ Table select columns (line 281: `<option value="">--</option>`)
- ✅ DataGrid select columns (line 607: `<option value="">--</option>`)

## Data Attributes

Preserve existing data attributes:
- `data-input-type="select"` - Already set via `[attr.data-input-type]="field.type"`

# Read-Only and Disabled Should Be Input Attributes

## Description

Read-only and disabled states are currently implemented as imperative methods that consumers must call, rather than declarative input attributes on the `DynamicFormComponent`. This violates Angular best practices and makes the component harder to use.

## Current Implementation

### Read-Only State (Imperative API)

```typescript
private _readOnly = signal(false);

get readOnly(): boolean {
  return this._readOnly();
}

setReadOnly(value: boolean): void {
  this._readOnly.set(value);
  this.cdr.markForCheck();
}
```

### Disabled State (No Form-Level Control)

- Field-level `disabled` exists in `FormConfig` (`field.disabled`)
- No form-level `disabled` input
- Only getter: `get disabled(): boolean { return this.form.disabled; }`

## Problem

### Consumer Must Use Imperative Pattern

```typescript
@Component({
  template: `
    <ngx-dynamic-form [config]="formConfig"></ngx-dynamic-form>
  `
})
export class ParentComponent {
  @ViewChild(DynamicForm) dynamicForm!: DynamicForm;

  ngOnInit() {
    // Imperative method call - not ideal
    this.dynamicForm.setReadOnly(true);
  }
}
```

### Issues with Current Approach

1. **Not declarative** - Cannot bind `[readOnly]="isReadOnly"` in template
2. **Requires ViewChild** - Adds boilerplate and coupling
3. **No reactive updates** - Changes require explicit method calls
4. **Unidiomatic Angular** - Modern Angular uses `input()` not setters
5. **No disabled input** - Cannot disable entire form declaratively
6. **Inconsistent with other inputs** - Component already uses `input()` for `config`, `fileUploadHandler`, `fileDownloadHandler`

## Recommended Fix

### Add Input Attributes Using `input()` Function

```typescript
export class DynamicForm implements OnInit, OnDestroy {
  // Existing inputs
  config = input.required<FormConfig>();
  fileUploadHandler = input<FileUploadHandler | undefined>(undefined);
  fileDownloadHandler = input<FileDownloadHandler | undefined>(undefined);

  // New inputs
  readOnly = input(false, {
    alias: 'readOnly',
    transform: (value: boolean | string) => booleanAttribute(value)
  });

  disabled = input(false, {
    alias: 'disabled',
    transform: (value: boolean | string) => booleanAttribute(value)
  });
}
```

### Update Internal Logic

1. Remove `private _readOnly` signal - use the `readOnly` input directly
2. Remove `setReadOnly()` method (or keep as internal alias)
3. When `disabled` input changes, call `this.form.disable()` / `this.form.enable()`
4. Use `effect()` to react to input changes:

```typescript
constructor() {
  // React to disabled input changes
  effect(() => {
    if (this.disabled()) {
      this.form.disable();
    } else {
      this.form.enable();
    }
  });
}
```

### Consumer Usage After Fix

```typescript
@Component({
  template: `
    <!-- Declarative, reactive, idiomatic -->
    <ngx-dynamic-form
      [config]="formConfig"
      [readOnly]="isReadOnly"
      [disabled]="isDisabled">
    </ngx-dynamic-form>
  `
})
export class ParentComponent {
  isReadOnly = signal(false);
  isDisabled = signal(false);

  toggleReadOnly() {
    this.isReadOnly.update(v => !v); // Automatically updates form
  }
}
```

## Affected Code Locations

| File | Lines | Description |
|------|-------|-------------|
| `projects/ngx-dynamic-forms/src/lib/components/dynamic-form/dynamic-form.ts` | 127-144 | Current `readOnly` signal + getter/setter |
| `projects/ngx-dynamic-forms/src/lib/components/dynamic-form/dynamic-form.ts` | 119-122 | Current `disabled` getter |
| `projects/ngx-dynamic-forms/src/lib/components/dynamic-form/dynamic-form.ts` | ~75-80 | Input declarations (needs new inputs added here) |

## Breaking Changes

### Public API Changes

- **Add**: `[readOnly]` input (optional, no breaking change)
- **Add**: `[disabled]` input (optional, no breaking change)
- **Deprecate**: `setReadOnly()` method (can keep for backwards compatibility)
- **Keep**: `get readOnly()` and `get disabled()` getters (read-only access)

### Migration Path

1. Add new `input()` declarations
2. Keep `setReadOnly()` method but mark as `@deprecated`
3. Update documentation to recommend input bindings
4. Consumers can migrate at their own pace

## Data Attributes

The following data attributes are already in use and should be preserved:
- `data-form-readonly` - Already set via `[attr.data-form-readonly]="readOnly"`
- `data-field-disabled` - Already set via `[attr.data-field-disabled]="form.get(field.name)?.disabled"`

## Resolution

Implemented as recommended:

1. Added `readOnlyInput` and `disabledInput` inputs using Angular's `input()` function with `booleanAttribute` transform
2. Added internal `readOnlyOverride` signal to support deprecated `setReadOnly()` method
3. Kept `readOnly` getter that combines input value with override for backwards compatibility
4. Marked `setReadOnly()` as `@deprecated`
5. Added effect in constructor to react to `disabledInput` changes and call `form.disable()`/`form.enable()`
6. Added `data-form-disabled` attribute to the form element template

Consumers can now use declarative bindings:
```html
<ngx-dynamic-form
  [config]="formConfig"
  [readOnly]="isReadOnly"
  [disabled]="isDisabled">
</ngx-dynamic-form>
```

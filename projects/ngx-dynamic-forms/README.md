# ngx-dynamic-forms

Dynamic form builder and renderer for Angular with a headless UI pattern.

## Features

- **Headless UI Pattern**: Zero default styling, complete styling freedom via `data-*` attributes
- **14 Field Types**: text, email, number, textarea, date, daterange, select, radio, checkbox, table, datagrid, phone, info, formref
- **Visual Form Builder**: Drag-drop field ordering, validation rules, sections
- **JSON-driven**: Define forms via JSON configuration
- **Auto-save**: Built-in local storage persistence
- **URL Sharing**: Compressed schema sharing via URL parameters

## Installation

```bash
npm install ngx-dynamic-forms
```

## Usage

### Basic Form Renderer

```typescript
import { Component } from '@angular/core';
import { DynamicForm, FormConfig } from 'ngx-dynamic-forms';

@Component({
  selector: 'app-my-form',
  imports: [DynamicForm],
  template: `<ngx-dynamic-form [config]="formConfig" (formSubmit)="onSubmit($event)" />`
})
export class MyFormComponent {
  formConfig: FormConfig = {
    id: 'contact-form',
    fields: [
      { name: 'name', label: 'Name', type: 'text', validations: [{ type: 'required', message: 'Name is required' }] },
      { name: 'email', label: 'Email', type: 'email' }
    ],
    submitLabel: 'Send'
  };

  onSubmit(data: any) {
    console.log('Form submitted:', data);
  }
}
```

### Form Builder

```typescript
import { Component } from '@angular/core';
import { NgxFormBuilder, FormConfig } from 'ngx-dynamic-forms';

@Component({
  selector: 'app-builder',
  imports: [NgxFormBuilder],
  template: `<ngx-form-builder [(config)]="formConfig" />`
})
export class BuilderComponent {
  formConfig: FormConfig | null = null;
}
```

### Styling

Import the default theme in your `styles.scss`:

```scss
@import 'ngx-dynamic-forms/styles/ngx-dynamic-forms';
```

Or create custom styles using `data-*` attribute selectors:

```scss
[data-form-id] {
  // Form container styles
}

[data-field-valid="false"] {
  // Invalid field styles
}

[data-validation-error] {
  // Error message styles
}
```

## API

### DynamicForm

| Input | Type | Description |
|-------|------|-------------|
| `config` | `FormConfig` | Form configuration |

| Output | Type | Description |
|--------|------|-------------|
| `formSubmit` | `EventEmitter<object>` | Emitted on form submit |
| `formSave` | `EventEmitter<object>` | Emitted on form save |
| `validationErrors` | `EventEmitter<FieldError[]>` | Emitted on validation changes |

### NgxFormBuilder

| Input | Type | Description |
|-------|------|-------------|
| `config` | `FormConfig` | Two-way binding for form config |
| `showToolbar` | `boolean` | Show/hide toolbar |
| `toolbarConfig` | `ToolbarConfig` | Configure toolbar buttons |

| Output | Type | Description |
|--------|------|-------------|
| `saveRequested` | `EventEmitter<FormConfig>` | Emitted when save is clicked |
| `exportRequested` | `EventEmitter<FormConfig>` | Emitted when export is clicked |

## License

MIT

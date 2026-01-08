/**
 * Types for @moe1399/form-validation
 * These mirror the types from @moe1399/ngx-dynamic-forms
 */

/**
 * Supported form field types
 */
export type FieldType =
  | 'text'
  | 'email'
  | 'number'
  | 'textarea'
  | 'select'
  | 'checkbox'
  | 'radio'
  | 'date'
  | 'daterange'
  | 'table'
  | 'info'
  | 'datagrid'
  | 'phone'
  | 'formref';

/**
 * Table column configuration
 */
export interface TableColumnConfig {
  name: string;
  label: string;
  type: 'text' | 'number' | 'date' | 'select';
  placeholder?: string;
  validations?: ValidationRule[];
  options?: { label: string; value: any }[];
  width?: number;
}

/**
 * Table field configuration
 */
export interface TableConfig {
  columns: TableColumnConfig[];
  rowMode: 'fixed' | 'dynamic';
  fixedRowCount?: number;
  minRows?: number;
  maxRows?: number;
  addRowLabel?: string;
  removeRowLabel?: string;
}

/**
 * DataGrid column configuration
 */
export interface DataGridColumnConfig {
  name: string;
  label: string;
  type: 'text' | 'number' | 'date' | 'select';
  placeholder?: string;
  validations?: ValidationRule[];
  options?: { label: string; value: any }[];
  width?: number;
  computed?: boolean;
  formula?: { type: 'expression'; expression: string };
  showInColumnTotal?: boolean;
  showInRowTotal?: boolean;
}

/**
 * DataGrid row label configuration
 */
export interface DataGridRowLabel {
  id: string;
  label: string;
}

/**
 * DataGrid field configuration
 */
export interface DataGridConfig {
  columns: DataGridColumnConfig[];
  rowLabels: DataGridRowLabel[];
  columnGroups?: { id: string; label: string; columnIds: string[] }[];
  rowLabelHeader?: string;
  totals?: {
    showRowTotals?: boolean;
    rowTotalLabel?: string;
    showColumnTotals?: boolean;
    columnTotalLabel?: string;
  };
}

/**
 * Validation rule configuration
 */
export interface ValidationRule {
  type: 'required' | 'email' | 'minLength' | 'maxLength' | 'min' | 'max' | 'pattern' | 'custom';
  value?: any;
  message: string;
  /**
   * Name of a registered custom validator
   */
  customValidatorName?: string;
  /**
   * Parameters to pass to the custom validator
   */
  customValidatorParams?: Record<string, any>;
}

/**
 * Form field configuration
 */
export interface FormFieldConfig {
  name: string;
  label: string;
  type: FieldType;
  placeholder?: string;
  description?: string;
  value?: any;
  validations?: ValidationRule[];
  options?: { label: string; value: any }[];
  disabled?: boolean;
  archived?: boolean;
  cssClass?: string;
  order?: number;
  inlineGroup?: string;
  width?: number;
  sectionId?: string;
  tableConfig?: TableConfig;
  datagridConfig?: DataGridConfig;
  content?: string;
  phoneConfig?: {
    countryCodes: { code: string; country: string; flag?: string }[];
    defaultCountryCode?: string;
  };
  daterangeConfig?: {
    fromLabel?: string;
    toLabel?: string;
    separatorText?: string;
    toDateOptional?: boolean;
  };
  formrefConfig?: {
    formId: string;
    showSections?: boolean;
    fieldPrefix?: string;
  };
}

/**
 * Form section configuration
 */
export interface FormSection {
  id: string;
  title: string;
  description?: string;
  anchorId?: string;
  order?: number;
}

/**
 * Complete form configuration
 */
export interface FormConfig {
  id: string;
  fields: FormFieldConfig[];
  sections?: FormSection[];
  submitLabel?: string;
  saveLabel?: string;
  autoSave?: boolean;
  autoSaveInterval?: number;
}

/**
 * Custom validator function signature
 */
export type CustomValidatorFn = (
  value: any,
  params?: Record<string, any>,
  fieldConfig?: FormFieldConfig,
  formData?: Record<string, any>
) => boolean;

/**
 * Field validation error
 */
export interface FieldValidationError {
  field: string;
  message: string;
  rule: string;
}

/**
 * Validation result
 */
export interface ValidationResult {
  valid: boolean;
  errors: FieldValidationError[];
}

/**
 * Supported form field types
 */
export type FieldType = 'text' | 'email' | 'number' | 'textarea' | 'select' | 'checkbox' | 'radio' | 'date' | 'table' | 'info' | 'datagrid';

/**
 * Column types supported within tables (subset of FieldType)
 */
export type TableColumnType = 'text' | 'number' | 'date' | 'select';

/**
 * Table row mode configuration
 */
export type TableRowMode = 'fixed' | 'dynamic';

/**
 * Table column configuration
 */
export interface TableColumnConfig {
  name: string;
  label: string;
  type: TableColumnType;
  placeholder?: string;
  validations?: ValidationRule[];
  options?: { label: string; value: any }[]; // For select columns
  width?: number; // Column width as flex proportion (1-4)
}

/**
 * Table field configuration
 */
export interface TableConfig {
  columns: TableColumnConfig[];
  rowMode: TableRowMode;
  fixedRowCount?: number; // For fixed mode: number of rows to display (default: 3)
  minRows?: number; // For dynamic mode: minimum rows (default: 0)
  maxRows?: number; // For dynamic mode: maximum rows (default: 10)
  addRowLabel?: string; // Button text for adding rows (default: 'Add row')
  removeRowLabel?: string; // Button aria-label for removing rows (default: 'Remove')
}

/**
 * Column types supported within datagrids
 */
export type DataGridColumnType = 'text' | 'number' | 'date' | 'select';

/**
 * DataGrid column group configuration for two-tier headers
 */
export interface DataGridColumnGroup {
  id: string;
  label: string;
  columnIds: string[]; // References to DataGridColumnConfig.name
}

/**
 * Computed column formula configuration
 */
export interface DataGridFormula {
  type: 'expression';
  expression: string; // e.g., "students * 0.5" or "col1 + col2"
}

/**
 * DataGrid column configuration
 */
export interface DataGridColumnConfig {
  name: string;
  label: string;
  type: DataGridColumnType;
  placeholder?: string;
  validations?: ValidationRule[];
  options?: { label: string; value: any }[]; // For select columns
  width?: number; // Column width (1-4)
  computed?: boolean; // If true, column is read-only and calculated
  formula?: DataGridFormula; // Formula for computed columns
  showInColumnTotal?: boolean; // Include in total row calculation (default: true for number)
  showInRowTotal?: boolean; // Include in row total calculation (default: true for number)
}

/**
 * DataGrid row label configuration
 */
export interface DataGridRowLabel {
  id: string; // Unique identifier for the row
  label: string; // Display label (e.g., "Year 1", "13+")
}

/**
 * DataGrid totals configuration
 */
export interface DataGridTotalsConfig {
  showRowTotals?: boolean; // Add a total column on the right
  rowTotalLabel?: string; // Header for total column (default: "Total")
  showColumnTotals?: boolean; // Add a total row at the bottom
  columnTotalLabel?: string; // Label for total row (default: "Total")
}

/**
 * DataGrid field configuration
 */
export interface DataGridConfig {
  columns: DataGridColumnConfig[];
  rowLabels: DataGridRowLabel[];
  columnGroups?: DataGridColumnGroup[];
  rowLabelHeader?: string; // Header for the row label column (e.g., "Year Level")
  totals?: DataGridTotalsConfig;
}

/**
 * Form section configuration
 */
export interface FormSection {
  id: string;
  title: string;
  description?: string;
  anchorId?: string; // Custom anchor ID (auto-generated from title if not provided)
  order?: number;
}

/**
 * Validation rule configuration
 */
export interface ValidationRule {
  type: 'required' | 'email' | 'minLength' | 'maxLength' | 'min' | 'max' | 'pattern' | 'custom';
  value?: any;
  message: string;
  validator?: (value: any) => boolean;
}

/**
 * Field configuration for dynamic form
 */
export interface FormFieldConfig {
  name: string;
  label: string;
  type: FieldType;
  placeholder?: string;
  description?: string; // Help text shown via info icon popover
  value?: any;
  validations?: ValidationRule[];
  options?: { label: string; value: any }[]; // For select, radio, checkbox
  disabled?: boolean;
  cssClass?: string;
  order?: number;
  inlineGroup?: string; // Fields with same group name render on same row
  width?: number; // Width as flex proportion (1-4), defaults to 1
  sectionId?: string; // Reference to FormSection.id
  tableConfig?: TableConfig; // Configuration for table field type
  datagridConfig?: DataGridConfig; // Configuration for datagrid field type
  content?: string; // Markdown content for info field type
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
  autoSaveInterval?: number; // milliseconds
}

/**
 * Form submission data
 */
export interface FormSubmission {
  formId: string;
  data: { [key: string]: any };
  timestamp: Date;
  isComplete: boolean;
}

/**
 * Field validation error
 */
export interface FieldError {
  field: string;
  message: string;
}

/**
 * @moe1399/ngx-dynamic-forms-validation
 *
 * Server-side form validation library compatible with @moe1399/ngx-dynamic-forms.
 * Use the same form configuration and validation rules on both client and server.
 *
 * @example
 * ```typescript
 * import { validateForm, validatorRegistry } from '@moe1399/ngx-dynamic-forms-validation';
 *
 * // Register custom validators (same implementation as Angular)
 * validatorRegistry.register('australianPhoneNumber', (value) => {
 *   if (!value) return true;
 *   return /^(\+61|0)[2-478]\d{8}$/.test(value.replace(/\s/g, ''));
 * });
 *
 * // Validate submitted form data
 * const result = validateForm(formConfig, formData);
 *
 * if (!result.valid) {
 *   // Return validation errors to client
 *   res.status(400).json({ errors: result.errors });
 * }
 * ```
 */
// Main validation functions
export { validateForm, validateFieldValue, validateFieldAsync, validateFormAsync } from './validator.js';
// Config loading and validation
export { parseConfig, validateConfig, loadConfig, loadConfigSync, } from './config-loader.js';
// Validator registry and autocomplete fetch registry
export { validatorRegistry, asyncValidatorRegistry, autocompleteFetchRegistry } from './registry.js';
/**
 * Relative path to the JSON Schema file for FormConfig validation.
 * The schema is auto-generated from TypeScript types and included in the package distribution.
 *
 * @example
 * ```typescript
 * // ESM with import attributes
 * import schema from '@moe1399/ngx-dynamic-forms-validation/schema' with { type: 'json' };
 *
 * // With Ajv validator
 * import Ajv from 'ajv';
 * const ajv = new Ajv();
 * const validate = ajv.compile(schema);
 * const isValid = validate(formConfig);
 * ```
 */
export const FORM_CONFIG_SCHEMA_PATH = 'form-config.schema.json';

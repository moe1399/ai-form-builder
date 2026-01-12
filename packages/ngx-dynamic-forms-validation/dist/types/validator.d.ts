import { FormConfig, FormFieldConfig, ValidationResult, AsyncValidationResult } from './types.js';
/**
 * Validate an entire form against its configuration
 *
 * @param config - The form configuration
 * @param data - The form data to validate
 * @returns ValidationResult with valid boolean and any errors
 *
 * @example
 * ```typescript
 * import { validateForm, validatorRegistry } from '@moe1399/form-validation';
 *
 * // Register custom validators first
 * validatorRegistry.register('australianPhoneNumber', (value) => {
 *   if (!value) return true;
 *   return /^(\+61|0)[2-478]\d{8}$/.test(value.replace(/\s/g, ''));
 * });
 *
 * // Validate form data
 * const result = validateForm(formConfig, formData);
 * if (!result.valid) {
 *   console.log('Validation errors:', result.errors);
 * }
 * ```
 */
export declare function validateForm(config: FormConfig, data: Record<string, any>): ValidationResult;
/**
 * Validate a single field value
 *
 * @param fieldConfig - The field configuration
 * @param value - The value to validate
 * @param formData - Optional full form data for contextual validation
 * @returns ValidationResult for the single field
 */
export declare function validateFieldValue(fieldConfig: FormFieldConfig, value: any, formData?: Record<string, any>): ValidationResult;
/**
 * Validate a single field's async validator
 *
 * @param fieldConfig - The field configuration
 * @param value - The value to validate
 * @param formData - Optional full form data for contextual validation
 * @returns Promise<AsyncValidationResult> from the async validator
 *
 * @example
 * ```typescript
 * import { validateFieldAsync, asyncValidatorRegistry } from '@moe1399/form-validation';
 *
 * // Register async validator
 * asyncValidatorRegistry.register('checkEmailExists', async (value) => {
 *   const response = await fetch(`/api/validate/email?email=${encodeURIComponent(value)}`);
 *   const result = await response.json();
 *   return { valid: result.available, message: result.available ? undefined : 'Email already exists' };
 * });
 *
 * // Use async validation
 * const result = await validateFieldAsync(fieldConfig, emailValue, formData);
 * if (!result.valid) {
 *   console.log(result.message);
 * }
 * ```
 */
export declare function validateFieldAsync(fieldConfig: FormFieldConfig, value: any, formData?: Record<string, any>): Promise<AsyncValidationResult>;
/**
 * Validate all async validators for a form
 *
 * @param config - The form configuration
 * @param data - The form data to validate
 * @returns Promise<ValidationResult> with async validation errors
 *
 * @example
 * ```typescript
 * import { validateFormAsync, asyncValidatorRegistry } from '@moe1399/form-validation';
 *
 * // Register async validators first
 * asyncValidatorRegistry.register('checkEmailExists', async (value) => { ... });
 * asyncValidatorRegistry.register('checkUsernameUnique', async (value) => { ... });
 *
 * // Validate form data asynchronously
 * const result = await validateFormAsync(formConfig, formData);
 * if (!result.valid) {
 *   console.log('Async validation errors:', result.errors);
 * }
 * ```
 */
export declare function validateFormAsync(config: FormConfig, data: Record<string, any>): Promise<ValidationResult>;

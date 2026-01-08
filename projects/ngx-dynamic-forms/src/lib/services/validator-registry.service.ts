import { Injectable } from '@angular/core';
import { FormFieldConfig } from '../models';

/**
 * Custom validator function signature for named validators.
 * Named validators can be registered and used across client and server.
 */
export type CustomValidatorFn = (
  value: any,
  params?: Record<string, any>,
  fieldConfig?: FormFieldConfig,
  formData?: Record<string, any>
) => boolean;

/**
 * Registry for custom validators that can be referenced by name.
 * Register validators here to use them in ValidationRule.customValidatorName.
 *
 * @example
 * ```typescript
 * // Register at app startup
 * validatorRegistry.register('australianPhoneNumber', (value) => {
 *   if (!value) return true;
 *   return /^(\+61|0)[2-478]\d{8}$/.test(value.replace(/\s/g, ''));
 * });
 *
 * // Use in form config
 * {
 *   type: 'custom',
 *   customValidatorName: 'australianPhoneNumber',
 *   message: 'Invalid Australian phone number'
 * }
 * ```
 */
@Injectable({
  providedIn: 'root',
})
export class ValidatorRegistry {
  private validators = new Map<string, CustomValidatorFn>();

  /**
   * Register a custom validator by name
   */
  register(name: string, validator: CustomValidatorFn): void {
    if (this.validators.has(name)) {
      console.warn(`ValidatorRegistry: Validator "${name}" is being overwritten`);
    }
    this.validators.set(name, validator);
  }

  /**
   * Register multiple validators at once
   */
  registerAll(validators: Record<string, CustomValidatorFn>): void {
    for (const [name, validator] of Object.entries(validators)) {
      this.register(name, validator);
    }
  }

  /**
   * Get a validator by name
   */
  get(name: string): CustomValidatorFn | undefined {
    return this.validators.get(name);
  }

  /**
   * Check if a validator exists
   */
  has(name: string): boolean {
    return this.validators.has(name);
  }

  /**
   * List all registered validator names
   */
  list(): string[] {
    return Array.from(this.validators.keys());
  }

  /**
   * Remove a validator
   */
  unregister(name: string): boolean {
    return this.validators.delete(name);
  }

  /**
   * Clear all custom validators
   */
  clear(): void {
    this.validators.clear();
  }
}

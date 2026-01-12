import { Injectable } from '@angular/core';
import { FormFieldConfig, AsyncValidationResult } from '../models';

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
 * Async validator function signature for named validators.
 * Named validators can be registered and used across client and server.
 */
export type AsyncValidatorFn = (
  value: any,
  params?: Record<string, any>,
  fieldConfig?: FormFieldConfig,
  formData?: Record<string, any>
) => Promise<AsyncValidationResult>;

// Shared storage maps - used by both Angular DI instances and global singletons
// This ensures validators registered on the global singleton are visible to Angular components
const sharedValidatorMap = new Map<string, CustomValidatorFn>();
const sharedAsyncValidatorMap = new Map<string, AsyncValidatorFn>();

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
  private validators = sharedValidatorMap;

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

/**
 * Registry for async validators that can be referenced by name.
 * Register async validators here to use them in AsyncValidationConfig.validatorName.
 *
 * @example
 * ```typescript
 * // Register at app startup
 * asyncValidatorRegistry.register('checkEmailExists', async (value) => {
 *   const response = await fetch(`/api/validate/email?email=${encodeURIComponent(value)}`);
 *   const result = await response.json();
 *   return { valid: result.available, message: result.available ? undefined : 'Email already exists' };
 * });
 *
 * // Use in form config
 * {
 *   name: 'email',
 *   type: 'email',
 *   asyncValidation: {
 *     validatorName: 'checkEmailExists',
 *     trigger: 'blur'
 *   }
 * }
 * ```
 */
@Injectable({
  providedIn: 'root',
})
export class AsyncValidatorRegistry {
  private validators = sharedAsyncValidatorMap;

  /**
   * Register an async validator by name
   */
  register(name: string, validator: AsyncValidatorFn): void {
    if (this.validators.has(name)) {
      console.warn(`AsyncValidatorRegistry: Validator "${name}" is being overwritten`);
    }
    this.validators.set(name, validator);
  }

  /**
   * Register multiple async validators at once
   */
  registerAll(validators: Record<string, AsyncValidatorFn>): void {
    for (const [name, validator] of Object.entries(validators)) {
      this.register(name, validator);
    }
  }

  /**
   * Get an async validator by name
   */
  get(name: string): AsyncValidatorFn | undefined {
    return this.validators.get(name);
  }

  /**
   * Check if an async validator exists
   */
  has(name: string): boolean {
    return this.validators.has(name);
  }

  /**
   * List all registered async validator names
   */
  list(): string[] {
    return Array.from(this.validators.keys());
  }

  /**
   * Remove an async validator
   */
  unregister(name: string): boolean {
    return this.validators.delete(name);
  }

  /**
   * Clear all async validators
   */
  clear(): void {
    this.validators.clear();
  }
}

/**
 * Global validator registry instance (for backwards compatibility)
 * @deprecated Use ValidatorRegistry as a service injected via constructor
 */
export const validatorRegistry = new ValidatorRegistry();

/**
 * Global async validator registry instance
 * @deprecated Use AsyncValidatorRegistry as a service injected via constructor
 */
export const asyncValidatorRegistry = new AsyncValidatorRegistry();

import { CustomValidatorFn, AsyncValidatorFn } from './types.js';

/**
 * Registry for custom validators that can be referenced by name.
 * Register validators here to use them with ValidationRule.customValidatorName.
 *
 * @example
 * ```typescript
 * import { validatorRegistry } from '@moe1399/form-validation';
 *
 * validatorRegistry.register('australianPhoneNumber', (value) => {
 *   if (!value) return true;
 *   return /^(\+61|0)[2-478]\d{8}$/.test(value.replace(/\s/g, ''));
 * });
 * ```
 */
class ValidatorRegistry {
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

/**
 * Registry for async validators that can be referenced by name.
 * Register async validators here to use them with AsyncValidationConfig.validatorName.
 *
 * @example
 * ```typescript
 * import { asyncValidatorRegistry } from '@moe1399/form-validation';
 *
 * asyncValidatorRegistry.register('checkEmailExists', async (value, params) => {
 *   const response = await fetch(`/api/validate/email?email=${encodeURIComponent(value)}`);
 *   const result = await response.json();
 *   return { valid: result.available, message: result.available ? undefined : 'Email already exists' };
 * });
 * ```
 */
class AsyncValidatorRegistry {
  private validators = new Map<string, AsyncValidatorFn>();

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
 * Global validator registry singleton
 */
export const validatorRegistry = new ValidatorRegistry();

/**
 * Global async validator registry singleton
 */
export const asyncValidatorRegistry = new AsyncValidatorRegistry();

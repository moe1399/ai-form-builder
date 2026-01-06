import { Injectable } from '@angular/core';
import { FormSubmission } from '../models/form-config.interface';

@Injectable({
  providedIn: 'root',
})
export class FormStorage {
  private readonly STORAGE_PREFIX = 'dynamic_form_';

  /**
   * Save form data to local storage
   */
  saveForm(formId: string, data: { [key: string]: any }, isComplete: boolean = false): void {
    const submission: FormSubmission = {
      formId,
      data,
      timestamp: new Date(),
      isComplete,
    };

    try {
      localStorage.setItem(
        this.getStorageKey(formId),
        JSON.stringify(submission)
      );
    } catch (error) {
      console.error('Error saving form to local storage:', error);
    }
  }

  /**
   * Load form data from local storage
   */
  loadForm(formId: string): FormSubmission | null {
    try {
      const stored = localStorage.getItem(this.getStorageKey(formId));
      if (stored) {
        const submission = JSON.parse(stored) as FormSubmission;
        submission.timestamp = new Date(submission.timestamp);
        return submission;
      }
    } catch (error) {
      console.error('Error loading form from local storage:', error);
    }
    return null;
  }

  /**
   * Clear form data from local storage
   */
  clearForm(formId: string): void {
    try {
      localStorage.removeItem(this.getStorageKey(formId));
    } catch (error) {
      console.error('Error clearing form from local storage:', error);
    }
  }

  /**
   * Check if form data exists in local storage
   */
  hasStoredForm(formId: string): boolean {
    return localStorage.getItem(this.getStorageKey(formId)) !== null;
  }

  /**
   * Get all stored forms
   */
  getAllForms(): FormSubmission[] {
    const forms: FormSubmission[] = [];
    try {
      for (let i = 0; i < localStorage.length; i++) {
        const key = localStorage.key(i);
        if (key?.startsWith(this.STORAGE_PREFIX)) {
          const stored = localStorage.getItem(key);
          if (stored) {
            const submission = JSON.parse(stored) as FormSubmission;
            submission.timestamp = new Date(submission.timestamp);
            forms.push(submission);
          }
        }
      }
    } catch (error) {
      console.error('Error getting all forms from local storage:', error);
    }
    return forms;
  }

  private getStorageKey(formId: string): string {
    return `${this.STORAGE_PREFIX}${formId}`;
  }
}

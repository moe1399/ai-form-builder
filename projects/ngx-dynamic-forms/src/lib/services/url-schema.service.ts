import { Injectable } from '@angular/core';
import { deflate, inflate } from 'pako';
import { FormConfig } from '../models/form-config.interface';

@Injectable({
  providedIn: 'root',
})
export class UrlSchemaService {
  private readonly SCHEMA_PARAM = 'schema';

  /**
   * Encodes a FormConfig into a compressed, URL-safe string
   */
  encodeSchema(config: FormConfig): string {
    const json = JSON.stringify(config);
    const compressed = deflate(json, { level: 9 });
    return this.uint8ArrayToBase64Url(compressed);
  }

  /**
   * Decodes a compressed, URL-safe string back into a FormConfig
   */
  decodeSchema(encoded: string): FormConfig | null {
    try {
      const compressed = this.base64UrlToUint8Array(encoded);
      const json = inflate(compressed, { to: 'string' });
      return JSON.parse(json) as FormConfig;
    } catch (error) {
      console.error('Failed to decode schema:', error);
      return null;
    }
  }

  /**
   * Generates a shareable URL with the encoded schema
   */
  generateShareUrl(config: FormConfig): string {
    const encoded = this.encodeSchema(config);
    const url = new URL(window.location.href);
    url.searchParams.set(this.SCHEMA_PARAM, encoded);
    return url.toString();
  }

  /**
   * Extracts and decodes a schema from the current URL
   */
  getSchemaFromUrl(): FormConfig | null {
    const params = new URLSearchParams(window.location.search);
    const encoded = params.get(this.SCHEMA_PARAM);
    if (!encoded) {
      return null;
    }
    return this.decodeSchema(encoded);
  }

  /**
   * Checks if the current URL has a schema parameter
   */
  hasSchemaInUrl(): boolean {
    const params = new URLSearchParams(window.location.search);
    return params.has(this.SCHEMA_PARAM);
  }

  /**
   * Clears the schema parameter from the URL without reloading the page
   */
  clearSchemaFromUrl(): void {
    const url = new URL(window.location.href);
    url.searchParams.delete(this.SCHEMA_PARAM);
    window.history.replaceState({}, '', url.toString());
  }

  /**
   * Copies the share URL to clipboard and returns success status
   */
  async copyShareUrlToClipboard(config: FormConfig): Promise<boolean> {
    try {
      const url = this.generateShareUrl(config);
      await navigator.clipboard.writeText(url);
      return true;
    } catch (error) {
      console.error('Failed to copy to clipboard:', error);
      return false;
    }
  }

  /**
   * Converts Uint8Array to URL-safe base64
   */
  private uint8ArrayToBase64Url(bytes: Uint8Array): string {
    const binary = String.fromCharCode(...bytes);
    const base64 = btoa(binary);
    // Make URL-safe: replace + with -, / with _, remove padding =
    return base64.replace(/\+/g, '-').replace(/\//g, '_').replace(/=+$/, '');
  }

  /**
   * Converts URL-safe base64 back to Uint8Array
   */
  private base64UrlToUint8Array(base64Url: string): Uint8Array {
    // Restore standard base64: replace - with +, _ with /
    let base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
    // Add padding if needed
    const padding = (4 - (base64.length % 4)) % 4;
    base64 += '='.repeat(padding);
    const binary = atob(base64);
    const bytes = new Uint8Array(binary.length);
    for (let i = 0; i < binary.length; i++) {
      bytes[i] = binary.charCodeAt(i);
    }
    return bytes;
  }
}

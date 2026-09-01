import { registerPlugin } from '@capacitor/core';

/**
 * Native (Android) bridge to the AndroidX Credential Manager - the system
 * "save password" and "choose a saved account" sheets backed by Google
 * Password Manager and any other installed credential provider.
 *
 * Implemented by `CredentialManagerPlugin.java`. Android-only; on web / iOS the
 * registered proxy rejects, so callers must guard with
 * `Capacitor.getPlatform() === 'android'` (or a try/catch).
 *
 * All methods are best-effort: a declined sheet or missing provider resolves
 * rather than throwing, and `getPassword` resolves with `null` fields when
 * there is nothing saved.
 */
export interface CredentialManagerPlugin {
  /** True when the Credential Manager could be created on this device. */
  isAvailable(): Promise<{ available: boolean }>;

  /**
   * Offer to store `username` / `password` in the system credential provider.
   * Always resolves - a declined or unsupported save is not an error.
   */
  savePassword(options: { username: string; password: string }): Promise<void>;

  /**
   * Show the "choose a saved password" sheet for this app. Resolves with the
   * picked credential, or `{ username: null, password: null }` when the user
   * dismisses it or nothing is saved.
   */
  getPassword(): Promise<{
    username: string | null;
    password: string | null;
  }>;
}

export const CredentialManager =
  registerPlugin<CredentialManagerPlugin>('CredentialManager');

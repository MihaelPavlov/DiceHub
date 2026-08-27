import { Injectable } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { Subscription, debounceTime } from 'rxjs';

export interface IFormDraftOptions<TExtra = Record<string, unknown>> {
  /** Control names to leave out of persistence entirely - always exclude passwords, tokens, and File-typed controls (not JSON-serializable). */
  exclude?: string[];
  /** How stale a draft can be before it's discarded instead of restored. Defaults to 24h. */
  maxAgeMs?: number;
  /** Debounce between a keystroke and the draft actually being written. Defaults to 500ms. */
  debounceMs?: number;
  /** Extra non-form component state to save/restore alongside the form values (e.g. a wizard step, a verified-email flag). */
  getExtra?: () => TExtra;
  applyExtra?: (extra: TExtra) => void;
}

interface IDraft<TExtra> {
  values: Record<string, unknown>;
  extra?: TExtra;
  savedAt: number;
}

/**
 * Auto-saves a reactive form's values to localStorage as the user types, and restores them
 * on the next visit. Exists because Android can kill this app's background process at any
 * time (trivially triggered by something as ordinary as opening the gallery/camera from a
 * file input), which wipes all in-memory component/form state - Angular reinitializes the
 * component fresh on relaunch with no memory of what the user had typed.
 */
@Injectable({ providedIn: 'root' })
export class FormDraftService {
  private static readonly DefaultMaxAgeMs = 24 * 60 * 60 * 1000;
  private static readonly DefaultDebounceMs = 500;
  private static readonly StorageKeyPrefix = 'formDraft:';

  /**
   * Restores any existing draft into the form immediately, then wires up debounced
   * auto-save on every subsequent change. Call `clear()` on successful submit, and
   * unsubscribe the returned Subscription in ngOnDestroy.
   */
  public autoSave<TExtra = Record<string, unknown>>(
    form: FormGroup,
    key: string,
    options: IFormDraftOptions<TExtra> = {}
  ): Subscription {
    this.restore(form, key, options);

    return form.valueChanges
      .pipe(debounceTime(options.debounceMs ?? FormDraftService.DefaultDebounceMs))
      .subscribe(() => this.save(form, key, options));
  }

  public save<TExtra>(form: FormGroup, key: string, options: IFormDraftOptions<TExtra> = {}): void {
    const values = form.getRawValue();
    (options.exclude ?? []).forEach((field) => delete values[field]);

    const draft: IDraft<TExtra> = {
      values,
      extra: options.getExtra?.(),
      savedAt: Date.now(),
    };

    localStorage.setItem(this.storageKey(key), JSON.stringify(draft));
  }

  public restore<TExtra>(form: FormGroup, key: string, options: IFormDraftOptions<TExtra> = {}): boolean {
    const raw = localStorage.getItem(this.storageKey(key));
    if (!raw) {
      return false;
    }

    try {
      const draft = JSON.parse(raw) as IDraft<TExtra>;
      const maxAgeMs = options.maxAgeMs ?? FormDraftService.DefaultMaxAgeMs;
      if (Date.now() - draft.savedAt > maxAgeMs) {
        this.clear(key);
        return false;
      }

      (options.exclude ?? []).forEach((field) => delete draft.values[field]);
      form.patchValue(draft.values);

      if (draft.extra !== undefined) {
        options.applyExtra?.(draft.extra);
      }

      return true;
    } catch {
      this.clear(key);
      return false;
    }
  }

  /** Whether a (not-yet-expired-check'd) draft exists for this key - use to decide whether to reopen a form/panel that a restored draft belongs to. */
  public hasDraft(key: string): boolean {
    return localStorage.getItem(this.storageKey(key)) !== null;
  }

  public clear(key: string): void {
    localStorage.removeItem(this.storageKey(key));
  }

  private storageKey(key: string): string {
    return `${FormDraftService.StorageKeyPrefix}${key}`;
  }
}

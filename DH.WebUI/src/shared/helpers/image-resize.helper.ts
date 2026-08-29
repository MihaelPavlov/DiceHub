/**
 * Client-side image downscaling. Reward/game/event/logo photos come straight
 * off a phone camera at several MB; the API relays every byte to Supabase
 * Storage in-band, so shrinking here is the single biggest win for those
 * upload requests.
 *
 * Safe by construction: vector images are passed through untouched, already-small
 * images are skipped, and any failure (decode error, blocked canvas, missing
 * toBlob) falls back to the original File so an upload is never blocked.
 */

export interface IDownscaleOptions {
  /** Longest edge, in px, of the output image. Default 1280. */
  maxDimension?: number;
  /** JPEG/WebP quality 0..1. Default 0.72. */
  quality?: number;
  /**
   * Output mime type. Default 'image/jpeg'. Pass 'image/png' for assets that
   * may rely on transparency (e.g. tenant/club logos).
   */
  mimeType?: string;
}

const DEFAULT_MAX_DIMENSION = 1280;
const DEFAULT_QUALITY = 0.72;
/** Below this the re-encode rarely helps and can even grow the file. */
const SKIP_BELOW_BYTES = 300 * 1024;

export async function downscaleImageFile(
  file: File,
  options: IDownscaleOptions = {}
): Promise<File> {
  const maxDimension = options.maxDimension ?? DEFAULT_MAX_DIMENSION;
  const quality = options.quality ?? DEFAULT_QUALITY;
  const mimeType = options.mimeType ?? 'image/jpeg';

  // Vector: canvas rasterisation would destroy it, not shrink it.
  if (file.type === 'image/svg+xml') {
    return file;
  }

  if (!file.type.startsWith('image/')) {
    return file;
  }

  try {
    const bitmap = await loadBitmap(file);
    const { width, height } = bitmap;

    const scale = Math.min(1, maxDimension / Math.max(width, height));
    const alreadySmall = scale === 1 && file.size <= SKIP_BELOW_BYTES;
    if (alreadySmall) {
      disposeBitmap(bitmap);
      return file;
    }

    const targetWidth = Math.max(1, Math.round(width * scale));
    const targetHeight = Math.max(1, Math.round(height * scale));

    const canvas = document.createElement('canvas');
    canvas.width = targetWidth;
    canvas.height = targetHeight;
    const ctx = canvas.getContext('2d');
    if (!ctx) {
      disposeBitmap(bitmap);
      return file;
    }
    ctx.drawImage(bitmap, 0, 0, targetWidth, targetHeight);
    disposeBitmap(bitmap);

    const blob = await canvasToBlob(canvas, mimeType, quality);
    if (!blob || blob.size >= file.size) {
      // No saving (or encoder unavailable) - keep the original.
      return file;
    }

    return new File([blob], renameForType(file.name, mimeType), {
      type: mimeType,
      lastModified: Date.now(),
    });
  } catch {
    return file;
  }
}

async function loadBitmap(file: File): Promise<ImageBitmap | HTMLImageElement> {
  if (typeof createImageBitmap === 'function') {
    try {
      return await createImageBitmap(file);
    } catch {
      // Fall through to the <img> path (e.g. some WebP/orientation cases).
    }
  }

  const url = URL.createObjectURL(file);
  try {
    const img = new Image();
    await new Promise<void>((resolve, reject) => {
      img.onload = () => resolve();
      img.onerror = () => reject(new Error('image decode failed'));
      img.src = url;
    });
    return img;
  } finally {
    URL.revokeObjectURL(url);
  }
}

function disposeBitmap(bitmap: ImageBitmap | HTMLImageElement): void {
  if (typeof ImageBitmap !== 'undefined' && bitmap instanceof ImageBitmap) {
    bitmap.close();
  }
}

function canvasToBlob(
  canvas: HTMLCanvasElement,
  mimeType: string,
  quality: number
): Promise<Blob | null> {
  return new Promise((resolve) => {
    canvas.toBlob((blob) => resolve(blob), mimeType, quality);
  });
}

function renameForType(name: string, mimeType: string): string {
  const ext =
    mimeType === 'image/png'
      ? 'png'
      : mimeType === 'image/webp'
      ? 'webp'
      : 'jpg';
  const dot = name.lastIndexOf('.');
  const base = dot > 0 ? name.slice(0, dot) : name;
  return `${base}.${ext}`;
}

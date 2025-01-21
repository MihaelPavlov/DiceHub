import { Injectable } from '@angular/core';
import { ImageEntityType } from '../pipe/entity-image.pipe';

interface CachedImageEntry {
  imageId: number; // ID of the image
  blobUrl: string; // Blob URL for the image
  timestamp: number; // Timestamp when the image was cached
}

@Injectable({
  providedIn: 'root',
})
export class CacheService {
  private imageCache: Map<ImageEntityType, CachedImageEntry[]> = new Map();
  private cacheExpiryDuration = 10 * 60 * 1000; // 10 minutes in milliseconds

  public get(entityType: ImageEntityType, imageId: number): string | undefined {
    this.cleanExpiredEntries(entityType); // Remove expired images
    const entityCache = this.imageCache.get(entityType) || [];
    const cachedImage = entityCache.find((entry) => entry.imageId === imageId);
    return cachedImage ? cachedImage.blobUrl : undefined;
  }

  public set(
    entityType: ImageEntityType,
    imageId: number,
    blobUrl: string
  ): void {
    const entityCache = this.imageCache.get(entityType) || [];
    const newEntry: CachedImageEntry = {
      imageId,
      blobUrl,
      timestamp: Date.now(),
    };
    this.imageCache.set(entityType, [...entityCache, newEntry]);
  }

  private cleanExpiredEntries(entityType: ImageEntityType): void {
    const currentCache = this.imageCache.get(entityType) || [];
    const currentTime = Date.now();
    const validCache = currentCache.filter(
      (entry) => currentTime - entry.timestamp < this.cacheExpiryDuration
    );
    this.imageCache.set(entityType, validCache);
  }
}

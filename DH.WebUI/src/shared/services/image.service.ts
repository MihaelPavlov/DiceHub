import { Injectable } from '@angular/core';
import { RestApiService } from '../services/rest-api.service';
import { DomSanitizer, SafeUrl } from '@angular/platform-browser';
import { BehaviorSubject, Observable, of } from 'rxjs';
import { catchError, switchMap } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class ImageService {
  private cachedImageUrls: { [key: number]: SafeUrl | null } = {};
  private loadingSubjects: { [key: number]: BehaviorSubject<SafeUrl | null> } = {};

  constructor(
    private api: RestApiService,
    private sanitizer: DomSanitizer
  ) {}

  getImage(imageId: number): Observable<SafeUrl | null> {
    // Return cached URL if it exists
    if (this.cachedImageUrls[imageId]) {
      return of(this.cachedImageUrls[imageId]);
    }

    // If a request is already in progress, return the observable
    if (!this.loadingSubjects[imageId]) {
      this.loadingSubjects[imageId] = new BehaviorSubject<SafeUrl | null>(null);
      this.loadImage(imageId);
    }

    return this.loadingSubjects[imageId].asObservable(); // Return the observable for ongoing requests
  }

  private loadImage(imageId: number): void {
    this.api.get<Blob>(`/events/get-image/${imageId}`, { responseType: 'blob' }).pipe(
      switchMap((imageBlob: Blob) => {
        const objectURL = URL.createObjectURL(imageBlob);
        const safeUrl = this.sanitizer.bypassSecurityTrustUrl(objectURL);
        this.cachedImageUrls[imageId] = safeUrl;
        this.loadingSubjects[imageId].next(safeUrl); // Emit the safe URL
        return of(safeUrl); // Return as observable
      }),
      catchError((error) => {
        console.error(`Failed to load image for ID ${imageId}:`, error);
        this.cachedImageUrls[imageId] = null; // Mark as failed to load
        this.loadingSubjects[imageId].next(null); // Emit null on error
        return of(null); // Return null on error
      })
    ).subscribe({
      complete: () => {
        // Cleanup the subject after the request is complete
        delete this.loadingSubjects[imageId];
      }
    });
  }
}

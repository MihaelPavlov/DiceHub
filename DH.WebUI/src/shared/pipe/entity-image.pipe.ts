import { AuthService } from './../../entities/auth/auth.service';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Pipe, PipeTransform } from '@angular/core';
import { environment } from '../environments/environment.development';
import { catchError, map, Observable, of, shareReplay } from 'rxjs';
import { CacheService } from '../services/image-cache.service';

/* 
The Enums is representing the entity types that can have an image.
The value of the enum is the endpoint of the API that will be used to fetch the image.
*/
export enum ImageEntityType {
  Games = 'games',
  Events = 'events',
  Rewards = 'rewards',
}

@Pipe({
  name: 'entityImage',
  pure: true,
})
export class EntityImagePipe implements PipeTransform {
  constructor(
    private readonly http: HttpClient,
    private readonly authService: AuthService,
    private readonly cacheService: CacheService
  ) {}

  transform(entityType: ImageEntityType, imageId: number): Observable<string> {
    const cachedImageUrl = this.cacheService.get(entityType, imageId);
    if (cachedImageUrl) {
      return of(cachedImageUrl);
    }

    return this.http
      .get(`${environment.defaultAppUrl}/${entityType}/get-image/${imageId}`, {
        headers: new HttpHeaders().set(
          'Authorization',
          `Bearer ${this.authService.getToken()}`
        ),
        responseType: 'blob',
      })
      .pipe(
        map((imageBlob: Blob) => {
          const imageUrl = URL.createObjectURL(imageBlob);
          this.cacheService.set(entityType, imageId, imageUrl);
          return imageUrl;
        }),
        catchError((error) => {
          console.error('Image fetch failed:', error);
          return of(''); // Fallback to an empty string or a default image URL
        })
      );
  }
}

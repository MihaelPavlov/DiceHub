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
    standalone: false
})
export class EntityImagePipe implements PipeTransform {
  constructor(
    private readonly http: HttpClient,
    private readonly authService: AuthService,
    private readonly cacheService: CacheService
  ) {}

  transform(entityType: ImageEntityType, imageId: number): Observable<string> {
    const cachedImageUrl = this.cacheService.get(entityType, imageId);
    const imageSharedUrl = 'shared/assets/images/default-no-image.jpg';

    if (cachedImageUrl) {
      return of(cachedImageUrl);
    }

    return this.http
      .get(`${environment.defaultAppUrl}/api/${entityType}/get-image/${imageId}`, {
        headers: new HttpHeaders().set(
          'Authorization',
          `Bearer ${this.authService.getToken()}`
        ),
        responseType: 'blob',
      })
      .pipe(
        map((imageBlob: Blob) => {
          if (imageBlob.size === 0) {
            // If the image is empty, return the default image URL
            this.cacheService.set(entityType, imageId, imageSharedUrl);
            return imageSharedUrl;
          }

          const imageUrl = URL.createObjectURL(imageBlob);
          this.cacheService.set(entityType, imageId, imageUrl);
          return imageUrl;
        }),
        catchError((error) => {
          this.cacheService.set(entityType, imageId, imageSharedUrl);

          return of(imageSharedUrl);
        })
      );
  }
}

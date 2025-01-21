import { AuthService } from './../../entities/auth/auth.service';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Pipe, PipeTransform } from '@angular/core';
import { environment } from '../environments/environment.development';
import { Observable } from 'rxjs';

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
    private readonly authService: AuthService
  ) {}

  transform(entityType: ImageEntityType, imageId: number): Observable<string> {
    // Fetch the image as a blob and convert it to a Blob URL
    return new Observable((observer) => {
      this.http
        .get(
          `${environment.defaultAppUrl}/${entityType}/get-image/${imageId}`,
          {
            headers: new HttpHeaders().set(
              'Authorization',
              `Bearer ${this.authService.getToken()}`
            ),
            responseType: 'blob',
          }
        )
        .subscribe({
          next: (imageBlob) => {
            const imageUrl = URL.createObjectURL(imageBlob);
            observer.next(imageUrl);
            observer.complete();
          },
          error: (error) => {
            observer.error(error);
          },
        });
    });
  }
}

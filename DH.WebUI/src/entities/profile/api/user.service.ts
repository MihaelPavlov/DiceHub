import { Injectable } from '@angular/core';
import { RestApiService } from '../../../shared/services/rest-api.service';
import { Observable } from 'rxjs';
import { IUser } from '../models/user.model';
import { PATH } from '../../../shared/configs/path.config';

@Injectable({
  providedIn: 'root',
})
export class UsersService {
  constructor(private readonly api: RestApiService) {}

  public getEmployeeList(): Observable<IUser[] | null> {
    return this.api.get<IUser[]>(
      `/${PATH.USER.CORE}/${PATH.USER.GET_EMPLOYEE_LIST}`
    );
  }
}

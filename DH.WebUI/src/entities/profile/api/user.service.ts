import { Injectable } from '@angular/core';
import { RestApiService } from '../../../shared/services/rest-api.service';
import { Observable } from 'rxjs';
import { IUser } from '../models/user.model';
import { PATH } from '../../../shared/configs/path.config';
import { GetUserStats } from '../models/get-user-stats.interface';
import { IOwnerResult } from '../models/owner-result.interface';
import { GetOwnerStats } from '../models/get-owner-stats.interface';

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

  public getEmployeeById(employeeId: string): Observable<IUser | null> {
    return this.api.get<IUser>(
      `/${PATH.USER.CORE}/${PATH.USER.GET_EMPLOYEE_BY_ID}/${employeeId}`
    );
  }

  public getOwner(): Observable<IOwnerResult | null> {
    return this.api.get<IOwnerResult>(
      `/${PATH.USER.CORE}/${PATH.USER.GET_OWNER}`
    );
  }

  public deleteOwner(): Observable<null> {
    return this.api.delete(`/${PATH.USER.CORE}/${PATH.USER.DELETE_OWNER}`);
  }

  public getUserList(): Observable<IUser[] | null> {
    return this.api.get<IUser[]>(
      `/${PATH.USER.CORE}/${PATH.USER.GET_USER_LIST}`
    );
  }

  public getUserStats(): Observable<GetUserStats> {
    return this.api.get<GetUserStats>(
      `/${PATH.USER.CORE}/${PATH.USER.GET_USER_STATS}`
    );
  }

   public getOwnerStats(): Observable<GetOwnerStats> {
    return this.api.get<GetOwnerStats>(
      `/${PATH.USER.CORE}/${PATH.USER.GET_OWNER_STATS}`
    );
  }

  public createEmployee(
    firstName: string,
    lastName: string,
    email: string,
    phoneNumber: string
  ): Observable<null> {
    return this.api.post(`/${PATH.USER.CORE}/${PATH.USER.CREATE_EMPLOYEE}`, {
      firstName,
      lastName,
      email,
      phoneNumber,
    });
  }

  public createOwner(
    email: string,
    clubPhoneNumber: string,
    clubName: string
  ): Observable<null> {
    return this.api.post(`/${PATH.USER.CORE}/${PATH.USER.CREATE_OWNER}`, {
      email,
      clubPhoneNumber,
      clubName,
    });
  }

  public updateEmployee(
    id: string,
    firstName: string,
    lastName: string,
    email: string,
    phoneNumber: string
  ): Observable<null> {
    return this.api.put(`/${PATH.USER.CORE}/${PATH.USER.UPDATE_EMPLOYEE}`, {
      id,
      firstName,
      lastName,
      email,
      phoneNumber,
    });
  }

  public deleteEmployee(employeeId: string): Observable<null> {
    return this.api.delete(
      `/${PATH.USER.CORE}/${PATH.USER.DELETE_EMPLOYEE}/${employeeId}`
    );
  }
}

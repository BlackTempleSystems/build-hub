import { inject, Injectable } from '@angular/core';
import { BaseServerRequestService } from '@app/core/api/base-server-request-service';
import { BaseServerResponse } from '@app/core/api/base-server-response';
import { catchError, map, Observable, of, tap } from 'rxjs';
import { LoginRequest } from './models/login.request';
import { LoginResponse } from './models/login.response';
import { RegisterUserRequest } from './models/register-user.request';
import { RegisterUserResponse } from './models/register-user.response';
import { UserModel } from './models/user.model';
import { LocalStorageService } from '../local-storage-service/local-storage.service';

/**
 * 
 */
@Injectable({
  providedIn: 'root',
})
export class AuthenticationService extends BaseServerRequestService {

  private readonly _USER_STORAGE_KEY: string = "User";

  private _localStorageService = inject(LocalStorageService);
  private _userData?: UserModel;

  public get userData(): UserModel | undefined {
    return this._userData;
  }

  public constructor() {
    super();
  }

  protected override getServiceDomain(): string {
    return 'authentication';
  }

  public login(loginRequest: LoginRequest): Observable<BaseServerResponse<LoginResponse>> {
    return this.sendServerPostRequest<LoginRequest, LoginResponse>('login', loginRequest);
  }

  public register(registerUserRequest: RegisterUserRequest): Observable<BaseServerResponse<RegisterUserResponse>> {
    return this.sendServerPostRequest<RegisterUserRequest, RegisterUserResponse>('register', registerUserRequest);
  }

  private authenticateUser(): Observable<BaseServerResponse<UserModel>> {
    return this.sendServerGetRequest<UserModel>('authenticateUser');
  }

  public saveUser(user: UserModel): void {
    this._userData = user;
    this._localStorageService.setItem<UserModel>(this._USER_STORAGE_KEY, user);
  }

  public tryToAuthenticateUser(): Observable<void> {
    return this.authenticateUser().pipe(
      tap(authenticateUserResponse => this.saveUser(authenticateUserResponse.response!)),
      catchError(() => {
        //this.clearUser();
        return of(void 0);
      }),
      map(() => void 0)
    );
  }
}

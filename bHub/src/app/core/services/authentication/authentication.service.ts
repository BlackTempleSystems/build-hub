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
 * Authentication service
 */
@Injectable({
  providedIn: 'root',
})
export class AuthenticationService extends BaseServerRequestService {

  /** 
   * The key for storing the user in the local storage
   */
  private readonly _USER_STORAGE_KEY: string = "User";

  /**
   * Local storage service
   */
  private _localStorageService = inject(LocalStorageService);

  /**
   * User data memory
   */
  private _userData?: UserModel;

  /**
   * User data getter
   */
  public get userData(): UserModel | undefined {
    return this._userData;
  }

  public constructor() {
    super();
  }

  /**
   * 
   * @returns 
   */
  protected override getServiceDomain(): string {
    return 'authentication';
  }

  /**
   * Sends a login request to the server
   * @param loginRequest
   * @returns
   */
  public login(loginRequest: LoginRequest): Observable<BaseServerResponse<LoginResponse>> {
    return this.sendPostRequest<LoginRequest, LoginResponse>('login', loginRequest);
  }

  /**
   * Sends a register request to the server
   * @param registerUserRequest 
   * @returns 
   */
  public register(registerUserRequest: RegisterUserRequest): Observable<BaseServerResponse<RegisterUserResponse>> {
    return this.sendPostRequest<RegisterUserRequest, RegisterUserResponse>('register', registerUserRequest);
  }

  /**
   * Authenticates the user
   * @returns 
   */
  private authenticateUser(): Observable<BaseServerResponse<UserModel>> {
    return this.sendGetRequest<UserModel>('authenticateUser');
  }

  /**
   * Saves the user in the local storage and in memory
   * @param user 
   */
  public saveUser(user: UserModel): void {
    this._userData = user;
    this._localStorageService.setItem<UserModel>(this._USER_STORAGE_KEY, user);
  }

  /**
   * Clears the user from the local storage and from memory
   */
  public clearUser(): void {
    this._userData = undefined;
    this._localStorageService.removeItem(this._USER_STORAGE_KEY);
  }

  public tryToAuthenticateUser(): Observable<void> {
    return this.authenticateUser().pipe(
      tap(authenticateUserResponse => this.saveUser(authenticateUserResponse.resultData!)),
      catchError(() => {
        this.clearUser();
        return of(void 0);
      }),
      map(() => void 0)
    );
  }
}

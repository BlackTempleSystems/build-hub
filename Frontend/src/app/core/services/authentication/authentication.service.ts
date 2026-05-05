import { Injectable } from '@angular/core';
import { BaseServerRequestService } from '@app/core/api/base-server-request-service';
import { BaseServerResponse } from '@app/core/api/base-server-response';
import { Observable } from 'rxjs';
import { LoginRequest } from './models/login.request';
import { LoginResponse } from './models/login.response';
import { RegisterUserRequest } from './models/register-user.request';
import { RegisterUserResponse } from './models/register-user.response';

@Injectable({
  providedIn: 'root',
})
export class AuthenticationService extends BaseServerRequestService {

  public constructor() {
    super();
  }

  protected override getServiceDomain(): string {
    return 'authentication';
  }

  public login(loginRequest: LoginRequest): Observable<BaseServerResponse<LoginResponse>> {
    return this.sendServerRequest<LoginRequest, LoginResponse>('login', loginRequest);
  }

  public register(registerUserRequest: RegisterUserRequest): Observable<BaseServerResponse<RegisterUserResponse>> {
    return this.sendServerRequest<RegisterUserRequest, RegisterUserResponse>('register', registerUserRequest);
  }

}

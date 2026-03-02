import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class AuthenticationService {
  private token: string | null = 'dummy-token';

  public getToken(): string | null {
    return this.token;
  }

  public clear(): void {
    this.token = null;
  }
}

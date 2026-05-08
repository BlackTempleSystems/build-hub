import { Component, inject } from '@angular/core';
import { FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { PasswordModule } from 'primeng/password';
import { InputTextModule } from 'primeng/inputtext';
import { CheckboxModule } from 'primeng/checkbox';
import { FloatLabelModule } from 'primeng/floatlabel';
import { RouterLink } from '@angular/router';
import { BasePage } from '@app/core/directives/base-page/base-page';
import { AuthenticationService } from '@app/core/services/authentication/authentication.service';
import { LoginRequest } from '@app/core/services/authentication/models/login.request';

@Component({
  selector: 'bh-login-page',
  imports: [
    ButtonModule,
    ReactiveFormsModule,
    PasswordModule,
    InputTextModule,
    CheckboxModule,
    FloatLabelModule,
    RouterLink,
  ],
  templateUrl: './login.page.html',
  styleUrl: './login.page.css',
})
export class LoginPage extends BasePage {

  private _authenticationService = inject(AuthenticationService);

  public loginForm!: FormGroup;

  public constructor() {
    super();
  }

  protected validate(): boolean {

    if (this.loginForm.invalid) {
      this.showErrorToast('Please, enter your credentials.');
      this.loginForm.markAllAsTouched();
      return false;
    }

    return true;
  }

  override ngOnInit(): void {
    this.loginForm = this._formBuilder.group({
      email: ['', [Validators.required]],
      password: ['', [Validators.required]]
    });
  }

  public onLogin(): void {
    if (!this.validate())
      return;

    const loginRequest: LoginRequest =
    {
      email: this.loginForm.get('email')?.value,
      password: this.loginForm.get('password')?.value
    };

    this._authenticationService.login(loginRequest).subscribe({
      next: (loginResponse) => {
        if (loginResponse.response) {
          this.loginForm.reset();
          this._authenticationService.saveUser(loginResponse.response.user);
          this.redirectTo('/dashboard');
        }
      },
      error: (error) => {
      }
    });

  }
}

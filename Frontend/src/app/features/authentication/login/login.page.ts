import { Component, inject } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
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

  public loginForm: FormGroup;

  public constructor(private fb: FormBuilder,) {
    super();

    this.loginForm = this.fb.group({
      userName: ['', [Validators.required]],
      userPassword: ['', [Validators.required]]
    });
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
    this.showErrorToast('Please, enter your credentials.');
  }

  public onLogin(): void {
    if (!this.validate())
      return;

    let loginRequest = new LoginRequest();
    loginRequest.userName = this.loginForm.get('userName')?.value;
    loginRequest.userPassword = this.loginForm.get('userPassword')?.value;

    this._authenticationService.login(this.loginForm.value).subscribe({
      next: (response) => {
        console.log(response);
      },
      error: (error) => {
        console.log(error);
      }
    });

    this.loginForm.reset();
  }
}

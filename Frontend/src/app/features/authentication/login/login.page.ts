import { Component, inject } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { PasswordModule } from 'primeng/password';
import { InputTextModule } from 'primeng/inputtext';
import { CheckboxModule } from 'primeng/checkbox';
import { FloatLabelModule } from 'primeng/floatlabel';
import { RouterLink } from '@angular/router';
import { BasePage } from '@app/core/directives/base-page/base-page';
import { Toast } from "primeng/toast";
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
    Toast
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
      userName: ['', [Validators.required]],
      password: ['', [Validators.required]]
    });
  }

  public onLogin(): void {
    if (!this.validate())
      return;

    // let loginRequest = new LoginRequest();
    // loginRequest.userName = this.loginForm.get('userName')?.value;
    // loginRequest.userPassword = this.loginForm.get('password')?.value;

    // this._authenticationService.login(this.loginForm.value).subscribe({
    //   next: (response) => {
    //     console.log(response);
    //   },
    //   error: (error) => {
    //     console.log(error);
    //   }
    // });

    this.loginForm.reset();
  }
}

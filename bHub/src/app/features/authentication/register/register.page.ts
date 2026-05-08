import { Component, inject } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { PasswordModule } from 'primeng/password';
import { InputTextModule } from 'primeng/inputtext';
import { CheckboxModule } from 'primeng/checkbox';
import { FloatLabelModule } from 'primeng/floatlabel';
import { AuthenticationService, ToastService } from '@app/core';
import { BasePage } from '@app/core/directives/base-page/base-page';
import { RegisterUserRequest } from '@app/core/services/authentication/models/register-user.request';
@Component({
  selector: 'bh-register-page',
  imports: [
    ButtonModule,
    ReactiveFormsModule,
    PasswordModule,
    InputTextModule,
    CheckboxModule,
    FloatLabelModule,
  ],
  templateUrl: './register.page.html',
  styleUrl: './register.page.css',
})

export class RegisterPage extends BasePage {

  private _authenticationService = inject(AuthenticationService);

  public registerForm!: FormGroup;

  public constructor() {
    super();
  }
  override ngOnInit(): void {

    this.registerForm = this._formBuilder.group({
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      userName: ['', [Validators.required, Validators.minLength(3)]],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(8)]],
      confirmPassword: ['', Validators.required]
    },);
  }

  protected override validate(): boolean {
    if (this.registerForm?.invalid) {
      this.registerForm.markAllAsTouched();

      return false;
    }

    return true;
  }


  onRegister(): void {
    if (!this.validate())
      return;

    const registerUserRequest: RegisterUserRequest = {
      firstName: this.registerForm.value.firstName!,
      lastName: this.registerForm.value.lastName!,
      userName: this.registerForm.value.userName!,
      email: this.registerForm.value.email!,
      password: this.registerForm.value.password!,
      confirmedPassword: this.registerForm.value.confirmPassword!
    };

    this._authenticationService.register(registerUserRequest).subscribe({
      next: (response) => {
        if (response.resultData) {
          this.registerForm.reset();
          this._authenticationService.saveUser(response.resultData.user);
          this.redirectTo('/dashboard');
        }
      },
      error: (error) => {
      }
    });

    this.registerForm.reset();
  }
}

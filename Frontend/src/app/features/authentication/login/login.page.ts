import { Component } from '@angular/core';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { PasswordModule } from 'primeng/password';
import { InputTextModule } from 'primeng/inputtext';
import { CheckboxModule } from 'primeng/checkbox';
import { FloatLabelModule } from 'primeng/floatlabel';
import { Toast } from 'primeng/toast';
import { ToastService } from '@app/core';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'bh-login-page',
  imports: [
    ButtonModule,
    ReactiveFormsModule,
    PasswordModule,
    InputTextModule,
    CheckboxModule,
    FloatLabelModule,
    Toast,
    RouterLink 
  ],
  templateUrl: './login.page.html',
  styleUrl: './login.page.css',
})
export class LoginPage {
  public userName: FormControl<string | null>;
  public password: FormControl<string | null>;

  public constructor(private toastService: ToastService) {
    this.userName = new FormControl<string>('');
    this.password = new FormControl<string>('');
  }

  private validate(): boolean {
    return true;
  }

  public onLogin(): void {
    this.validate();
  }
}

import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { PasswordModule } from 'primeng/password';
import { InputTextModule } from 'primeng/inputtext';
import { CheckboxModule } from 'primeng/checkbox';
import { FloatLabelModule } from 'primeng/floatlabel';
import { Toast } from 'primeng/toast';
import { ToastService } from '@app/core';

@Component({
  selector: 'bh-login-page',
  imports: [
    ButtonModule,
    FormsModule,
    PasswordModule,
    InputTextModule,
    CheckboxModule,
    FloatLabelModule,
    Toast,
  ],
  templateUrl: './login.page.html',
  styleUrl: './login.page.css',
})
export class LoginPage {
  public userName: string;
  public password: string;

  public constructor(private toastService: ToastService) {
    this.userName = '';
    this.password = '';
  }

  private validate(): boolean {
    if (this.userName.length <= 0) {
      this.toastService.showErrorToast('Username is empty');
      return false;
    }
    return true;
  }

  public onLogin(): void {
    this.validate();
  }
}

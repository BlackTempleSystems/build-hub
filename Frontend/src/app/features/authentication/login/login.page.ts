import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { PasswordModule } from 'primeng/password';
import { InputTextModule } from 'primeng/inputtext';
import { CheckboxModule } from 'primeng/checkbox';
import { FloatLabelModule } from 'primeng/floatlabel';
import { ToastModule } from 'primeng/toast';
import { MessageService } from 'primeng/api';

@Component({
  selector: 'bh-login-page',
  imports: [
    ButtonModule,
    FormsModule,
    PasswordModule,
    InputTextModule,
    CheckboxModule,
    FloatLabelModule,
    ToastModule,
  ],
  providers: [MessageService],
  templateUrl: './login.page.html',
  styleUrl: './login.page.css',
})
export class LoginPage {
  public userName: string;
  public password: string;

  public constructor(private toastMessageService: MessageService) {
    this.userName = '';
    this.password = '';
  }

  private validate(): boolean {
    if (this.userName.length <= 0) {
      this.toastMessageService.add({
        severity: 'error',
        summary: 'Error',
        detail: 'Message Content',
      });
      return false;
    }
    return true;
  }

  public onLogin(): void {
    this.validate();
  }
}

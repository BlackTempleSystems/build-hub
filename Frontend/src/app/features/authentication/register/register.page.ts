import { Component } from '@angular/core';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { PasswordModule } from 'primeng/password';
import { InputTextModule } from 'primeng/inputtext';
import { CheckboxModule } from 'primeng/checkbox';
import { FloatLabelModule } from 'primeng/floatlabel';
import { Toast } from 'primeng/toast';
import { ToastService } from '@app/core';
@Component({
  selector: 'bh-register-page',
  imports: [
      ButtonModule,
      ReactiveFormsModule,
      PasswordModule,
      InputTextModule,
      CheckboxModule,
      FloatLabelModule,
      Toast,
    ],
  templateUrl: './register.page.html',
  styleUrl: './register.page.css',
})
export class RegisterPage {
   public userName: FormControl<string | null>;
   public password: FormControl<string | null>;
   public confirmPassword: FormControl<string | null>;

    public constructor(private toastService: ToastService) {
    this.userName = new FormControl<string>('');
    this.password = new FormControl<string>('');
    this.confirmPassword = new FormControl<string>('');
  }
}

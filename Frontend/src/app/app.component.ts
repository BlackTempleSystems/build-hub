import { Component } from '@angular/core';
import { LoginPage } from './features/authentication/login/login.page';

@Component({
  selector: 'bh-root',
  imports: [LoginPage],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css',
})
export class AppComponent {
  public title = 'BuildHub Frontend';
}

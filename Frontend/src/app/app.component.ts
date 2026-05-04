import { Component } from '@angular/core';
import { RouterOutlet } from "@angular/router";
import { Toast } from "primeng/toast";

@Component({
  selector: 'bh-root',
  imports: [RouterOutlet, Toast],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css',
})
export class AppComponent {
  public title = 'BuildHub Frontend';
}

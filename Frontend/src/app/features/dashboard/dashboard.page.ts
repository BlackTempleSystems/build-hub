import { Component } from '@angular/core';
import { PageHeader } from "../../layouts/page-header/page-header";
import { TabsModule } from 'primeng/tabs';

@Component({
  selector: 'bh-dashboard-page',
  imports: [PageHeader, TabsModule],
  templateUrl: './dashboard.page.html',
  styleUrl: './dashboard.page.css',
})
export class DashboardPage {

}

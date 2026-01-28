import { Component, Input,  } from '@angular/core';

@Component({
  selector: 'bh-page-header',
  imports: [],
  templateUrl: './page-header.html',
  styleUrl: './page-header.css',
})
export class PageHeader {
  @Input() public title = '';
  @Input() public description = '';
}

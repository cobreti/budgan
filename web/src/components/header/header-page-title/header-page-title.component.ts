import { Component, Input } from '@angular/core';
import { PageService } from '@services/page.service';

@Component({
  selector: 'app-header-page-title',
  templateUrl: './header-page-title.component.html',
  styleUrls: ['./header-page-title.component.scss']
})
export class HeaderPageTitleComponent {

  constructor(public pageService: PageService) {}
}

import { PageBodyComponent } from '@/components/page-body/page-body.component';
import { PageComponent } from '@/components/page/page.component';
import { Component } from '@angular/core';
import { TranslatePipe } from '@ngx-translate/core';

@Component({
  selector: 'app-accounts-overview',
  templateUrl: './accounts-overview.html',
  styleUrl: './accounts-overview.scss',
  imports: [TranslatePipe, PageComponent, PageBodyComponent],
})
export class AccountOverviewComponent {}

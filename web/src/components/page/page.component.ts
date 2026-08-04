import { ChangeDetectionStrategy, Component, effect, inject, input } from '@angular/core';
import { PageService } from '@services/page.service';

@Component({
  selector: 'app-page',
  templateUrl: './page.component.html',
  styleUrl: './page.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class PageComponent {
  private readonly pageService = inject(PageService);

  title = input<string>('');

  constructor() {
    effect(() => {
      this.pageService.setTitle(this.title());
    });
  }
}

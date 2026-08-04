import { ChangeDetectionStrategy, Component } from '@angular/core';
import { MatCard } from '@angular/material/card';

@Component({
  selector: 'app-page-menu',
  templateUrl: './page-menu.component.html',
  styleUrl: './page-menu.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [MatCard],
})
export class PageMenuComponent {}

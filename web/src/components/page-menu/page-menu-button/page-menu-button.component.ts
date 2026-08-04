import { ChangeDetectionStrategy, Component, input, output } from '@angular/core';
import { MatIcon } from '@angular/material/icon';

@Component({
  selector: 'app-page-menu-button',
  templateUrl: './page-menu-button.component.html',
  styleUrl: './page-menu-button.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: {
    '[class.page-menu-button--danger]': 'variant() === "danger"',
    '[class.page-menu-button--disabled]': 'disabled()',
  },
  imports: [MatIcon],
})
export class PageMenuButtonComponent {
  icon = input.required<string>();
  label = input.required<string>();
  variant = input<'default' | 'danger'>('default');
  disabled = input<boolean>(false);
  clicked = output<void>();
}

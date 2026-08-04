import { ChangeDetectionStrategy, Component, input, output } from '@angular/core';
import { MatIcon } from '@angular/material/icon';

@Component({
  selector: 'app-main-menu-button',
  templateUrl: './main-menu-button.component.html',
  styleUrl: './main-menu-button.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [MatIcon],
})
export class MainMenuButtonComponent {
  icon = input.required<string>();
  label = input.required<string>();
  clicked = output<void>();
}

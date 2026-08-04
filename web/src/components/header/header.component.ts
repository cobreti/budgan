import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { NgOptimizedImage } from '@angular/common';
import { Router } from '@angular/router';
import { MatToolbar } from '@angular/material/toolbar';
import { MatIconButton } from '@angular/material/button';
import { MatIcon } from '@angular/material/icon';
import { MatMenu, MatMenuItem, MatMenuTrigger } from '@angular/material/menu';
import { MatTooltip } from '@angular/material/tooltip';
import { TranslatePipe } from '@ngx-translate/core';
import { MainMenuService } from '@services/main-menu.service';
import { LOCALE_SERVICE } from '@services/locale.service';
import { THEME_SERVICE } from '@services/theme.service';
import { HeaderPageTitleComponent } from '@components/header/header-page-title/header-page-title.component';
import { environment } from '../../environments/environment';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrl: './header.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [MatToolbar, HeaderPageTitleComponent, MatIconButton, MatIcon, MatMenu, MatMenuItem, MatMenuTrigger, MatTooltip, TranslatePipe, NgOptimizedImage],
})
export class HeaderComponent {
  private readonly _mainMenuService = inject(MainMenuService);
  private readonly _localeService = inject(LOCALE_SERVICE);
  private readonly _router = inject(Router);
  private readonly _themeService = inject(THEME_SERVICE);
  protected readonly isDark = this._themeService.isDark;
  protected readonly buildType = environment.buildType;
  protected readonly currentLocale = this._localeService.currentLocale;

  onMenuBtnClick(): void {
    this._mainMenuService.toggleMenu();
  }

  switchLocale(locale: string): void {
    const urlTree = this._router.parseUrl(this._router.url);
    const segments = urlTree.root.children['primary']?.segments ?? [];
    const remaining = segments.slice(1).map((s) => s.path);
    this._router.navigate([locale, ...remaining], { queryParams: urlTree.queryParams });
  }

  async onHome(): Promise<void> {
    await this._router.navigate([this._localeService.currentLocale()]);
  }

  toggleTheme(): void {
    this._themeService.toggle();
  }
}

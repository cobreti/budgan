import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import {
  MAT_DIALOG_DATA,
  MatDialogActions,
  MatDialogContent,
  MatDialogTitle,
  MatDialogRef,
} from '@angular/material/dialog';
import { MatButton, MatIconButton } from '@angular/material/button';
import { MatIcon } from '@angular/material/icon';
import { MatListModule } from '@angular/material/list';
import { MatCheckbox } from '@angular/material/checkbox';
import { TranslatePipe } from '@ngx-translate/core';
import {
  DEMO_STATEMENTS_SERVICE,
  DemoStatementFolder,
  DemoStatementsService,
} from '@services/demo-statements.service';

export type DemoStatementPickerData = {
  multiple: boolean
}

@Component({
  selector: 'app-demo-statement-picker',
  templateUrl: './demo-statement-picker.component.html',
  styleUrl: './demo-statement-picker.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [
    MatDialogTitle,
    MatDialogContent,
    MatDialogActions,
    MatButton,
    MatIconButton,
    MatIcon,
    MatListModule,
    MatCheckbox,
    TranslatePipe,
  ],
})
export class DemoStatementPickerComponent {
  private readonly _dialogRef = inject(MatDialogRef<DemoStatementPickerComponent, File[]>);
  private readonly _demoStatements = inject<DemoStatementsService>(DEMO_STATEMENTS_SERVICE);
  readonly data = inject<DemoStatementPickerData>(MAT_DIALOG_DATA);

  readonly folders = signal<DemoStatementFolder[]>([]);
  readonly selectedFolder = signal<DemoStatementFolder | null>(null);
  readonly selectedFilenames = signal<Set<string>>(new Set());
  readonly loadingFolders = signal<boolean>(true);
  readonly confirming = signal<boolean>(false);
  readonly errorKey = signal<string | null>(null);

  constructor() {
    this.loadFolders();
  }

  private async loadFolders(): Promise<void> {
    this.loadingFolders.set(true);
    const result = await this._demoStatements.listFolders();
    this.loadingFolders.set(false);

    if (!result.success) {
      this.errorKey.set('demoStatementPicker.manifestLoadError');
      return;
    }
    this.folders.set(result.value);
  }

  onSelectFolder(folder: DemoStatementFolder): void {
    this.selectedFolder.set(folder);
    this.selectedFilenames.set(new Set());
    this.errorKey.set(null);
  }

  onBackToFolders(): void {
    this.selectedFolder.set(null);
    this.selectedFilenames.set(new Set());
    this.errorKey.set(null);
  }

  isSelected(filename: string): boolean {
    return this.selectedFilenames().has(filename);
  }

  onToggleFile(filename: string): void {
    if (!this.data.multiple) {
      this.selectedFilenames.set(this.isSelected(filename) ? new Set() : new Set([filename]));
      return;
    }

    this.selectedFilenames.update((current) => {
      const next = new Set(current);
      if (next.has(filename)) {
        next.delete(filename);
      } else {
        next.add(filename);
      }
      return next;
    });
  }

  onCancel(): void {
    this._dialogRef.close(undefined);
  }

  async onConfirm(): Promise<void> {
    const folder = this.selectedFolder();
    const filenames = Array.from(this.selectedFilenames());
    if (!folder || filenames.length === 0) return;

    this.confirming.set(true);
    this.errorKey.set(null);
    const result = await this._demoStatements.loadFiles(folder.id, filenames);
    this.confirming.set(false);

    if (!result.success) {
      this.errorKey.set('demoStatementPicker.loadError');
      return;
    }

    this._dialogRef.close(result.value);
  }
}

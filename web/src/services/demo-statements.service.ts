import { Injectable, InjectionToken, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { Result } from '@app-types/result';

export type DemoStatementFolder = {
  id: string
  labelKey: string
  files: string[]
}

type DemoStatementManifest = {
  version: number
  folders: DemoStatementFolder[]
}

export interface DemoStatementsService {
  listFolders(): Promise<Result<DemoStatementFolder[]>>
  loadFiles(folderId: string, filenames: string[]): Promise<Result<File[]>>
}

export const DEMO_STATEMENTS_SERVICE = new InjectionToken<DemoStatementsService>('DemoStatementsService')

const MANIFEST_URL = '/assets/samples/demo-statements/manifest.json'
const BASE_URL = '/assets/samples/demo-statements'

@Injectable({ providedIn: 'root' })
export class DemoStatementsServiceImpl implements DemoStatementsService {
  private readonly _http = inject(HttpClient)
  private _manifest: DemoStatementManifest | null = null

  async listFolders(): Promise<Result<DemoStatementFolder[]>> {
    const manifestResult = await this.loadManifest()
    if (!manifestResult.success) {
      return manifestResult
    }
    return { success: true, value: manifestResult.value.folders }
  }

  async loadFiles(folderId: string, filenames: string[]): Promise<Result<File[]>> {
    const manifestResult = await this.loadManifest()
    if (!manifestResult.success) {
      return manifestResult
    }

    const folder = manifestResult.value.folders.find((f) => f.id === folderId)
    if (!folder) {
      return { success: false, error: 'folder-not-found' }
    }

    try {
      const files = await Promise.all(
        filenames.map((filename) => this.loadFile(folder.id, filename)),
      )
      return { success: true, value: files }
    } catch {
      return { success: false, error: 'demo-file-load-error' }
    }
  }

  private async loadFile(folderId: string, filename: string): Promise<File> {
    const url = `${BASE_URL}/${folderId}/${encodeURIComponent(filename)}`
    const text = await firstValueFrom(this._http.get(url, { responseType: 'text' }))
    return new File([text], filename, { type: 'text/csv' })
  }

  private async loadManifest(): Promise<Result<DemoStatementManifest>> {
    if (this._manifest) {
      return { success: true, value: this._manifest }
    }

    try {
      const manifest = await firstValueFrom(
        this._http.get<DemoStatementManifest>(MANIFEST_URL),
      )
      this._manifest = manifest
      return { success: true, value: manifest }
    } catch {
      return { success: false, error: 'manifest-load-error' }
    }
  }
}

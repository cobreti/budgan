import { InjectionToken } from '@angular/core';
import { AllDataExportPayload } from '@services/budgan-export.service';

export interface ImportSummary {
  skipped: number;
  errors: number;
}

export interface ImportService {
  importAllData(payload: AllDataExportPayload): Promise<ImportSummary>;
}

export const IMPORT_SERVICE = new InjectionToken<ImportService>('ImportService');

import { Injectable, InjectionToken } from '@angular/core';


export interface IdGeneratorService {
  generateId(): string;
}

export const ID_GENERATOR_SERVICE = new InjectionToken<IdGeneratorService>('IdGeneratorService');



@Injectable({
  providedIn: 'root',
})
export class IdGeneratorServiceImpl implements IdGeneratorService {
  generateId(): string {
    return crypto.randomUUID();
  }
}

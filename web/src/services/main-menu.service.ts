import { Injectable, Signal, signal, WritableSignal } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class MainMenuService {

  private _isOpen: WritableSignal<boolean> = signal(false);

  public get isOpen(): Signal<boolean> { return this._isOpen; }

  constructor() { }

  toggleMenu() {
    this._isOpen.update(value => !value);
  }

  close(): void {
    this._isOpen.set(false);
  }
}

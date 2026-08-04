import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';

@Injectable({ providedIn: 'root' })
export class BdgHttpClient {
  constructor(private httpClient: HttpClient) {}

  public get<TYPE>(url: string): Promise<TYPE> {
    return new Promise((resolve, reject) => {
      this.httpClient.get<TYPE>(url).subscribe({
        next: (res) => resolve(res),
        error: (err: HttpErrorResponse) => this._handleError(err, resolve, reject),
      });
    });
  }

  public post<TYPE>(url: string, body: any): Promise<TYPE> {
    return new Promise((resolve, reject) => {
      this.httpClient.post<TYPE>(url, body).subscribe({
        next: (res) => resolve(res),
        error: (err: HttpErrorResponse) => this._handleError(err, resolve, reject),
      });
    });
  }

  delete<TYPE = void>(url: string): Promise<TYPE> {
    return new Promise((resolve, reject) => {
      this.httpClient.delete<TYPE>(url).subscribe({
        next: (res) => resolve(res),
        error: (err: HttpErrorResponse) => this._handleError(err, resolve, reject),
      });
    });
  }

  private _handleError<TYPE>(
    err: HttpErrorResponse,
    resolve: (value: TYPE) => void,
    reject: (reason: HttpErrorResponse) => void,
  ): void {
    if (err.error && typeof err.error === 'object' && 'succeeded' in err.error) {
      resolve(err.error as TYPE);
    } else {
      reject(err);
    }
  }
}


export interface ApiResult<T> {
  succeeded: boolean;
  successValue: T;
  errorValue?: any;
}

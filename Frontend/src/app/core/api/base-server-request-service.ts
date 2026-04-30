import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../../environments/environment.development';
import { catchError, Observable, throwError } from 'rxjs';
import { BaseServerResponse } from './base-server-response';

@Injectable()
export abstract class BaseServerRequestService {

    private _httpClient: HttpClient = inject(HttpClient);

    protected constructor() {
    }

    protected abstract getServiceDomain(): string;

    protected sendServerRequest<Request, Response>(
        serviceRoute: string, request: Request): Observable<BaseServerResponse<Response>> {
        this.sendServerRequest
        return this._httpClient
            .post<BaseServerResponse<Response>>(
                this.constructFullRequestURL(serviceRoute),
                request
            )
            .pipe(catchError(this.handleError))
    }

    protected handleError(error: HttpErrorResponse) {
        if (error.status === 0) {
            console.error('An error occurred:', error.error);
        } else {
            console.error(
                `Backend returned code ${error.status}, body was: `,
                error.error
            );
        }
        return throwError(
            () => new Error('Something bad happened; please try again later.')
        );
    }

    private constructFullRequestURL(serviceRoute: string): string {
        return `${environment.backendUrl}/${this.getServiceDomain()}/${serviceRoute}`;
    }
} 
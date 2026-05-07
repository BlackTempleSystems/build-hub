import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../../environments/environment.development';
import { catchError, Observable, throwError } from 'rxjs';
import { BaseServerResponse } from './base-server-response';
import { ToastService } from '../services';

/** 
 * A base class for sending http requests
*/
@Injectable()
export abstract class BaseServerRequestService {

    /**Angular http client  */
    private _httpClient: HttpClient = inject(HttpClient);
    /** Toast service reference*/
    private _toastService: ToastService = inject(ToastService);

    protected constructor() {
    }

    /**The domain of the request (authentication, build, etc.) */
    protected abstract getServiceDomain(): string;

    /**
     * Sends an http request to the server returning and observable
     *  with response template parameter
     * @param serviceRoute the path of the request
     * @param request request input model
     * @returns An observable with the response.
     */
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

    /**
     * Handles errors related with http requests.
     * @param httpErrorResponse 
     * @returns 
     */
    protected handleError(httpErrorResponse: HttpErrorResponse) {

        //TODO LOG error
        return throwError(
            () => new Error('Something bad happened; please try again later.')
        );
    }

    /**
     * Creates the full request path.
     * @param serviceRoute route of the service.
     * @returns The full request path
     */
    private constructFullRequestURL(serviceRoute: string): string {
        return `${environment.backendUrl}/${this.getServiceDomain()}/${serviceRoute}`;
    }
} 
import { HttpHandler, HttpInterceptor, HttpRequest } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { catchError, Observable, switchMap, throwError } from "rxjs";

/**
 * An access token interceptor. It sends the token via http only cookie to the server.
 */
@Injectable()
export class AccessTokenInterceptor implements HttpInterceptor {

    intercept(request: HttpRequest<any>, next: HttpHandler) {
        return next.handle(request.clone({ withCredentials: true }))
    }
}
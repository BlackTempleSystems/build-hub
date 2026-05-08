import { Directive, inject, OnInit } from "@angular/core";
import { FormBuilder } from "@angular/forms";
import { ToastService } from "@app/core/services";
import { Router } from "@angular/router"

/**
 * Base page directive aiming to give most pages what it needs, and to reduce imports and unnecessary 
 * or boilerplate logic */
@Directive()
export abstract class BasePage implements OnInit {

    /**Toast service instance */
    private _toastService: ToastService = inject(ToastService);

    /**
     * Form builder instance.
     */
    protected _formBuilder: FormBuilder = inject(FormBuilder);

    /**
     * Router
     */
    private _router: Router = inject(Router);

    protected constructor() {
    }

    /**
     * On init functions for control initialization logic.
     */
    abstract ngOnInit(): void;

    /**
     * Validates the form or component state before submitting a request.
     */
    protected abstract validate(): boolean;

    /**
     * Redirects to a given path.
     * @param path where to redirect to.
     */
    protected redirectTo(path: string): void {
        this._router.navigate([path]);
    }

    /**
     * shows and error message toast.
     * @param messageContent content of the error message.
     */
    protected showErrorToast(messageContent: string) {
        this._toastService.showErrorToast(messageContent);
    }
}
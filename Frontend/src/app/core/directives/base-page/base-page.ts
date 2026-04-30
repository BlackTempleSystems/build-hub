import { Directive, inject, OnInit } from "@angular/core";
import { ToastService } from "@app/core/services";
import { Toast } from "primeng/toast";

@Directive()
export abstract class BasePage implements OnInit {

    private _toastService: ToastService = inject(ToastService);

    protected constructor() {
    }

    abstract ngOnInit(): void;

    protected abstract validate(): boolean; 

    protected showErrorToast(messageContent: string){
        this._toastService.showErrorToast(messageContent);
    }
}
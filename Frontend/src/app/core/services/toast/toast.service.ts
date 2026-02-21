import { inject, Injectable } from '@angular/core';
import { MessageService } from 'primeng/api';

@Injectable({
  providedIn: 'root',
})
export class ToastService {
  private messageService: MessageService = inject(MessageService);

  public showErrorToast(messageContent: string, isSticky: boolean = false) {
    this.messageService.add({
      severity: 'error',
      summary: 'Error',
      detail: messageContent,
      sticky: isSticky,
    });
  }
}

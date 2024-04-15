import { Injectable } from '@angular/core';
import { ControllerService } from './controller.service';
import { ModelService } from './model.service';
import { Message } from './objects';

@Injectable()
export class DashboardService {

  constructor(private controller: ControllerService, public model: ModelService) {

    controller.getMessage().subscribe((message: Message) => {
      if (message.action === 'GetReport' || message.action === 'OpenReport') {
        setTimeout(() => {
          if (this.model.reportParams.autoZoom) {

          }
        });
      }

    });
  }

}

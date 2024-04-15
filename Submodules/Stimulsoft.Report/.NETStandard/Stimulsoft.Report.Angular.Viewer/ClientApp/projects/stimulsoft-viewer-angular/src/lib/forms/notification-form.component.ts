import { Component, OnInit } from '@angular/core';
import { ModelService } from '../services/model.service';
import { FormService } from './form.service';

@Component({
  selector: 'sti-notification-form',
  template: `
  <sti-base-form [fontFamily]="model.options.toolbar.fontFamily"
      [color]="model.options.toolbar.fontColor"
      [name]="'notificationForm'"
      [defaultTop]="150"
      [caption]="model.loc('Viewer')"
      [level]="4"
      [showButtons]="false"
      [showSeparator]="false"
      (changeVisibility)="changeVisibility($event)">
      <table class="stiJsViewerClearAllStyles" cellpadding="0" cellspacing="0">
        <tbody>
          <tr *ngIf="model.notificationFormOptions?.image" class="stiJsViewerClearAllStyles">
            <td class="stiJsViewerClearAllStyles" style="text-align: center;">
              <img src="{{model.img(model.notificationFormOptions?.image)}}" style="margin-top: 20px; width: 112px; height: 112px;">
            </td>
          </tr>
          <tr *ngIf="model.notificationFormOptions?.message" class="stiJsViewerClearAllStyles">
            <td class="stiJsViewerClearAllStyles">
              <div class="stiJsViewerNotificationFormMessage">
                {{model.notificationFormOptions?.message}}
              </div>
            </td>
          </tr>
          <tr *ngIf="model.notificationFormOptions?.description" class="stiJsViewerClearAllStyles">
            <td class="stiJsViewerClearAllStyles">
              <div class="stiJsViewerNotificationFormDescription">
                {{model.notificationFormOptions?.description}}
              </div>
            </td>
          </tr>
          <tr class="stiJsViewerClearAllStyles">
            <td class="stiJsViewerClearAllStyles" style="text-align: center;">
              <sti-button
                [caption]="model.notificationFormOptions?.buttonCaption"
                [styleName]="'stiJsViewerLoginButton'"
                [innerTableWidth]="'100%'"
                [height]="'40px'"
                [minWidth]="'80px'"
                [captionAlign]="'center'"
                [cursor]="'pointer'"
                [display]="'inline-block'"
                [minWidth]="'200px'"
                [width]="'auto'"
                [margin]="'20px 30px 30px 30px'"
                [fontSize]="'14px'"
                (action)="action()">
              </sti-button>
            </td>
          </tr>
        </tbody>
      </table>
    </sti-base-form>
    `
})

export class NotificationFormComponent implements OnInit {

  constructor(public model: ModelService, public formService: FormService) { }

  ngOnInit() { }

  changeVisibility(state: string) {
    if (state === 'hidden' && this.model.notificationFormOptions?.cancelAction) {
      this.model.notificationFormOptions.cancelAction();
    }
  }

  action() {
    this.model.notificationFormOptions?.action ? this.model.notificationFormOptions.action() : this.formService.closeForm('notificationForm');
  }
}

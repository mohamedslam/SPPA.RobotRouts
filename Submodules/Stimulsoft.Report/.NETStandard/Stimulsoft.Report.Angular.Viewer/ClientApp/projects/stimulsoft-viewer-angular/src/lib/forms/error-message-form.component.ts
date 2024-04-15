import { Component, OnInit } from '@angular/core';
import { ModelService } from '../services/model.service';
import { FormService } from './form.service';

@Component({
  selector: 'sti-error-message-form',
  template: `
    <sti-base-form [fontFamily]="model.options.toolbar.fontFamily"
      [color]="model.options.toolbar.fontColor"
      [name]="'errorMessageForm'"
      [caption]="model.loc(model.errorMessage?.type == 'Error' ? 'Error' : 'FormViewerTitle')"
      [level]="4"
      [defaultTop]="150"
      [showCancel]="false"
      (action)="formService.closeForm('errorMessageForm')">
      <table class="stiJsViewerClearAllStyles" cellpadding="0" cellspacing="0">
        <tbody>
          <tr class="stiJsViewerClearAllStyles">
            <td class="stiJsViewerClearAllStyles">
              <img [attr.src]="getImage()" style="padding: 15px; height: 32px; width: 32px;">
            </td>
            <td class="stiJsViewerMessagesFormDescription" style="max-width: 600px;"
              [style.color]="model.options.toolbar.fontColor" [innerHTML]="model.errorMessage?.error">
            </td>
          </tr>
        </tbody>
      </table>
    </sti-base-form>
  `
})

export class ErrorMessageFormComponent implements OnInit {

  constructor(public model: ModelService, public formService: FormService) { }

  ngOnInit() { }

  getImage() {
    const messageType = this.model.errorMessage?.type;

    if (messageType === 'Warning') {
      return this.model.img('MsgFormWarning.png');
    } else if (messageType === true || messageType === 'Info') {
      return this.model.img('MsgFormInfo.png');
    } else {
      return this.model.img('MsgFormError.png');
    }
  }
}

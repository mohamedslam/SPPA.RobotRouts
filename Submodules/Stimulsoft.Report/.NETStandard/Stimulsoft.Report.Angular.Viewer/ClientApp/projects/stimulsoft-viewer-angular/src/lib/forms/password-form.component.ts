import { Component, OnInit, ViewChild } from '@angular/core';
import { ModelService } from '../services/model.service';
import { ControllerService } from '../services/controller.service';
import { FormService } from './form.service';
import { TextBoxComponent } from '../controls/text-box.componet';

@Component({
  selector: 'sti-password-form',
  template: `
    <sti-base-form [fontFamily]="model.options.toolbar.fontFamily"
      [color]="model.options.toolbar.fontColor"
      [name]="'passwordForm'"
      [caption]="model.loc('PasswordSaveReport').replace(':', '')"
      [level]="2"
      [defaultTop]="150"
      (changeVisibility)="changeVisibility($event)"
      (action)="action()">
      <table class="stiJsViewerClearAllStyles" cellpadding="0" cellspacing="0" style="margin: 5px;">
        <tbody>
          <tr class="stiJsViewerClearAllStyles">
            <td class="stiJsViewerCaptionControls">
              {{model.loc('PasswordEnter')}}
            </td>
            <td class="stiJsViewerCaptionControls">
              <sti-text-box #textBox [type]="'password'" [width]="200" [focusOnCreate]="true" [value]="password" (onchange)="password = $event.value">
              </sti-text-box>
            </td>
          </tr>
        </tbody>
      </table>
    </sti-base-form>
  `
})

export class PasswordFormComponent implements OnInit {

  @ViewChild('textBox') textBox: TextBoxComponent;

  password: string;

  constructor(public model: ModelService, public controller: ControllerService, public formService: FormService) { }

  ngOnInit() { }

  changeVisibility(state: string) {
    if (state === 'visible') {
      this.password = '';
      this.textBox.element.nativeElement.focus();
    }
  }

  action() {
    const data = this.formService.form.formData;
    data.openingFilePassword = this.password;
    this.formService.closeForm('passwordForm');

    this.controller.postOpen(data);
  }
}

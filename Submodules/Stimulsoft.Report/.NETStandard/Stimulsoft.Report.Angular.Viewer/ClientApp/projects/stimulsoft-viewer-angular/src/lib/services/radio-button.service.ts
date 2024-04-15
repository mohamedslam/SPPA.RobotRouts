import { Injectable } from '@angular/core';
import { RadioButtonComponent } from '../controls/radio-button.components';

@Injectable()
export class RadioButtonService {

  groups = {};

  constructor() { }

  public addButton(button: RadioButtonComponent) {
    if (!this.groups[button.groupName]) {
      this.groups[button.groupName] = [];
    }
    this.groups[button.groupName].push(button);
  }

  public removeButton(button: RadioButtonComponent) {
    const buttons = this.groups[button.groupName] as RadioButtonComponent[];
    buttons?.splice(buttons.indexOf(button), 1);
  }

  public check(button: RadioButtonComponent) {
    const buttons = this.groups[button.groupName] as RadioButtonComponent[];
    buttons?.filter(b => b.name !== button.name).forEach(b => b.uncheck());
  }

}

import { Component, OnInit, Input, ViewChild } from '@angular/core';
import { Variable } from '../../services/objects';
import { TextBoxComponent } from '../text-box.componet';
import { HelperService } from '../../services/helper.service';
import { ModelService } from '../../services/model.service';

@Component({
  selector: 'sti-parameter-text-box',
  template: `
    <sti-text-box #element
      [maxLength]="variable.type === 'Char' ? 1 : ''"
      [value]="isMenu ? helper.getStringKey(item.key, variable): value"
      [width]="width"
      [readOnly]="readOnly"
      [focusOnCreate]="focusOnCreate"
      [enabled]="!variable?.isNull"
      [color]="color"
      (onchange)="onchange($event)">
    </sti-text-box>
  `
})

export class ParameterTextBoxComponent implements OnInit {

  @ViewChild('element') element: TextBoxComponent;

  @Input() item: Variable;
  @Input() variable: Variable;
  @Input() readOnly: boolean;
  @Input() focusOnCreate = false;
  @Input() isMenu = false;
  @Input() value = '';
  @Input() secondTextBox = false;

  constructor(public helper: HelperService, public model: ModelService) { }

  ngOnInit() { }

  onchange(target: any) {
    if (this.element.oldValue === target.value) {
      return;
    }
    const value = this.getValue(target);
    if (this.isMenu) {
      if (!this.secondTextBox) {
        this.item.key = value;
      } else {
        this.item.keyTo = value;
      }
    } else {
      if (!this.secondTextBox) {
        if (this.variable.basicType === 'Range' || this.variable.type === 'DateTime') {
          this.variable.key = value;
        } else {
          this.variable.value = value;
        }
      } else {
        this.variable.keyTo = value;
      }
    }
  }

  getValue(target: any): any {
    if (this.variable.type === 'DateTime') {
      if (this.element.oldValue === target.value) {
        return;
      }
      try {
        const timeString = new Date().toLocaleTimeString();
        const isAmericanFormat = timeString.toLowerCase().indexOf('am') >= 0 || timeString.toLowerCase().indexOf('pm') >= 0;
        const formatDate = isAmericanFormat ? 'MM/dd/yyyy' : 'dd.MM.yyyy';
        let format = formatDate + (isAmericanFormat ? ' h:mm:ss tt' : ' hh:mm:ss');
        if (this.variable.dateTimeType === 'Date') {
          format = formatDate;
        }
        if (this.variable.dateTimeType === 'Time') {
          format = 'hh:mm:ss';
        }
        const date = this.helper.getDateTimeFromString(target.value, this.model.options.appearance.parametersPanelDateFormat || format);
        return this.helper.getDateTimeObject(date);
      } catch (e) {
        alert(e);
      }
    } else {
      return target.value;
    }
  }

  get width(): number {
    let width = 210;
    if (this.variable.basicType === 'Range') {
      width = 140;
      if (this.variable.type === 'Guid' || this.variable.type === 'String') {
        width = 190;
      }
      if (this.variable.type === 'DateTime') {
        width = 235;
      }
      if (this.variable.type === 'Char') {
        width = 60;
      }
    } else {
      if (this.variable.type === 'Guid') {
        width = 265;
      } else {
        width = 210;
      }
    }
    return width;
  }

  get color(): string {
    return !this.variable.isNull
      ? (this.model.options.toolbar.fontColor && this.model.options.toolbar.fontColor !== 'Empty' ? this.model.options.toolbar.fontColor : '#444444')
      : this.variable.type === 'DateTime' ? 'transparent' : '#c6c6c6';
  }
}

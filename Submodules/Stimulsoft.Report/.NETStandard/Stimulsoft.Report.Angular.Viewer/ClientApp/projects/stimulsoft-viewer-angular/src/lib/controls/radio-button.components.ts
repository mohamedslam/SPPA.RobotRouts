import { Component, OnInit, Input, OnDestroy, AfterViewInit, Output, EventEmitter } from '@angular/core';
import { ModelService } from '../services/model.service';
import { RadioButtonService } from '../services/radio-button.service';

@Component({
  selector: 'sti-radio-button',
  template: `
    <table [class]="enabled ? 'stiJsViewerRadioButton' : 'stiJsViewerRadioButtonDisabled'" cellpadding="0" cellspacing="0"
      [style.fontFamily]="model.options.toolbar.fontFamily"
      [style.margin]="margin"
      [attr.title]="tooltip"
      (mouseover)="mouseover()" (mouseout)="mouseout()" (click)="click()">
      <tbody>
        <tr class="stiJsViewerClearAllStyles">
          <td class="stiJsViewerClearAllStyles">
            <div class="stiJsViewerRadioButtonOutCircle" [class]="outClass">
              <div class="stiJsViewerRadioButtonInnerCircle"
                  [style.margin]="model.options.isTouchDevice ? '4px' : '3px'"
                  [style.width]="model.options.isTouchDevice ? '9px' : '7px'"
                  [style.height]="model.options.isTouchDevice ? '9px' : '7px'"
                  [style.visibility]="checked ? 'visible' : 'hidden'"
                  [style.opacity]="enabled ? 1 : 0.5">
              </div>
            </div>
          </td>
          <td *ngIf="caption != null" class="stiJsViewerClearAllStyles" style="padding-left: 4px; white-space: nowrap;">
            {{caption}}
          </td>
          <td class="stiJsViewerClearAllStyles" [style.paddingLeft]="paddingLeftLastCell">
            <ng-content></ng-content>
          </td>
        </tr>
      </tbody>
    </table>
  `
})

export class RadioButtonComponent implements OnInit, OnDestroy, AfterViewInit {

  @Output() action: EventEmitter<any> = new EventEmitter();

  @Input() groupName: string;
  @Input() name: string;
  @Input() enabled = true;
  @Input() checked = false;
  @Input() tooltip: string;
  @Input() caption: string;
  @Input() margin: string;
  @Input() paddingLeftLastCell: string;

  over = false;

  constructor(public model: ModelService, public radioService: RadioButtonService) { }

  ngAfterViewInit(): void {
    this.radioService.addButton(this);
  }

  ngOnDestroy(): void {
    this.radioService.removeButton(this);
  }

  ngOnInit() { }

  mouseover() {
    if (!this.enabled) {
      return;
    }
    this.over = true;
  }

  mouseout() {
    if (!this.enabled) {
      return;
    }
    this.over = false;
  }

  click() {
    if (this.enabled) {
      this.checked = true;
      this.radioService.check(this);
      this.action.emit(true);
    }
  }

  public uncheck() {
    this.checked = false;
    this.action.emit(false);
  }

  get outClass(): string {
    return !this.enabled ? 'stiJsViewerRadioButtonOutCircleDisabled' : (this.over ? 'stiJsViewerRadioButtonOutCircleOver' : 'stiJsViewerRadioButtonOutCircle');
  }
}

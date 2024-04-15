import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { Variable } from '../../services/objects';

@Component({
  selector: 'sti-parameter-checkbox',
  template: `
    <sti-check-box [isEnabled]="isEnabled"
                   [isChecked]="checked"
                   [captionText]="captionText"
                   [margin]="margin"
                   [width]="width"
                   [imageBlockParentWidth]="imageBlockParentWidth"
                   (action)="onAction($event)">
    </sti-check-box>
    `
})

export class ParameterCheckboxComponent implements OnInit {

  @Input() params: Variable;
  @Input() captionText: string;
  @Input() margin: string;
  @Input() width: string;
  @Input() imageBlockParentWidth: string;
  @Input() isEnabled = true;
  @Input() isMenuParameter = false;
  @Input() nullable = false;

  @Output() action: EventEmitter<any> = new EventEmitter();

  constructor() { }

  ngOnInit() { }

  onAction(checked: boolean) {
    this.action.emit(checked);
  }

  get checked(): boolean {
    if (this.nullable) {
      return this.params.isNull;
    } else {
      return this.isMenuParameter ? this.params.isChecked : (typeof (this.params.value) === 'boolean' && this.params.value) || this.params.value === 'true' || this.params.value === 'True';
    }
  }

}

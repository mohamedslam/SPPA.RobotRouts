import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { ModelService } from '../services/model.service';

@Component({
  selector: 'sti-check-box',
  template: `
    <table cellpadding="0" cellspacing="0"
      [class]="isEnabled ? 'stiJsViewerCheckBox' : 'stiJsViewerCheckBoxDisabled'"
      [style.fontFamily]="model.options.toolbar.fontFamily"
      [style.color]="model.options.toolbar.fontColor"
      [attr.title]="tooltip"
      [style.margin]="margin"
      [style.width]="width"
      style="box-sizing:content-box"
      (mouseover)="mouseover()"
      (mouseleave)="mouseleave()"
      (click)="click()">
       <tbody>
          <tr class="stiJsViewerClearAllStyles">
             <td style="line-height: 0"
                 [style.padding]="model.options.isTouchDevice ? '1px 3px 1px 1px' : ''"
                 [style.width]="imageBlockParentWidth"
                 class="stiJsViewerClearAllStyles">
                <div [style.width.px]="model.options.isTouchDevice ? 16 : 13"
                     [style.height.px]="model.options.isTouchDevice ? 16 : 13"
                     [class]="isEnabled ? (over ? 'stiJsViewerCheckBoxImageBlockOver' : 'stiJsViewerCheckBoxImageBlock') : 'stiJsViewerCheckBoxImageBlockDisabled'"
                     style="box-sizing: content-box">
                     <table class="stiJsViewerClearAllStyles" cellpadding="0" cellspacing="0" style="width: 100%; height: 100%;">
                        <tbody>
                          <tr class="stiJsViewerClearAllStyles">
                            <td [style.textAlign]="model.options.isTouchDevice ? 'center' : 'right'"
                                [style.verticalAlign]="model.options.isTouchDevice ? 'middle' : 'top'">
                                <img [src]="model.img((indeterminate ? 'CheckBoxIndeterminate' : 'CheckBox') + (styleColors && styleColors.isDarkStyle ? 'White.png' : '.png'))"
                                    [style.visibility]="isChecked ? 'visible' : 'hidden'"
                                    [style.width.px]="indeterminate ? 13 : 12"
                                    [style.height.px]="indeterminate ? 13 : 12"
                                    [style.opacity]="isEnabled ? 1 : 0.5"
                                    style="vertical-align: baseline"/>
                            </td>
                          </tr>
                        </tbody>
                      </table>
                </div>
             </td>
             <td *ngIf="captionText != null"
                 [style.padding]="!model.options.isTouchDevice ? '1px 0 0 4px' : ''"
                 style="white-space: nowrap">
                 {{captionText}}
             </td>
          </tr>
       </tbody>
    </table>
  `
})

export class CheckboxComponent implements OnInit {

  @Input() isEnabled = true;
  @Input() isChecked = false;
  @Input() indeterminate = false;
  @Input() captionText: string;
  @Input() tooltip: string;
  @Input() margin: string;
  @Input() width: string;
  @Input() styleColors: any;
  @Input() imageBlockParentWidth: string;

  @Output() action: EventEmitter<any> = new EventEmitter();

  public over = false;

  constructor(public model: ModelService) { }

  ngOnInit() { }

  mouseover() {
    this.over = true;
  }

  mouseleave() {
    this.over = false;
  }

  click() {
    if (this.isEnabled) {
      this.isChecked = !this.isChecked;
      this.action.emit(this.isChecked);
    }
  }

}

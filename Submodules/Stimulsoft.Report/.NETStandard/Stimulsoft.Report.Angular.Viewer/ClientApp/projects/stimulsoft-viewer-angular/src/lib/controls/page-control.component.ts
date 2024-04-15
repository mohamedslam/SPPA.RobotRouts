import { Component, OnInit, Input } from '@angular/core';
import { ModelService } from '../services/model.service';
import { ControllerService } from '../services/controller.service';
import { HelperService } from '../services/helper.service';

@Component({
  selector: 'sti-page-control',
  template: `
    <table class="stiJsViewerClearAllStyles" cellpadding="0" cellspacing="0" [style.margin]="helper.val(margin, '1px')">
      <tbody>
          <tr class="stiJsViewerClearAllStyles">
            <td class="stiJsViewerClearAllStyles" style="padding: '0 2px 0 2px' ">
              {{model.loc('Page')}}
            </td>
            <td *ngIf="model.options.isMobileDevice" class="stiJsViewerClearAllStyles" style="padding: '0 2px 0 2px' ">
              <span>{{model.reportParams.pageNumber + 1}}</span>
            </td>
            <td *ngIf="!model.options.isMobileDevice" class="stiJsViewerClearAllStyles" style="padding: '0 2px 0 2px' ">
              <sti-text-box [width]="45" [enabled]="enabled" [style.color]="textBoxColor" [style.opacity]="enabled ? 1 : 0.5"
              (action)="textBoxAction($event)" [value]="model.reportParams.pageNumber + 1" [border]="textBoxBorder"></sti-text-box>
            </td>
            <td class="stiJsViewerClearAllStyles" style="padding: '0 2px 0 2px' ">
              {{model.loc('PageOf')}}
            </td>
            <td class="stiJsViewerClearAllStyles" style="padding: '0 2px 0 2px' ">
              {{count}}
            </td>
          </tr>
      </tbody>
    </table>
  `
})

export class PageControlComponent implements OnInit {

  @Input() enabled = true;
  @Input() count = '?';
  @Input() textBoxBorder: string;
  @Input() margin: string;

  constructor(public model: ModelService, public controller: ControllerService, public helper: HelperService) { }

  ngOnInit() { }

  textBoxAction(target: any): void {
    if (this.model.reportParams.pagesCount > 0 && this.model.reportParams.pageNumber !== this.getCorrectValue(target.value) - 1) {
      this.controller.action({ name: 'GoToPage', value: this.getCorrectValue(target.value) - 1 });
    }
  }

  getCorrectValue(value: any): number {
    value = parseInt(value, 10);
    if (value < 1 || !value) {
      value = 1;
    }
    if (value > this.model.reportParams.pagesCount) {
      value = this.model.reportParams.pagesCount;
    }
    return value;
  }

  get textBoxColor(): string {
    const toolbarFontColor = this.model.options.toolbar.fontColor;
    return this.enabled ? (toolbarFontColor && toolbarFontColor !== 'Empty' ? toolbarFontColor : '#444444')
      : (this.model.reportParams && this.model.reportParams.viewMode !== 'SinglePage' ? 'transparent' : toolbarFontColor);
  }
}

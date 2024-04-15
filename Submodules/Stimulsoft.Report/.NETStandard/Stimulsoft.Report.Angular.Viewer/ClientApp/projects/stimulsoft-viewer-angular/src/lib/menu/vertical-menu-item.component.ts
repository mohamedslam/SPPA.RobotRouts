import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { ModelService } from '../services/model.service';
import { Item } from '../services/objects';
import { MenuService } from './menu.service';

@Component({
  selector: 'sti-vertical-menu-item',
  template: `
    <div [style.height]="item?.type == 'DigitalSignature' ? 'auto' :(model.options.isMobileDevice ? '0.4in' : (model.options.isTouchDevice ? '30px' : '24px'))"
         [class]="className"
         [style.width]="item?.type == 'DigitalSignature' ? '100%' :  menuService.getVerticalMenu().width + 'px'"
         [style.lineHeight]="item?.type == 'DigitalSignature' ? 1.3 : null"
         (mouseover)="mouseover()"
         (mouseout)="mouseout()"
         (click)="click()">
         <table class="stiJsViewerClearAllStyles" cellpadding="0" cellspacing="0" style="height: 100%; width: 100%">
            <tbody>
              <tr class="stiJsViewerClearAllStyles">
                 <td *ngIf="item?.imageName != null && !model.options.appearance.rightToLeft"
                     style="width:22px; min-width = 22px; padding: 0; textAlign: center; lineHeight: 0"
                     class="stiJsViewerClearAllStyles">
                     <img [style.width.px]="(imageSizesWidth ? imageSizesWidth : (isSmallItem ? 16 : 32))"
                          [style.height.px]="(imageSizesHeight ? imageSizesHeight : (isSmallItem ? 16 : 32))"
                          [src]="model.img(item?.imageName)"
                          [style.visible]="model.img(item?.imageName) === '' ? 'none' : ''" />
                </td>

                <td *ngIf="item?.caption != null && !model.options.appearance.rightToLeft && item?.type != 'DigitalSignature'"
                     style="padding: 0px 20px 0px 7px; text-align: left; white-space: nowrap;"
                     [style.fontSize]="model.options.isMobileDevice ? '0.16in' : ''"
                     class="stiJsViewerClearAllStyles">
                     {{item?.caption}}
                </td>

                <td *ngIf="item?.caption != null && !model.options.appearance.rightToLeft && item?.type == 'DigitalSignature'"
                     style="padding: 8px 20px 8px 8px ; text-align: left; white-space: nowrap;"
                     [style.fontSize]="model.options.isMobileDevice ? '0.16in' : ''"
                     class="stiJsViewerClearAllStyles" [innerHtml]="item?.caption">
                </td>

                <!-- right-to-left -->
                <td *ngIf="item?.caption != null && model.options.appearance.rightToLeft && item?.type == 'DigitalSignature'"
                     style="padding: 8px 20px 8px 8px; text-align: right; white-space: nowrap;"
                     [style.fontSize]="model.options.isMobileDevice ? '0.16in' : ''"
                     class="stiJsViewerClearAllStyles" [innerHtml]="item?.caption">
                </td>

                <td *ngIf="item?.caption != null && model.options.appearance.rightToLeft && item?.type != 'DigitalSignature'"
                     style="padding: 0 7px 0 20px; text-align: right; white-space: nowrap;"
                     [style.fontSize]="model.options.isMobileDevice ? '0.16in' : ''"
                     class="stiJsViewerClearAllStyles">
                     {{item?.caption}}
                </td>

                <td *ngIf="item?.imageName != null && model.options.appearance.rightToLeft"
                     style="width:22px; min-width = 22px; padding: 0; textAlign: center; lineHeight: 0"
                     class="stiJsViewerClearAllStyles">
                     <img [style.width.px]="(imageSizesWidth ? imageSizesWidth : (isSmallItem ? 16 : 32))"
                          [style.height.px]="(imageSizesHeight ? imageSizesHeight : (isSmallItem ? 16 : 32))"
                          [src]="model.img(item?.imageName)"
                          [style.visible]="model.img(item?.imageName) === '' ? 'none' : ''" />
                </td>
              </tr>
            </tbody>
          </table>
    </div>
  `
})

export class VerticalMenuItemComponent implements OnInit {

  @Input() styleName = 'stiJsViewerMenuStandartItem';
  @Input() item: Item;
  @Input() imageSizesWidth: number;
  @Input() imageSizesHeight: number;

  @Output() action: EventEmitter<any> = new EventEmitter();

  over = false;
  private _enabled = true;

  constructor(public model: ModelService, public menuService: MenuService) { }

  ngOnInit() { }

  @Input() get enabled(): boolean {
    return this._enabled;
  }

  set enabled(value: boolean) {
    if (!value) {
      this.over = false;
    }
    this._enabled = value;
  }

  get className(): string {
    return this.styleName ? this.styleName + ' ' + (this.enabled ? (this.styleName + (this.over ? 'Over' : (this.item?.selected ? 'Selected' : 'Default'))) : this.styleName + 'Disabled') : '';
  }

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
    if (this._enabled) {
      this.action.emit(this.item);
    }
  }

  get isSmallItem(): boolean {
    return this.styleName && this.styleName.indexOf('MenuStandartItem') >= 0;
  }
}

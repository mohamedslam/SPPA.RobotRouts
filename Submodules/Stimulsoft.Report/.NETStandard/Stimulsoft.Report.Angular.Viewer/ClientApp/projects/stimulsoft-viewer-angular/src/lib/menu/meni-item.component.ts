import { Component, OnInit, Input } from '@angular/core';
import { ModelService } from '../services/model.service';

export class MenuItem {
  constructor(public name?: string, public caption?: string, public img?: string, public type?: string, public selected?: boolean, public imageSize?: string,
    public value?: any) { }
}

@Component({
  selector: 'sti-menu-item',
  template: `
    <div *ngIf="item.type!='separator'" [class]="className" (mouseover)="mouseover()"
         (mouseout)="mouseout()" [style.height]="height">
         <table style="height: 100%; width: 100%" class="stiJsViewerClearAllStyles" cellpadding="0" cellspacing="0">
            <tr class="stiJsViewerClearAllStyles">
                <td *ngIf="item.img != null && !model.options.appearance.rightToLeft" class="stiJsViewerClearAllStyles" style="width:22px; min-width:22px; padding: 0; text-align:center; line-height: 0;">
                  <img *ngIf="item.img != ''" [style.height.px]="item.imageSize === 'Big' ? 32 : 16" [style.width.px]="item.imageSize === 'Big' ? 32 : 16" src="{{model.img(item.img)}}" />
                </td>

                <td *ngIf="item.caption != null" class="stiJsViewerClearAllStyles" style="white-space:nowrap"
                  [style.textAlign]="model.options.appearance.rightToLeft ? 'right' : 'left'"
                  [style.padding]="item.imageSize == 'None' ? '0 20px 0 30px' : '0 20px 0 7px'">
                  {{item.caption}}
                </td>

                <td *ngIf="item.img != null && model.options.appearance.rightToLeft" class="stiJsViewerClearAllStyles" style="width:22px; min-width:22px; padding: 0; text-align:center; line-height: 0;">
                  <img *ngIf="item.img != ''" [style.height.px]="item.imageSize === 'Big' ? 32 : 16" [style.width.px]="item.imageSize === 'Big' ? 32 : 16" src="{{model.img(item.img)}}" />
                </td>
            </tr>
         </table>
    </div>
    <div *ngIf="item.type=='separator'" class="stiJsViewerVerticalMenuSeparator" [style.margin]="item.imageSize == 'Big' ? '1px 2px 1px 2px' : ''">
    </div>
  `
})

export class MenuItemComponent implements OnInit {

  @Input() item: MenuItem;
  @Input() styleName = 'stiJsViewerMenuStandartItem';

  selected = false;
  over = false;

  private _enabled = true;

  constructor(public model: ModelService) { }

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

  mouseover() {
    if (!this.enabled || !this.enabled) {
      return;
    }
    this.over = true;
  }

  mouseout() {
    if (!this.enabled || !this.enabled) {
      return;
    }
    this.over = false;
  }

  get height(): string {
    if (this.item.imageSize === 'Big') {
      return '38px';
    } else {
      return this.model.options.isMobileDevice ? '0.4in' : (this.model.options.isTouchDevice ? '30px' : '24px');
    }
  }

  get className(): string {
    return this.styleName ? this.styleName + ' ' + (this.enabled ? (this.styleName + ((this.selected || this.item.selected) ? 'Selected' : (this.over ? 'Over' : ''))) : this.styleName + 'Disabled') : '';
  }
}

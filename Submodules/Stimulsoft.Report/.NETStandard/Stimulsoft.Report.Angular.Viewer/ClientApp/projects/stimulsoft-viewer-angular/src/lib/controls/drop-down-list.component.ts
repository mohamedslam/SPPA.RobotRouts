import { Component, OnInit, Input, ViewChild, ElementRef, Output, EventEmitter } from '@angular/core';
import { ModelService } from '../services/model.service';
import { Item } from '../services/objects';
import { MouseService } from '../services/mouse.service';
import { MenuService } from '../menu/menu.service';

@Component({
  selector: 'sti-drop-down-list',
  template: `
        <table #element [class]="className" cellpadding="0" cellspacing="0"
              [style.fontFamily]="model.options.toolbar.fontFamily"
              [style.color]="model.options.toolbar.fontColor"
              [style.margin]="margin"
              [style.verticalAlign]="verticalAlign"
              [style.display]="styleDisplay"
              [attr.title]="toolTip"
              (mouseover)="mouseover()"
              (mouseout)="mouseout()">
          <tbody>
            <tr class="stiJsViewerClearAllStyles">
              <td class="stiJsViewerClearAllStyles">
                <input #input
                [style.width.px]="width - (model.options.isTouchDevice ? 23 : 15) - (showImage ? 38 : 0)"
                [style.border]="0"
                [style.cursor]="readOnly ? 'default' : 'text'"
                [style.fontFamily]="model.options.toolbar.fontFamily"
                [style.color]="model.options.toolbar.fontColor"
                [style.height]="model.options.isTouchDevice ? '23px' : '18px'"
                [style.lineHeight]="model.options.isTouchDevice ? '23px' : '18px'"
                [style.visibility]="enabled ? 'visible' : 'hidden'"
                [value]="inputValue"
                (click)="inputClick()"
                class="stiJsViewerDropDownList_TextBox">
              </td>

              <td class="stiJsViewerClearAllStyles">
                  <sti-button [imageName]="'Arrows.' + (model.options.isTouchDevice ? 'Big' : 'Small') + 'ArrowDown.png'"
                     [styleName]="'stiJsViewerDropDownListButton'"
                     [imageSizesWidth]="model.options.isTouchDevice ? 16 : 8"
                     [imageSizesHeight]="model.options.isTouchDevice ? 16 : 8"
                     [height]="model.options.isTouchDevice ? '26px' : '21px'"
                     [enabled]="enabled"
                     [selected]="selected"
                     [margin]="'0'"
                     (action)="showListMenu()">
                  </sti-button>
              </td>
            </tr>
          </tbody>
        </table>

  `
})

export class DropDownListComponent implements OnInit {

  @ViewChild('input') input: ElementRef;
  @ViewChild('element') element: ElementRef;

  @Input() toolTip: string;
  @Input() showImage = false;
  @Input() width: number;
  @Input() margin: string;
  @Input() items: Item[];
  @Input() styleDisplay: string;
  @Input() verticalAlign: string;

  @Output() action: EventEmitter<any> = new EventEmitter();

  over = false;
  private _readOnly = false;
  private _enabled = true;
  private _key: any;

  constructor(public model: ModelService, public mouseService: MouseService, public menuService: MenuService) {
    this.mouseService.getDocumentMouseUp().subscribe(() => {
      this.hideListMenu();
    });
  }

  ngOnInit() { }

  set readOnly(value: boolean) {
    this._readOnly = value;
    setTimeout(() => {
      try {
        this.input.nativeElement.setAttribute('unselectable', value ? 'on' : 'off');
        this.input.nativeElement.setAttribute('onselectstart', value ? 'return false' : '');
        this.input.nativeElement.readOnly = value;
      } catch (e) { }
    });
  }

  @Input() get readOnly(): boolean {
    return this._readOnly;
  }

  @Input() get enabled(): boolean {
    return this._enabled;
  }

  set enabled(value: boolean) {
    if (!value) {
      this.over = false;
    }
    this._enabled = value;
  }

  @Input() get key(): any {
    return this._key;
  }

  set key(key: any) {
    if (this.items != null) {
      this.items.forEach(i => i.selected = i.key === key);
    }
    this._key = key;
  }

  get inputValue(): string {
    if (this.items != null) {
      return this.items.find(i => i.key === this._key)?.caption || '';
    }
    return '';
  }

  get selected(): boolean {
    const vm = this.menuService.getVerticalMenu();
    return vm?.state === 'expanded' && vm?.parent === this.element;
  }

  get className(): string {
    return this.selected ? 'stiJsViewerDropDownListOver' : (this.enabled ? (this.over ? 'stiJsViewerDropDownListOver' : 'stiJsViewerDropDownList') : 'stiJsViewerDropDownListDisabled');
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

  inputClick() {
    if (this.readOnly) {
      this.showListMenu();
    }
  }

  showListMenu() {
    this.menuService.addMenu({
      type: this.menuService.VERTICAL_MENU_NAME, name: this.menuService.VERTICAL_MENU_NAME, items: [], parent: this.element,
      verticalItems: this.items, itemStyleName: 'stiJsViewerMenuStandartItem', menuStyleName: 'stiJsViewerDropdownMenu',
      action: this.action, width: this.element.nativeElement.offsetWidth,
      state: ''
    });

    setTimeout(() => {
      this.menuService.showMenu(this.menuService.VERTICAL_MENU_NAME);
    });
  }

  hideListMenu() {
    if (this.menuService.getVerticalMenu()) {
      this.menuService.getVerticalMenu().state = 'initialDown';
    }
  }

}

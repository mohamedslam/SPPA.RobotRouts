import { Injectable, ElementRef, EventEmitter } from '@angular/core';
import { MenuItem } from './meni-item.component';
import { HelperService } from '../services/helper.service';
import { ModelService } from '../services/model.service';
import { MouseService } from '../services/mouse.service';
import { Variable, Item } from '../services/objects';
import { Subject, Observable } from 'rxjs';

export class Menu {

  private _state?: string;
  public name: string;
  public items: MenuItem[];
  public parent: ElementRef;
  public type: string;
  public menuEl?: ElementRef;
  public top?: number;
  public left?: number;
  public innerContent?: ElementRef;
  public width?: number;
  public height?: number;
  public menuStyleName?: string;
  public params?: Variable;
  public verticalItems?: Item[];
  public action?: EventEmitter<any>;
  public itemStyleName?: string;
  public value?: any;
  public sizeStyle?: string;
  public onCloseEvent?: any;

  constructor() { }

  public get state(): string {
    return this._state;
  }

  public set state(value: string) {
    let oldState = this._state;
    this._state = value;
    if (oldState != 'initialDown' && oldState != '' && value == 'initialDown' && this.onCloseEvent) {
      this.onCloseEvent();
    }
  }

}

@Injectable()
export class MenuService {
  public VERTICAL_MENU_NAME = 'verticalMenu';

  public menuMouseUp = '';

  private _menus: {} = {};
  private subject = new Subject<Menu>();

  constructor(private helper: HelperService, private model: ModelService, private mouseService: MouseService) {
    this.mouseService.getDocumentMouseUp().subscribe((event) => {
      if (this.menuMouseUp === '') {
        this.closeAllMenus();
      } else if (this.menuMouseUp !== 'datePickerMenu' && this.menuMouseUp !== this.VERTICAL_MENU_NAME) {
        const datePicker = Object.values(this._menus).find(m => (m as Menu).type === 'datePickerMenu') as Menu;
        if (datePicker != null) {
          datePicker.state = 'initialDown';
        }
      }
      this.menuMouseUp = '';
    });
  }

  public getVisibility(): Observable<Menu> {
    return this.subject.asObservable();
  }

  public addMenu(menu: Menu): void {
    let inMenu = new Menu();
    for (let key in menu) {
      inMenu[key] = menu[key];
    }
    this._menus[menu.name] = inMenu;
  }

  public closeAllMenus() {
    Object.values(this._menus).forEach((m: Menu) => m.state = m.type === 'buttonMenu' && this.model.options.isMobileDevice ? 'initialLeft' : 'initialDown');
  }

  public isMenuVisible(): boolean {
    return Object.values(this._menus).find(m => (m as Menu).state === 'expanded') != null;
  }

  public showMenu(menuName: string) {
    const menu: Menu = this._menus[menuName];

    if (menu?.type !== 'datePickerMenu' && menu?.type !== this.VERTICAL_MENU_NAME) {
      this.closeAllMenus();
    }

    if (menu) {
      menu.sizeStyle = 'opacity: 0;';
      if (this.model.options.isMobileDevice && menu.type === 'buttonMenu') {
        setTimeout(() => {
          const innerContent = menu.innerContent.nativeElement;
          menu.left = 0;
          menu.top = 0;
          menu.width = innerContent.offsetWidth;
          menu.height = innerContent.offsetHeight;
          menu.state = 'initialLeft';
          menu.sizeStyle = null;

          setTimeout(() => {
            menu.state = 'expanded';
          });
        });
      } else {
        setTimeout(() => {
          menu.sizeStyle = null;
          this.showMenuInternal(menu);
        });
      }
      this.subject.next(menu);
    }
  }

  private showMenuInternal(menu: Menu) {
    const isVertMenu = true;

    const parentButton = menu.parent.nativeElement;
    const innerContent = menu.innerContent.nativeElement;
    const offsetHeight = menu.menuEl.nativeElement.offsetHeight;
    const style = menu.menuEl.nativeElement.style;

    const coords = this.getMenuCoordinates(parentButton, innerContent, offsetHeight, style, isVertMenu);

    menu.left = coords.left;
    menu.top = coords.top;
    menu.width = menu.width || coords.width;
    menu.height = coords.height;
    menu.state = coords.state;

    setTimeout(() => {
      menu.state = 'expanded';
    });
  }

  public getMenuCoordinates(parentButton: any, innerContent: any, offsetHeight: number, style: any, isVertMenu: boolean): any {
    const menu: any = {};
    const browserWidth = window.innerWidth || document.documentElement.clientWidth || document.body.clientWidth;
    const browserHeight = window.innerHeight || document.documentElement.clientHeight || document.body.clientHeight;
    const rightAlign = false;
    const mainClassName = 'stiJsViewerMainPanel';
    let animDirect = 'Down';
    const leftOffset = 0;

    let left = (isVertMenu)
      ? (this.model.options.appearance.rightToLeft || rightAlign
        ? (this.helper.findPosX(parentButton, mainClassName) - innerContent.offsetWidth + parentButton.offsetWidth) - (leftOffset || 0)
        : this.helper.findPosX(parentButton, mainClassName) - (leftOffset || 0))
      : ((animDirect === 'Right')
        ? (this.helper.findPosX(parentButton, mainClassName) + parentButton.offsetWidth + 2)
        : (this.helper.findPosX(parentButton, mainClassName) - parentButton.offsetWidth - 2));

    if (left + innerContent.offsetWidth > browserWidth) {
      left = browserWidth - innerContent.offsetWidth - 15;
    }
    if (left < 0) {
      left = 10;
    }
    menu.left = left;

    if (animDirect === 'Down' &&
      this.helper.findPosY(parentButton) + parentButton.offsetHeight + innerContent.offsetHeight > browserHeight &&
      this.helper.findPosY(parentButton) - innerContent.offsetHeight > 0) {
      animDirect = 'Up';
    }

    menu.top = (isVertMenu)
      ? ((animDirect === 'Down')
        ? (this.helper.findPosY(parentButton, mainClassName) + parentButton.offsetHeight + 2)
        : (this.helper.findPosY(parentButton, mainClassName) - offsetHeight))
      : (this.helper.findPosY(parentButton, mainClassName) + parentButton.offsetHeight + innerContent.offsetHeight > browserHeight &&
        (browserHeight - innerContent.offsetHeight - 10) > 0)
        ? (browserHeight - innerContent.offsetHeight - 10)
        : this.helper.findPosY(parentButton, mainClassName);

    menu.width = innerContent.offsetWidth;
    menu.height = innerContent.offsetHeight;

    if (menu.top + innerContent.offsetHeight > browserHeight) {
      menu.top = (browserHeight - innerContent.offsetHeight - 10);
    }

    if (menu.top < 0) {
      menu.top = 10;
    }

    menu.state = animDirect === 'Down' ? 'initialDown' : 'initialUp';

    return menu;
  }

  get menus(): Menu[] {
    return Object.values(this._menus);
  }

  getVerticalMenu(): Menu {
    return this._menus[this.VERTICAL_MENU_NAME];
  }

  getMenu(name: string): Menu {
    return this._menus[name];
  }

}

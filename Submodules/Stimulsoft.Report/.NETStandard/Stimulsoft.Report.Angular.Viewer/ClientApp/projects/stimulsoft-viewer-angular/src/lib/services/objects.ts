import { MenuItem } from '../menu/meni-item.component';
import { ElementRef, Injectable, Optional } from '@angular/core';
import { Subject, Observable } from 'rxjs';
import { PageService } from './page.service';

export class ComponentDescription {
  constructor(public type: string, public caption?: string, public img?: string, public action?: string,
    public tooltip?: boolean, public arrow?: string, public menuItems?: MenuItem[], public margin?: string, public selected?: boolean) {
    this.selected = false;
  }
}


export class ViewerEvent {
  constructor(public name: string, public value?: any, public bookmarkPage?: number, public bookmarkAnchor?: string, public componentGuid?: string) { }
}

export class ControlClass {
  public toolbar: ControlProps = new ControlProps();
  public reportPanel: ControlProps = new ControlProps();
  public drillDownPanel: ControlProps = new ControlProps();
  public findPanel: ControlProps = new ControlProps();
  public resourcesPanel: ControlProps = new ControlProps();
  public bookmarksPanel: ControlProps = new ControlProps();
  public navigatePanel: ControlProps = new ControlProps();
  public parametersPanel: ControlProps = new ControlProps();
  public dashboardsPanel: ControlProps = new ControlProps();
  public viewer: ControlProps = new ControlProps();
  public tooltip: ControlProps = new ControlProps();
  public aboutPanel: ControlProps = new ControlProps();

  public bookmarksLabel: any;

  constructor() { }

  public get head(): HTMLHeadElement {
    return document.getElementsByTagName('head')[0];
  }
}

export class ControlProps {
  private _visible = false;
  private subject = new Subject<any>();

  public enabled = true;

  public layout: Rectangle = new Rectangle();
  constructor(public pageService?: PageService, public el?: ElementRef) { }

  public get offsetHeight(): number {
    return this.el?.nativeElement.offsetHeight || 0;
  }

  public get offsetWidth(): number {
    return this.el?.nativeElement.offsetWidth || 0;
  }

  public set visible(value: boolean) {
    this._visible = value;
    this.subject.next(value);
    this.pageService?.calculateLayout();
  }

  public get visible(): boolean {
    return this._visible;
  }

  public getVisibility(): Observable<any> {
    return this.subject.asObservable();
  }

  public get exists(): boolean {
    return this.el != null;
  }
}

export class Rectangle {

  private _top: number = 0;

  public set top(value: number) {
    this._top = value;
  }

  public get top(): number {
    return this._top;
  }

  constructor(public width: number = 0, public height: number = 0, public left: number = 0,
    public bottom: number = 0, public right: number = 0) { }
}

export class BookmarkNode {

  constructor(public name?: string, public url?: string, public page?: number, public compunentGuid?: string, public nodes?: BookmarkNode[],
    public open?: boolean, public selected?: boolean) { }

}

export class InteractionParams {

  constructor(public action?: string, public drillDownParameters?: any, public drillDownGuid?: string, public dashboardDrillDownGuid?: string,
    public variables?: any, public sortingParameters?: any, public collapsingParameters?: any, public isBindingVariable?: true) { }
}

export class InteractionObject {
  constructor(public paramsVariables: any[], public countColumns: any[], public countInColumn: any[]) { }
}

export class Variable {
  constructor(public name?: string, public alias?: string, public description?: string, public basicType?: string, public type?: string,
    public allowUserValues?: boolean, public dateTimeType?: string, public items?: Variable[], public key?: any, public value?: any,
    public keyTo?: any, public isChecked: boolean = true, public focusOnCreate?: boolean, public visible: boolean = true, public binding?: boolean,
    public isNull?: boolean, public isFirstInitialization?: boolean, public checkedStates?: boolean[]) { }
}

export class DateTimeObject {
  constructor(public year?: number, public month?: number, public day?: number, public hours?: number, public minutes?: number, public seconds?: number) { }
}

export class Item {
  constructor(public name?: string, public caption?: string, public imageName?: string, public key?: any, public haveSubMenu?: boolean, public imageSizes?: any,
    public selected?: boolean, public type?: string) { }
}

export class Message {
  constructor(public action: string, public data?: any, public subAction?: string) { }
}

export class Form {
  constructor(public name: string, public left: number, public top: number, public isMooving = false, public level?: number,
    public formData?: any) { }
}

export class ExportFormSettings {
  constructor(public components: ExportComponent[], public cSettings?: any, public openAfterExport?: boolean, public groups?: any,
    public update?: boolean) { }
}

export class ExportGroup {
  constructor(public opened: boolean) { }
}

export class ExportComponent {
  constructor(public name: string, public type: string, public margin: string, public label?: string, public tooltip?: string,
    public caption?: string, public checked?: boolean, public width?: number, public disabled?: boolean, public items?: Item[],
    public key?: any) { }
}


export class DrillDown {
  constructor(public caption: string, public selected: boolean = false, public reportParams: any,
    public visible: boolean) { }
}

export class ErrorMessage {
  constructor(public error: string, public type: any) { }
}

export class Resource {
  constructor(public name: string, public type: string, public alias: string, public id?: string) { }
}

export class NotificationFormOptions {
  constructor(public image?: string, public message?: string, public description?: string, public buttonCaption?: string,
    public cancelAction?: any, public action?: any) { }
}

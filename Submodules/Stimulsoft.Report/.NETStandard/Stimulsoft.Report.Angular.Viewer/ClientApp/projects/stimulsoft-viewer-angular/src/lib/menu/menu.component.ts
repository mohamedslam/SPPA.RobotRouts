import { Component, OnInit, Input, ElementRef, ViewChild, AfterViewInit } from '@angular/core';
import { ModelService } from '../services/model.service';
import { MenuItem } from './meni-item.component';
import { Menu, MenuService } from './menu.service';
import { trigger, state, style, transition, animate, AnimationEvent } from '@angular/animations';
import { ControllerService } from '../services/controller.service';
import { HelperService } from '../services/helper.service';

@Component({
  selector: 'sti-menu',
  template: `
      <div #menuEl class="stiJsViewerParentMenu"
      [style]="menu.sizeStyle || style"
      [style.top.px]="menu.top"
      [style.left.px]="menu.left"
      [style.zIndex]="zIndex"
      [style.height]="menu.type=='buttonMenu' && model.options.isMobileDevice ? '100%' : menu.height + 'px'"
      [@.disabled]="!model.options.toolbar.menuAnimation"
      (mouseup)="mouseup()">
        <div #innerContent [style]="{overflowX: 'hidden', overflowY: overflow}"
        [style.color]="model.options.toolbar.fontColor"
        [style.fontFamily]="model.options.toolbar.fontFamily"
        [style.maxHeight]="menu.type=='buttonMenu' && model.options.isMobileDevice ? '100%' : '450px'"
        [style.height]="menu.type=='buttonMenu' && model.options.isMobileDevice ? '100%' : null"
        [@expand]="menu.state || 'initialDown'"
        (@expand.start)="startAnimation($event)"
        (@expand.done)="doneAnimation($event)"
        [class]="helper.val(menu.menuStyleName, 'stiJsViewerMenu')">
            <ng-container *ngIf="menu.type=='buttonMenu'">
              <sti-menu-item *ngFor="let item of menu.items" [item]="item" (click)="menuClick(item)"></sti-menu-item>
            </ng-container>

            <sti-parameter-menu-edit-list *ngIf="menu.type=='parameterMenuForEditList'" [menu]="menu">
            </sti-parameter-menu-edit-list>

            <sti-parameter-menu-not-edit-list *ngIf="menu.type=='parameterMenuForNotEditList'" [menu]="menu">
            </sti-parameter-menu-not-edit-list>

            <sti-parameter-menu-for-value *ngIf="menu.type=='parameterMenuForValue'" [menu]="menu">
            </sti-parameter-menu-for-value>

            <sti-parameter-menu-for-range *ngIf="menu.type=='parameterMenuForRange'" [menu]="menu">
            </sti-parameter-menu-for-range>

            <sti-date-picker-menu *ngIf="menu.type=='datePickerMenu'" [menu]="menu">
            </sti-date-picker-menu>

            <sti-double-date-picker-menu *ngIf="menu.type=='doubleDatePickerMenu'" [menu]="menu">
            </sti-double-date-picker-menu>

            <sti-document-security-menu *ngIf="menu.type=='documentSecurityMenu'">
            </sti-document-security-menu>

            <sti-digital-signature-menu *ngIf="menu.type=='useDigitalSignatureMenu'">
            </sti-digital-signature-menu>

            <sti-vertical-menu *ngIf="menu.type=='verticalMenu'" [menu]="menu">
            </sti-vertical-menu>
        </div>
      </div>
  `,
  animations: [
    trigger('expand', [
      state('initialDown', style({ transform: 'translateY(-100%)' })),
      state('initialUp', style({ transform: 'translateY(100%)' })),
      state('initialLeft', style({ transform: 'translateX(-100%)' })),
      state('expanded', style({ transform: 'translateY(0) translateX(0)' })),
      transition('initialUp => expanded', [
        animate('150ms ease-in-out')]),
      transition('initialDown => expanded', [
        animate('150ms ease-in-out')]),
      transition('initialLeft => expanded', [
        animate('150ms ease-in-out')]),
      transition('expanded => initialLeft', [
        animate('150ms ease-in-out')])
    ])
  ]
})

export class MenuComponent implements OnInit, AfterViewInit {

  @Input() menu: Menu;

  @ViewChild('menuEl') menuEl: ElementRef;
  @ViewChild('innerContent') innerContent: ElementRef;

  style = 'display: none';
  overflow = 'hidden';

  constructor(public model: ModelService, public controller: ControllerService, public helper: HelperService, public menuService: MenuService) { }

  ngAfterViewInit(): void {
    this.menu.menuEl = this.menuEl;
    this.menu.innerContent = this.innerContent;
  }

  ngOnInit() {
  }

  startAnimation(event: AnimationEvent) {
    if (event.toState === 'expanded') {
      this.style = 'width: 350px; overflow: hidden';
      this.overflow = 'hidden';
    }
  }

  doneAnimation(event: AnimationEvent) {
    if (event.toState === 'expanded') {
      this.style = 'overflow: visible;';
      this.overflow = this.menu.type.indexOf('parameterMenu') === 0 || this.model.options.isMobileDevice ? 'auto' : 'hidden';
    } else if (event.toState === 'initialUp' || event.toState === 'initialDown') {
      this.style = 'display: none';
      this.overflow = 'hidden';
    }
  }

  menuClick(item: MenuItem) {
    this.menuService.closeAllMenus();
    this.controller.action({ name: item.name, value: item.value });
  }

  mouseup() {
    this.menuService.menuMouseUp = this.menu.type;
  }

  get zIndex() {
    return this.menu.type === 'datePickerMenu' ? 36 : (this.menu.type === 'verticalMenu' ? 37 : 35);
  }

}

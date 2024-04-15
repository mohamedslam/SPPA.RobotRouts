import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { Item } from '../services/objects';
import { Menu } from './menu.service';

@Component({
  selector: 'sti-vertical-menu',
  template: `
    <sti-vertical-menu-item *ngFor="let item of menu.verticalItems" [item]="item" [styleName]="menu.itemStyleName" (action)="action(item)">
    </sti-vertical-menu-item>
  `
})

export class VerticalMenuComponent implements OnInit {

  @Input() menu: Menu;

  constructor() { }

  ngOnInit() { }

  action(item: Item) {
    this.menu?.action?.emit(item);
  }
}

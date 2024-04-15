import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { Item } from '../services/objects';

@Component({
  selector: 'sti-drop-down-list-for-export-form',
  template: `
    <sti-drop-down-list [items]="items"
      [width]="width"
      [readOnly]="true"
      [key]="key"
      [margin]="margin"
      [enabled]="enabled"
      [styleDisplay]="styleDisplay"
      [verticalAlign]="verticalAlign"
      (action)="key = $event.key; action.emit($event)">
    </sti-drop-down-list>
  `
})

export class DropDownListForExportFormComponent implements OnInit {

  @Input() items: Item[];
  @Input() width: number;
  @Input() key: string;
  @Input() margin: string;
  @Input() enabled = true;
  @Input() styleDisplay: string;
  @Input() verticalAlign: string;

  @Output() action: EventEmitter<any> = new EventEmitter();

  constructor() { }

  ngOnInit() { }
}

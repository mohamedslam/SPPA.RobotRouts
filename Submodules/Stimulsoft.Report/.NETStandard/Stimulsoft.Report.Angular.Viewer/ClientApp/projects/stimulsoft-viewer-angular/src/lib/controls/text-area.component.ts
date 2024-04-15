import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'sti-text-area',
  template: `
    <textarea
      [style.width.px]="width"
      [style.minWidth.px]="width"
      [style.height.px]="height"
      [style.minHeight.px]="height"
      [class]="className"
      [value]="value || ''"
      [style.margin]="margin"
      [style.cursor]="readOnly ? 'default' : ''"
      [style.padding]="padding"
      style="padding-top: 3px, font-family: Arial"
      (mouseover)="over=true"
      (mouseleave)="over=false"
      (focused)="focused=true; selected=true"
      (blur)="focused=false; selected=false; onblur.emit($event.target)"
      (keypress)="keypress($event)"
      (keyup)="onchanged($event)">
    </textarea>
  `
})

export class TextAreaComponent implements OnInit {

  @Input() width: number;
  @Input() height: number;
  @Input() enabled = true;
  @Input() value: string;
  @Input() padding: string;
  @Input() margin: string;

  @Output() action: EventEmitter<any> = new EventEmitter();
  @Output() onchange: EventEmitter<any> = new EventEmitter();
  @Output() onblur: EventEmitter<any> = new EventEmitter();

  selected = false;
  focused = false;
  over = false;

  _readOnly = false;

  styleName = 'stiJsViewerTextBox';

  constructor() { }

  ngOnInit() { }

  keypress(event: KeyboardEvent) {
    if (!this.enabled) {
      return false;
    }
    if (event.keyCode === 13) {
      this.action.emit(event.target);
    }
  }

  onchanged(event: KeyboardEvent) {
    this.onchange.emit(event.target);
  }

  get className(): string {
    if (this._readOnly) {
      return this.styleName + ' ' + this.styleName + 'Default';
    }
    return this.styleName + ' ' + this.styleName + (this.selected ? 'Over' : (this.enabled ? (this.over ? 'Over' : 'Default') : 'Disabled'));
  }

  get readOnly(): boolean {
    return this._readOnly;
  }
}

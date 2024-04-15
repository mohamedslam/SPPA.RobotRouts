import { Component, OnInit, Input, ViewChild, ElementRef, Output, EventEmitter, AfterViewInit } from '@angular/core';
import { ModelService } from '../services/model.service';

@Component({
  selector: 'sti-text-box',
  template: `
    <input #element
    [style]="{fontFamily:model.options.toolbar.fontFamily, height: height, lineHeight: height, boxSizing:'content-box'}"
    [style.color]="color ? color : (model.options.toolbar.fontColor != '' ? model.options.toolbar.fontColor : '')"
    [style.width]="width != null ? width + 'px' : ''"
    [style.margin]="margin"
    [style.cursor]="readOnly ? 'default' : ''"
    [style.padding]="padding"
    [style.border]="border"
    [title]="tooltip || ''"
    [class]="className"
    [value]="value || ''"
    [attr.maxLength]="maxLength"
    [attr.type]="type"
    (mouseover)="over=true"
    (mouseleave)="over=false"
    (focused)="focused=true; selected=true"
    (blur)="focused=false; selected=false; onblur.emit($event.target)"
    (keypress)="keypress($event)"
    (keyup)="onchanged($event)"
    (focus)="focus()"/>
  `
})

export class TextBoxComponent implements OnInit, AfterViewInit {

  @ViewChild('element') element: ElementRef;

  @Input() width: number;
  @Input() actionLostFocus: any;
  @Input() tooltip: string;
  @Input() enabled = true;
  @Input() value: string;
  @Input() margin: string;
  @Input() focusOnCreate = false;
  @Input() maxLength: number;
  @Input() color: string;
  @Input() type: string;
  @Input() padding: string;
  @Input() border: string;

  @Output() action: EventEmitter<any> = new EventEmitter();
  @Output() onchange: EventEmitter<any> = new EventEmitter();
  @Output() onblur: EventEmitter<any> = new EventEmitter();

  public oldValue: string;

  selected = false;
  focused = false;
  over = false;
  _readOnly = false;

  styleName = 'stiJsViewerTextBox';

  constructor(public model: ModelService) { }

  ngAfterViewInit(): void {
    if (this.focusOnCreate) {
      setTimeout(() => {
        this.element.nativeElement.focus();
      });
    }
  }

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

  focus() {
    this.oldValue = this.element.nativeElement.value;
  }

  get height(): string {
    return this.model.options.isTouchDevice ? '26px' : '21px';
  }

  get className(): string {
    if (this._readOnly) {
      return this.styleName + ' ' + this.styleName + 'Default';
    }
    return this.styleName + ' ' + this.styleName + (this.selected ? 'Over' : (this.enabled ? (this.over ? 'Over' : 'Default') : 'Disabled'));
  }

  @Input()
  set readOnly(value: boolean) {
    this._readOnly = value;
    setTimeout(() => {
      try {
        this.element.nativeElement.setAttribute('unselectable', value ? 'on' : 'off');
        this.element.nativeElement.setAttribute('onselectstart', value ? 'return false' : '');
        this.element.nativeElement.readOnly = value;
      } catch (e) { }
    });
  }

  get readOnly(): boolean {
    return this._readOnly;
  }

  ngOnInit() { }
}

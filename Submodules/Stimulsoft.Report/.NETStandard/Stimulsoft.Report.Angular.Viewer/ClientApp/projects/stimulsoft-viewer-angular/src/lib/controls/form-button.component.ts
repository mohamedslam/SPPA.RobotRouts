import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'sti-form-button',
  template: `
      <sti-button [styleName]="'stiJsViewerFormButton'"
                  [caption]="caption"
                  [actionName]="actionName"
                  [captionAlign]="captionAlign"
                  [innerTableWidth]="'100%'"
                  [minWidth]="'80px'"
                  [imageName]="imageName"
                  [imageCellWidth]="imageCellWidth"
                  [captionPadding]="captionPadding"
                  [margin]="margin"
                  (action)="onaction($event)">
      </sti-button>
  `
})
export class FormButtonComponent implements OnInit {

  @Input() caption: string;
  @Input() actionName: string;
  @Input() imageName: string;
  @Input() imageCellWidth: string;
  @Input() captionPadding: string;
  @Input() margin: string;
  @Input() captionAlign = 'center';

  @Output() action: EventEmitter<any> = new EventEmitter();

  constructor() { }

  ngOnInit() { }

  onaction(event: any) {
    this.action.emit(event);
  }
}

import { Component, OnInit, Output, EventEmitter, Input } from '@angular/core';
import { ModelService } from '../services/model.service';

@Component({
  selector: 'sti-parameter-menu-item',
  template: `
      <div [class]="className"
        [style.height.px]="model.options.isTouchDevice ? 30 : 24"
        (mouseover)="mouseover()"
        (mouseout)="mouseout()"
        (mousedown)="mousedown()"
        (mouseup)="mouseup()">
        <table class="stiJsViewerClearAllStyles stiJsViewerParametersMenuItemInnerTable" cellpadding="0" cellspacing="0">
          <tbody>
            <tr class="stiJsViewerClearAllStyles">
              <td class="stiJsViewerClearAllStyles" [style.padding]="padding">
                <ng-content>
                </ng-content>
              </td>
            </tr>
          </tbody>
        </table>

      </div>
  `
})

export class ParameterMenuItemComponent implements OnInit {

  @Output() action: EventEmitter<any> = new EventEmitter();

  @Input() active = true;
  @Input() padding = '0px 5px';

  isOver = false;
  mouseDown = false;

  constructor(public model: ModelService) { }

  ngOnInit() { }

  mouseover() {
    this.isOver = true;
  }

  mouseout() {
    this.isOver = false;
    this.mouseDown = false;
  }

  mousedown() {
    this.mouseDown = true;
  }

  mouseup() {
    this.mouseDown = false;
    this.action.emit();
  }

  get className(): string {
    return this.active ? this.mouseDown ? 'stiJsViewerParametersMenuItemPressed' : (this.isOver ? 'stiJsViewerParametersMenuItemOver' : 'stiJsViewerParametersMenuItem') : 'stiJsViewerParametersMenuItem';
  }
}

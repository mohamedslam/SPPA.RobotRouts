import { Component, OnInit, Input } from '@angular/core';
import { ModelService } from '../services/model.service';

@Component({
  selector: 'sti-dashboard-button',
  template: `
    <sti-button
      [margin]="'2px 1px 2px 2px'"
      [display]="display"
      [borderColor]="!dbsMode ? 'transparent' : ''"
      [height]="model.options.toolbar.displayMode == 'Separated' ? (dbsMode ? (model.options.isTouchDevice ? '28px' : '23px') : '28px') : null">
    </sti-button>
  `
})

export class DashboardButtonComponent implements OnInit {

  constructor(public model: ModelService) { }

  @Input() showCloseButton = false;
  @Input() info: any;
  @Input() dbsMode: boolean;
  @Input() display: string;

  ngOnInit() { }

}

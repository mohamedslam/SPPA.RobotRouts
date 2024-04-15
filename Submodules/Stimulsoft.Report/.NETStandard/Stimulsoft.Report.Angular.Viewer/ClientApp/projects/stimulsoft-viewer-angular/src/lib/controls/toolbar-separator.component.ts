import { Component, OnInit } from '@angular/core';
import { ModelService } from '../services/model.service';

@Component({
  selector: 'sti-toolbar-separator',
  template: `
    <div style="width:1px" [style.height]="model.options.isMobileDevice ? '0.4in' : (model.options.isTouchDevice ? '26px' : '21px')" class="stiJsViewerToolBarSeparator"></div>
  `
})

export class ToolbarSeparatorComponent implements OnInit {
  constructor(public model: ModelService) { }

  ngOnInit() { }
}

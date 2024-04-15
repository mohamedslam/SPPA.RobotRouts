import { Component, OnInit } from '@angular/core';
import { ModelService } from '../services/model.service';

@Component({
  selector: 'sti-progress',
  template: `
    <div style="position: absolute; z-index: 1000; left: calc(50% - 35px);"
        [style.top]="model.options.appearance.fullScreenMode ? 'calc(50% - 100px)' : '250px'"
        [style.display]="model.showProgress ? '' : 'none'">
      <div class="js_viewer_loader js_viewer_loader_default"></div>
    </div>
  `
})

export class ProgressComponent implements OnInit {

  constructor(public model: ModelService) { }

  ngOnInit() { }
}

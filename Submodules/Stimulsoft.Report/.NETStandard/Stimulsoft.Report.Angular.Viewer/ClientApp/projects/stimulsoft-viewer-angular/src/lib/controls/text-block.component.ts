import { Component, OnInit, Input } from '@angular/core';
import { ModelService } from '../services/model.service';

@Component({
  selector: 'sti-text-block',
  template: `
    <div [style]="{fontFamily: model.options.toolbar.fontFamily, fontSize: '12px', paddingTop: '2px', margin: margin}">
      {{text || ''}}
    </div>
  `
})

export class TextBlockComponent implements OnInit {

  @Input() text: string;
  @Input() margin: string;

  constructor(public model: ModelService) { }

  ngOnInit() { }
}

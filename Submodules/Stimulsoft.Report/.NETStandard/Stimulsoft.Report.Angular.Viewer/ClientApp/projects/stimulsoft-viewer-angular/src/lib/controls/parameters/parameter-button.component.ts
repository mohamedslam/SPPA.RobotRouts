import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { Variable } from '../../services/objects';
import { ModelService } from '../../services/model.service';

@Component({
  selector: 'sti-parameter-button',
  template: `
    <sti-button [imageName]="buttonType + '.png'"
        [styleName]="'stiJsViewerFormButton'"
        [height]="model.options.isTouchDevice ? '26px' : '21px'"
        [innerTableWidth]="'100%'"
        [enabled]="!params?.isNull"
        (action)="onaction()">
    </sti-button>
  `
})

export class ParameterButtonComponent implements OnInit {

  @Input() params: Variable;
  @Input() buttonType: string;

  @Output() action: EventEmitter<any> = new EventEmitter();

  constructor(public model: ModelService) { }

  ngOnInit() { }

  onaction() {
    this.action.emit(this.params);
  }
}

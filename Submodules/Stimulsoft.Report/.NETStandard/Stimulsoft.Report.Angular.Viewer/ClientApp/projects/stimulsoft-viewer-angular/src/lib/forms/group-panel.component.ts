import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { ModelService } from '../services/model.service';
import { trigger, state, transition, animate, style, AnimationEvent } from '@angular/animations';

@Component({
  selector: 'sti-group-panel',
  template: `
  <div #groupPanel [style.fontFamily]="model.options.toolbar.fontFamily"
       [style.color]="model.options.toolbarFontColor"
       [style.minWidth.px]="width"
       [style.margin]="margin"
       style="overflow: hidden">
      <sti-form-button [caption]="caption"
          [imageName]="opened ? 'Arrows.BigArrowDown.png' : 'Arrows.BigArrowRight.png'"
          [imageCellWidth]="'1px'"
          [captionPadding]="'0 15px 0 5px'"
          [margin]="'0px'"
          [captionAlign]="'left'"
          (action)="opened = !opened; action.emit(opened)">
      </sti-form-button>

      <div class="stiJsViewerGroupPanelContainer"
           [style.padding]="innerPadding"
           [style.display]="display"
           [@state]="opened ? 'opened' : 'closed'"
           (@state.start)="startAnimation($event)"
           (@state.done)="doneAnimation($event)">
          <ng-content>

          </ng-content>
      </div>


  </div>
  `,
  animations: [
    trigger('state', [
      state('opened', style({ height: '*' })),
      state('closed', style({ height: '0px' })),
      transition('* => *', [
        animate('150ms ease-in-out')])
    ])
  ]
})

export class GroupPanelComponent implements OnInit {

  @Input() width: number;
  @Input() caption: string;
  @Input() innerPadding: string;
  @Input() opened = false;
  @Input() margin: string;

  @Output() action: EventEmitter<any> = new EventEmitter();

  display = 'none';

  constructor(public model: ModelService) { }

  ngOnInit() { }

  startAnimation(event: AnimationEvent) {
    if (event.toState === 'opened') {
      this.display = '';
    }
  }

  doneAnimation(event: AnimationEvent) {
    if (event.toState === 'closed') {
      this.display = 'none';
    }
  }
}

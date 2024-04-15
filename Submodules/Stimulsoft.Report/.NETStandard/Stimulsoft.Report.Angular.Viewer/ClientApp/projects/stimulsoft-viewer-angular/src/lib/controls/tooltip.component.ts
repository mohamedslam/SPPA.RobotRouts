import { Component, OnInit, AfterViewInit, ViewChild, ElementRef } from '@angular/core';
import { ModelService } from '../services/model.service';
import { trigger, state, transition, animate, style, keyframes } from '@angular/animations';
import { TooltipService } from '../services/tooltip.service';
import { HelperService } from '../services/helper.service';

@Component({
  selector: 'sti-tooltip',
  template: `
      <div #element class='stiJsViewerToolTip' [@expandDown]="tooltipService.state"
        [style.left.px]='tooltipService.left' [style.top.px]='tooltipService.top'
        (mouseover)="tooltipService.showFromTooltip()" (mouseout)="tooltipService.hide()">
        <table class="stiJsViewerClearAllStyles" cellpadding="0" cellspacing="0" style="height: 100%;"
          [style.border]="model.options.appearance.showTooltipsHelp ? '' : 0">
          <tbody>
            <tr class="stiJsViewerClearAllStyles">
              <td class="stiJsViewerToolTipTextCell">
                {{tooltipService.caption}}
              </td>
            </tr>
            <tr *ngIf="model.options.appearance.showTooltipsHelp" class="stiJsViewerClearAllStyles">
              <td  class="stiJsViewerClearAllStyles">
                <sti-button [caption]="model.loc('TellMeMore')"
                    [imageName]="'HelpIcon.png'"
                    [margin]="'4px 8px 4px 8px'"
                    (action)='action()'
                    [styleName]="'stiJsViewerHyperlinkButton'">
                </sti-button>
              </td>
            </tr>
          </tbody>
        </table>
    </div>
  `,
  animations: [
    trigger('expandDown', [
      state('initial', style({ opacity: 0, display: 'none' })),
      state('preInitial', style({ opacity: 0, display: 'block' })),
      state('preInitial2', style({ opacity: 0, display: 'block' })),
      state('expanded', style({ opacity: 1, display: 'block' })),
      transition('preInitial => expanded', [
        animate('300ms 300ms ease-in-out', keyframes([
          style({ display: 'block', opacity: 0, offset: 0 }),
          style({ display: 'block', opacity: 1, offset: 1 }),
        ]))
      ])
    ])
  ]
})

export class TooltipComponent implements OnInit, AfterViewInit {

  @ViewChild('element') element: ElementRef;

  constructor(public model: ModelService, public tooltipService: TooltipService, public helper: HelperService) { }

  ngAfterViewInit(): void {
    this.model.controls.tooltip.el = this.element;
  }

  ngOnInit() { }

  action() {
    this.helper.showHelpWindow(this.tooltipService.helpLink);
  }

}

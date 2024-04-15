import { Component, OnInit, ViewChild, ElementRef, AfterViewInit } from '@angular/core';
import { ModelService } from '../services/model.service';
import { DrillDownService } from '../services/drill-down.service';
import { ControllerService } from '../services/controller.service';
import { Message, DrillDown } from '../services/objects';

@Component({
  selector: 'sti-drill-down-panel',
  template: `
  <div #element [class]="'stiJsViewerToolBar' + (model.options.toolbar.displayMode == 'Separated' ? ' stiJsViewerToolBarSeparated' : '')"
    [style.display]="model.controls.drillDownPanel.visible ? '' : 'none'">
    <div [style.paddingTop]="model.options.toolbar.displayMode == 'Simple' ? '2px' : ''">
      <table class="stiJsViewerToolBarTable" cellpadding="0" cellspacing="0" style="margin: 0px; box-sizing: border-box;"
        [style.border]="model.options.toolbar.displayMode == 'Separated' ? '0px' : ''"
        [style.color]="model.options.toolbar.fontColor"
        [style.fontFamily]="model.options.toolbar.fontFamily">
        <tbody>
          <tr class="stiJsViewerClearAllStyles">
            <td class="stiJsViewerClearAllStyles">
              <table class="stiJsViewerClearAllStyles" cellpadding="0" cellspacing="0">
                <tbody>
                  <tr class="stiJsViewerClearAllStyles">
                    <td *ngFor="let button of model.drillDownButtons; index as i" class="stiJsViewerClearAllStyles" style="padding: 0px; border: 0px; line-height: 0;">
                      <sti-button [display]="button.visible ? 'inline-block': 'none'"
                        [margin]="'2px 1px 2px 2px'"
                        [caption]="button.caption"
                        [height]="model.options.toolbar.displayMode == 'Separated' ? '28px' : null"
                        [display]="'inline-block'"
                        [selected]="button.selected"
                        [closeButton]="i > 0"
                        (action)="action(button)"
                        (closeButtonAction)="close(button)">
                      </sti-button>
                    </td>
                  </tr>
                </tbody>
              </table>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
  `
})

export class DrillDownPanelComponent implements OnInit, AfterViewInit {

  @ViewChild('element') element: ElementRef;

  constructor(public model: ModelService, public drillDownService: DrillDownService, public controller: ControllerService) {
    controller.getMessage().subscribe((message: Message) => {
      if (message?.action === 'DrillDown' && this.element) {
        this.element.nativeElement.scrollTop = 0;
      }
    });
  }

  ngOnInit() { }

  ngAfterViewInit(): void {
    this.model.controls.drillDownPanel.el = this.element;
  }

  action(button: DrillDown) {
    this.drillDownService.saveState();
    this.model.drillDownButtons.forEach(b => b.selected = false);
    button.selected = true;
    this.model.setReportParams(button.reportParams);
    this.element.nativeElement.scrollTop = 0;
    this.controller.getPages();
  }

  close(button: DrillDown) {
    this.model.drillDownButtons.splice(this.model.drillDownButtons.indexOf(button), 1);
    button.visible = false;
    if (button.selected) {
      this.action(this.model.drillDownButtons[0]);
    }
  }
}

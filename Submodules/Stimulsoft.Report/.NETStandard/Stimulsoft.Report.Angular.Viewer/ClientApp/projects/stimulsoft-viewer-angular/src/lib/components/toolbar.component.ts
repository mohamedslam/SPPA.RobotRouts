import { Component, OnInit, ViewChild, ElementRef, AfterViewInit } from '@angular/core';
import { ModelService } from '../services/model.service';
import { ControllerService } from '../services/controller.service';
import { ToolbarService } from '../services/toolbar.service';

@Component({
  selector: 'sti-toolbar',
  template: `
    <div #element [class]="'stiJsViewerToolBar' + (model.options.toolbar.displayMode === 'Separated' ? '  stiJsViewerToolBarSeparated' : '')" style="display: block"
         [style.width.px]="!model.controls.toolbar.visible ? '0px' : ''"
         [style.height.px]="!model.controls.toolbar.visible ? '0px' : ''"
         [style.display]="!model.options.isMobileDevice ? (this.model.controls.toolbar.visible ? '' : 'none') : ''"
         [style.transition]="model.options.isMobileDevice ? 'margin 300ms ease, opacity 300ms ease' : null"
         [style.zIndex]="model.options.toolbar.autoHide ? 5 : 2"
         [style.position]="model.options.toolbar.autoHide ? 'absolute' : 'relative'"
         [style.opacity]="model.options.isMobileDevice ? (model.controls.toolbar.visible ? (model.options.toolbar.autoHide ? 0.9 : 1) : 0) : 1"
         [style.marginTop]="model.options.isMobileDevice && !model.controls.toolbar.visible ? '-0.55in' : 0">
      <div [style]="model.options.toolbar.displayMode === 'Simple' ? 'paddingTop: 2px' : ''">
        <table class="stiJsViewerToolBarTable" style="margin: 0px; box-sizing: border-box;"
               cellpadding="0" cellspacing="0"
               [style.background]="model.options.toolbar.backgroundColor !== '' ? model.options.toolbar.backgroundColor : ''"
               [style.border]="model.options.toolbar.displayMode === 'Separated' ? '0px' : (model.options.toolbar.borderColor !== '' ?  '1px solid ' + model.options.toolbar.borderColor : '')"
               [style.color]="model.options.toolbar.fontColor !== '' ? model.options.toolbar.fontColor : ''"
               [style.fontFamily]="model.options.toolbar.fontFamily">
          <tbody>
            <tr class="stiJsViewerClearAllStyles">
              <td *ngIf="model.options.appearance.rightToLeft">
              <sti-toolbar-dop [comps]="toolbarService.dopComps" ></sti-toolbar-dop>
              </td>

              <td class="stiJsViewerClearAllStyles" style="width: 100%">
                <sti-toolbar-dop
                  [align]="model.options.appearance.rightToLeft ? 'right' : (model.options.toolbar.alignment === 'default' ? 'left' : model.options.toolbar.alignment)"
                  [comps]="toolbarService.comps">
                </sti-toolbar-dop>
              </td>

              <td *ngIf="!model.options.appearance.rightToLeft">
                <sti-toolbar-dop [comps]="toolbarService.dopComps" ></sti-toolbar-dop>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
      <div class="stiJsViewerDisabledPanel" [style.display]="this.model.controls.toolbar.enabled ? 'none' : 'block'"></div>
    </div>
  `
})

export class ToolbarComponent implements OnInit, AfterViewInit {

  @ViewChild('element') element: ElementRef;

  constructor(public model: ModelService, public controller: ControllerService, public toolbarService: ToolbarService) { }

  ngAfterViewInit(): void {
    this.model.controls.toolbar.el = this.element;
  }

  ngOnInit() {
    this.toolbarService.initButtons();
  }

}

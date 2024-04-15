import { Component, OnInit, AfterViewInit, ViewChild, ElementRef } from '@angular/core';
import { ModelService } from '../services/model.service';
import { HelperService } from '../services/helper.service';
import { InteractionsService } from '../services/interactions.service';
import { MenuService } from '../menu/menu.service';
import { ControllerService } from '../services/controller.service';
import { trigger, state, transition, style, keyframes, animate } from '@angular/animations';

@Component({
  selector: 'sti-interactions-panel',
  template: `
    <div #element [class]="className"
       [style.display]="!this.model.options.isMobileDevice ? (model.controls.parametersPanel.visible ? '' : 'none') : null"
       [style.fontFamily]="model.options.toolbar.fontFamily"
       [style.color]="model.options.toolbar.fontColor"
       [style.top.px]="top"
       [style.left.px]="model.controls.parametersPanel.layout.left"
       [style.bottom]="bottom"
       [style.transition]="model.options.isMobileDevice ? 'opacity 300ms ease' : null"
       [@visibility]="!this.model.options.isMobileDevice ? null : (model.controls.parametersPanel.visible ? 'visible' : 'hidden')">
       <div [class]="innerClassName" [style.marginTop]="model.options.toolbar.displayMode == 'Simple' ? '2px' : ''" (scroll)="menuService.closeAllMenus()">
         <div class="stiJsViewerInnerContainerParametersPanel"
              [style.background]="helper.val(model.options.toolbar.backgroundColor, '')"
              [style.border]="helper.val(model.options.toolbar.backgroundColor, '') != '' ? '1px solid ' + helper.val(model.options.toolbar.backgroundColor, '') : ''"
              [style.maxHeight]="model.options.appearance.parametersPanelPosition == 'Top' ? model.options.appearance.parametersPanelMaxHeight + 'px' : ''"
              (scroll)="menuService.closeAllMenus()">
            <table *ngIf="model.interactions" class="stiJsViewerClearAllStyles" cellpadding="0" cellspacing="0" style="border: 0px;">
              <tbody>
                <tr *ngFor="let item of model.interactions.countInColumn; let indexRow=index" >
                  <ng-container *ngFor="let item2 of model.interactions.countColumns; let indexColumn=index">
                    <ng-container *ngIf="index(indexRow, indexColumn) < length()">
                        <td [style.padding]="'0 10px 0 ' + (indexColumn > 0 ? '30px' : '0')"
                            [style.whiteSpace]="'nowrap'"
                            [style.verticalAlign]='getNameAlign(indexRow, indexColumn)'
                            [style.paddingTop]='getNamePadding(indexRow, indexColumn)'
                            [attr.title]='getTitle(indexRow, indexColumn)'>
                            {{getCaption(indexRow, indexColumn)}}
                              <table *ngIf="ifLeftRange(indexRow, indexColumn)" class="stiJsViewerClearAllStyles" cellpadding="0" cellspacing="0" style="height: 60px;">
                                <tbody>
                                  <tr class="stiJsViewerClearAllStyles">
                                    <td class="stiJsViewerClearAllStyles" rowspan="2" style="vertical-align: top; padding-top: 9px;">
                                      {{getCaption(indexRow, indexColumn, false)}}
                                    </td>
                                    <td class="stiJsViewerClearAllStyles" style="padding-left: 12px;">
                                      {{model.loc('RangeFrom')}}
                                    </td>
                                  </tr>
                                  <tr class="stiJsViewerClearAllStyles">
                                    <td class="stiJsViewerClearAllStyles" style="padding-left: 12px;">
                                      {{model.loc('RangeTo')}}
                                    </td>
                                  </tr>
                                </tbody>
                              </table>
                            </td>
                        <td [style.padding]="'0px'">
                          <sti-parameter *ngIf="index(indexRow, indexColumn) != length()"  [params]="model.interactions.paramsVariables[index(indexRow, indexColumn)]"></sti-parameter>

                          <table *ngIf="index(indexRow, indexColumn) == length()" class="stiJsViewerClearAllStyles" cellpadding="0" cellspacing="0"
                            align="right" style="margin: 5px 2px 10px 0px;">
                            <tr class="stiJsViewerClearAllStyles">
                              <td class="stiJsViewerClearAllStyles">
                                <sti-form-button [caption]="model.loc('Reset')" (action)="controller.action({name: 'Reset'})"></sti-form-button>
                              </td>
                              <td class="stiJsViewerClearAllStyles" style="padding-left: 10px;">
                                <sti-form-button [caption]="model.loc('Submit')" (action)="controller.action({name: 'Submit'})"></sti-form-button>
                              </td>
                            </tr>
                          </table>
                        </td>
                    </ng-container>
                  </ng-container>
                </tr>
                <tr *ngIf="length() == model.interactions.countInColumn.length * model.interactions.countColumns.length">
                  <td></td>
                  <td *ngIf="model.interactions.countColumns.length > 1"></td>
                  <td *ngIf="model.interactions.countColumns.length > 1"></td>
                  <td [style.padding]="'0px'">
                      <table class="stiJsViewerClearAllStyles" cellpadding="0" cellspacing="0"
                         align="right" style="margin: 5px 2px 10px 0px;">
                         <tr class="stiJsViewerClearAllStyles">
                           <td class="stiJsViewerClearAllStyles">
                             <sti-form-button [caption]="model.loc('Reset')" (action)="controller.action({name: 'Reset'})"></sti-form-button>
                           </td>
                           <td class="stiJsViewerClearAllStyles" style="padding-left: 10px;">
                              <sti-form-button [caption]="model.loc('Submit')" (action)="controller.action({name: 'Submit'})"></sti-form-button>
                           </td>
                         </tr>
                       </table>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
      </div>
    </div>
  `,
  animations: [
    trigger('visibility', [
      state('visible', style({ opacity: 1, display: 'block' })),
      state('hidden', style({ opacity: 0, display: 'none' })),
      transition('hidden => visible', [
        animate('300ms ease-in-out', keyframes([
          style({ display: 'block', opacity: 0, offset: 0 }),
          style({ display: 'block', opacity: 1, offset: 1 }),
        ]))
      ]),
      transition('visible => hidden', [
        animate('300ms ease-in-out', keyframes([
          style({ display: 'block', opacity: 1, offset: 0 }),
          style({ display: 'none', opacity: 0, offset: 1 }),
        ]))
      ])
    ])
  ]
})

export class InteractionsPanelComponent implements OnInit, AfterViewInit {

  currentOpeningParameter = null;
  dropDownButtonWasClicked = false;
  dateTimeButtonWasClicked = false;

  @ViewChild('element') element: ElementRef;

  constructor(public model: ModelService, public helper: HelperService, public interactionService: InteractionsService, public menuService: MenuService,
    public controller: ControllerService) { }

  ngAfterViewInit(): void {
    this.model.controls.parametersPanel.el = this.element;
  }

  ngOnInit() { }

  getNameAlign(indexRow: number, indexColumn: number): string {
    const index = this.index(indexRow, indexColumn);
    if (index < this.length() &&
      this.model.interactions.paramsVariables[index].basicType === 'Range' &&
      this.model.options.appearance.parametersPanelPosition === 'Left') {
      return 'top';
    }
    return '';
  }

  getNamePadding(indexRow: number, indexColumn: number): string {
    const index = this.index(indexRow, indexColumn);
    if (index < this.length() &&
      this.model.interactions.paramsVariables[index].basicType === 'Range' &&
      this.model.options.appearance.parametersPanelPosition === 'Left') {
      return this.model.options.isTouchDevice ? '11px' : '9px';
    }
    return '';
  }

  getCaption(indexRow: number, indexColumn: number, checkLeft = true): string {
    const index = this.index(indexRow, indexColumn);
    if (checkLeft && this.ifLeftRange(indexRow, indexColumn)) {
      return '';
    }
    return index < this.length() ? this.model.interactions.paramsVariables[index].alias : '';
  }

  ifLeftRange(indexRow: number, indexColumn: number): boolean {
    const index = this.index(indexRow, indexColumn);
    return this.model.interactions.paramsVariables[index].basicType === 'Range' && this.model.options.appearance.parametersPanelPosition === 'Left';
  }

  getTitle(indexRow: number, indexColumn: number): string {
    const index = this.index(indexRow, indexColumn);
    return index < this.length() ? this.model.interactions.paramsVariables[index].description : '';
  }

  index(indexRow: number, indexColumn: number): number {
    return indexColumn * this.model.interactions.countInColumn.length + indexRow;
  }

  length(): number {
    return this.model.interactions?.paramsVariables != null ? Object.keys(this.model.interactions?.paramsVariables).length : 0;
  }

  get className(): string {
    let className = 'stiJsViewerParametersPanel';

    if (this.model.options.appearance.parametersPanelPosition === 'Top') {
      className += ' stiJsViewerParametersPanelTop';

      if (this.model.options.toolbar.displayMode === 'Separated') {
        className += ' stiJsViewerParametersPanelSeparatedTop';
      }
    }

    return className;
  }

  get top(): number {
    let styleTop = this.model.options.toolbar.visible ? this.model.controls.toolbar.offsetHeight : 0;
    if (this.model.options.isMobileDevice && this.model.options.toolbar.autoHide) {
      styleTop = 0;
    }
    styleTop += this.model.controls.drillDownPanel.exists ? this.model.controls.drillDownPanel.offsetHeight : 0;
    styleTop += this.model.controls.findPanel.exists ? this.model.controls.findPanel.offsetHeight : 0;
    styleTop += this.model.controls.resourcesPanel.exists ? this.model.controls.resourcesPanel.offsetHeight : 0;
    return styleTop;
  }

  get bottom(): string {
    if (this.model.options.appearance.parametersPanelPosition === 'Left') {
      if (this.model.options.isMobileDevice) {
        return this.model.options.toolbar.autoHide ? '0' : '0.5in';
      } else {
        return this.model.options.toolbar.displayMode === 'Separated' && this.model.options.toolbar.visible ? '35px' : '0';
      }
    }
    return '';
  }

  get innerClassName(): string {
    let className = this.model.options.toolbar.displayMode === 'Simple' ? 'stiJsViewerInnerParametersPanelSimple' : '';

    if (this.model.options.appearance.parametersPanelPosition === 'Left') {
      className += ' stiJsViewerInnerParametersPanelLeft';
      if (this.model.options.toolbar.displayMode === 'Separated') {
        className += ' stiJsViewerInnerParametersPanelSeparatedLeft';
      }
    }
    return className;
  }

}

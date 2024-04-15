import { Component, OnInit, Input, ViewChild, ElementRef, Output, EventEmitter } from '@angular/core';
import { ControllerService } from '../services/controller.service';
import { ModelService } from '../services/model.service';
import { MenuService, Menu } from '../menu/menu.service';
import { TooltipService } from '../services/tooltip.service';
import { HelperService } from '../services/helper.service';
import { MenuItem } from '../menu/meni-item.component';

@Component({
  selector: 'sti-button',
  template: `
  <div #button
    [style.fontFamily]="model.options.toolbar.fontFamily"
    [style.cursor]="cursor"
    [style.borderColor]="borderColor"
    [class]="className"
    [style]="{height: helper.val(height, '23px'), boxSizing: helper.val(boxSizing, 'content-box'), margin: helper.val(margin, '1px'), minWidth: minWidth, width: width, display: display }"
    [style.fontSize]="fontSize"
    (mouseover)="mouseover()"
    (mouseout)="mouseout()"
    (click)="click()" >
    <table class="stiJsViewerClearAllStyles" cellpadding="0" cellspacing="0" style="height: 100%;width:100%" [style.width]="innerTableWidth">
      <tbody>
        <tr class="stiJsViewerClearAllStyles">
          <td *ngIf="imageName != null" [style]="{lineHeight:'0', padding:helper.val(imageCellPadding, '0 3px'), textAlign: imageCellTextAlign, width: imageCellWidth}" class="stiJsViewerClearAllStyles">
            <img src="{{model.img(imageName)}}" [style.opacity]="enabled ? '1' : '0.5'"
                [style.width.px]="imageSizesWidth"
                [style.height.px]="imageSizesHeight"
                [style.margin]="imageMargin">
          </td>
          <td *ngIf="caption != null"
             [style]="{whiteSpace:'nowrap', textAlign: captionAlign || 'left', padding:captionPadding || ((arrow ? '1px 0' : '1px 5px') + (imageName ? ' 0 0' : ' 0 5px')), width: captionWidth}"
             [style.maxWidth]="captionMaxWidth"
             [style.lineHeight]="captionLineHeight"
             [style.paddingLeft]="captionPaddingLeft"
             [style.whiteSpace]="captionWhiteSpace"
             [style.overflow]="captionOverflow"
             [style.textOverflow]="captionTextOverflow"
             class="stiJsViewerClearAllStyles">
            {{caption}}
            <div *ngIf="caption2 != null">
            {{caption2}}
            </div>
          </td>
          <td *ngIf="arrow" class="stiJsViewerClearAllStyles">
            <img src="{{arrow == 'Up' || arrow == 'Down' ? model.img('Arrows.SmallArrow' + arrow + (arrow == 'Down' && styleColors && styleColors.isDarkStyle ? 'White.png' : '.png')) : model.img(arrow)}}"
            [style]="{lineHeight:'0', width:'8px', height:'8px', verticalAlign:'baseline', padding:caption ? '0 5px 0 5px' : '0 5px 0 2px', marginTop: arrowMarginTop}"
            [style.opacity]="enabled ? '1' : '0.5'"/>
          </td>

          <td *ngIf="closeButton" class="stiJsViewerClearAllStyles">
            <sti-button [imageName]="'CloseForm.png'"
              [imageMargin]="'1px 0 0 -1px'"
              [margin]="'0 2px 0 0'"
              [imageCellPadding]="'0'"
              [width]="model.options.isTouchDevice ? '22px' : '17px'"
              [height]="model.options.isTouchDevice ? '22px' : '17px'"
              (action)="closeButtonPressed = true; closeButtonAction.emit()">
            </sti-button>
          </td>

          <td #resButtonEl *ngIf="resourceButton" style="width: 1px;">
            <sti-button [imageName]="'Arrows.SmallArrowDown.png'"
              [styleName]="'stiJsViewerResourceDropDownButton'"
              [innerTableWidth]="'100%'"
              [margin]="'0 7px 0 3px'"
              [imageCellTextAlign]="'center'"
              [imageSizesWidth]="8"
              [imageSizesHeight]="8"
              [height]="model.options.isTouchDevice ? '23px' : '17px'"
              [width]="model.options.isTouchDevice ? '23px' : '17px'"
              (action)="resourceButtonPressed = true;">>
            </sti-button>
          </td>

        </tr>
      </tbody>
    </table>
  </div>
  `
})
export class ButtonComponent implements OnInit {

  @Input() caption: string;
  @Input() caption2: string;
  @Input() captionAlign: string;
  @Input() captionPadding: string;
  @Input() captionPaddingLeft: string;
  @Input() captionWhiteSpace: string;
  @Input() captionOverflow: string;
  @Input() captionTextOverflow: string;
  @Input() captionWidth: string;
  @Input() captionMaxWidth: string;
  @Input() captionLineHeight: string;
  @Input() imageName: string;
  @Input() arrow: string;
  @Input() arrowMarginTop: string;
  @Input() margin = '1px';
  @Input() height = '23px';
  @Input() selected = false;
  @Input() minWidth: string;
  @Input() innerTableWidth: string;
  @Input() menuItems: MenuItem[];
  @Input() actionName: string;
  @Input() tooltip: any;
  @Input() imageCellTextAlign = 'center';
  @Input() imageCellWidth: string;
  @Input() imageCellPadding: string;
  @Input() imageSizesWidth = 16;
  @Input() imageSizesHeight = 16;
  @Input() imageMargin: string;
  @Input() width: string;
  @Input() display: string;
  @Input() closeButton = false;
  @Input() resourceButton = false;
  @Input() styleColors: any;
  @Input() boxSizing: string;
  @Input() navagationPanelTooltip = false;
  @Input() cursor: string;
  @Input() fontSize: string;
  @Input() helpLink = 'user-manual/index.html?viewer_reports.htm';
  @Input() borderColor;

  @Output() action: EventEmitter<any> = new EventEmitter();
  @Output() closeButtonAction: EventEmitter<any> = new EventEmitter();
  @ViewChild('button') button: ElementRef;
  @ViewChild('resButtonEl') resButtonEl: ElementRef;

  over = false;
  showMenu = false;
  closeButtonPressed = false;
  resourceButtonPressed = false;
  private _enabled = true;
  private _styleName: string;
  private menuObj: Menu;
  private canShowTooltip = true;

  constructor(public model: ModelService, public controller: ControllerService, public menuService: MenuService, private tooltipService: TooltipService,
    public helper: HelperService) { }

  ngOnInit(): void {
    if (this.menuItems) {
      setTimeout(() => {
        this.menuObj = { type: 'buttonMenu', name: this.actionName, items: this.menuItems, parent: !this.resourceButton ? this.button : this.resButtonEl, state: '' };
        this.menuService.addMenu(this.menuObj);
      }, 500);
    }
  }

  @Input() get styleName(): string {
    return this._styleName || 'stiJsViewerStandartSmallButton';
  }

  set styleName(value: string) {
    this._styleName = value;
  }

  @Input() get enabled(): boolean {
    return this._enabled;
  }

  set enabled(value: boolean) {
    if (!value) {
      this.over = false;
    }
    this._enabled = value;
  }

  mouseover() {
    if (!this.enabled) {
      return;
    }
    this.over = true;

    if (this.model.options.toolbar.showMenuMode === 'Hover' && ['Print', 'Save', 'SendEmail', 'Zoom', 'ViewMode'].some(a => a === this.actionName)) {
      this.tooltipService.hideImmediately();
      this.menuService.showMenu(this.actionName);
    } else if (this.tooltip && !this.menuService.menus.some(e => e.state === 'expanded') && this.canShowTooltip) {
      const top = !this.navagationPanelTooltip ? this.model.controls.toolbar.offsetHeight + this.model.controls.dashboardsPanel.offsetHeight :
        this.helper.findPosY(this.model.controls.navigatePanel.el.nativeElement, 'stiJsViewerMainPanel');
      const tooltip = this.tooltip === true ? (this.model.localization[this.actionName + 'ToolTip'] != null ? this.model.loc(this.actionName + 'ToolTip') : this.model.loc(this.actionName)) : this.tooltip;
      this.tooltipService.show(this.helper.findPosX(this.button.nativeElement, 'stiJsViewerMainPanel'),
        top, tooltip, !this.navagationPanelTooltip, this.helpLink);
    }
  }

  mouseout() {
    if (!this.enabled) {
      return;
    }
    this.over = false;
    this.tooltipService.hide();
    setTimeout(() => {
      this.canShowTooltip = true;
    }, 1000);
  }

  click() {
    if (this.enabled && !this.closeButtonPressed) {
      this.tooltipService.hideImmediately();
      if (this.menuItems == null || (this.resourceButton && !this.resourceButtonPressed)) {
        this.action.emit();
      } else {
        this.menuService.showMenu(this.actionName);
      }
    }
    this.canShowTooltip = false;
    this.closeButtonPressed = false;
    this.resourceButtonPressed = false;
  }

  get className(): string {
    return this.styleName ? this.styleName + ' ' + (this.enabled ? (this.styleName + ((this.selected || this.menuObj?.state === 'expanded') ? 'Selected' : (this.over ? 'Over' : 'Default'))) : this.styleName + 'Disabled') : '';
  }
}

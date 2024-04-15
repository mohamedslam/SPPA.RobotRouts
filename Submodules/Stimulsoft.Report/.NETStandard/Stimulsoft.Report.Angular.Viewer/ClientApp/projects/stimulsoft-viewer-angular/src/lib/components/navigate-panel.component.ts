import { Component, OnInit, AfterViewInit, ViewChild, ElementRef } from '@angular/core';
import { ModelService } from '../services/model.service';
import { ControllerService } from '../services/controller.service';
import { ComponentDescription, Message } from '../services/objects';
import { HelperService } from '../services/helper.service';
import { PageService } from '../services/page.service';

@Component({
  selector: 'sti-navigate-panel',
  template: `
  <div #element class="stiJsViewerNavigatePanel" style="display: block;"
    [style.display]="!model.options.isMobileDevice ? (this.model.controls.navigatePanel.visible ? '' : 'none') : ''"
    [style.transition]="model.options.isMobileDevice ? 'margin 300ms ease, opacity 300ms ease' : null"
    [style.zIndex]="model.options.isMobileDevice ? (model.options.toolbar.autoHide ? 5 : 2) : null"
    [style.opacity]="model.options.isMobileDevice ? (model.controls.navigatePanel.visible ? (model.options.toolbar.autoHide ? 0.9 : 1) : 0) : 1"
    [style.marginBottom]="model.options.isMobileDevice && !model.controls.navigatePanel.visible ? '-0.55in' : 0">
    <table class="stiJsViewerClearAllStyles" cellpadding="0" cellspacing="0">
      <tbody>
        <tr class="stiJsViewerClearAllStyles">
          <td *ngFor="let comp of comps" class="stiJsViewerClearAllStyles" [style.width]="comp.type == 'space' ? '100%' : null">
            <sti-button *ngIf="comp.type=='button'"
              [caption]="comp.caption"
              [imageName]="comp.img"
              [arrow]="comp.arrow"
              [menuItems]="comp.menuItems"
              [actionName]="comp.action"
              [tooltip]="comp.tooltip"
              [styleName]="'stiJsViewerNavigateButton'"
              [height]="model.options.isMobileDevice ? '0.5in' : '35px'"
              [width]="model.options.isMobileDevice ? '0.4in' : '35px'"
              [boxSizing]="'border-box'"
              [arrowMarginTop]="'1px'"
              [enabled]="enabled(comp)"
              [imageCellTextAlign]="'center'"
              [innerTableWidth]="'100%'"
              [navagationPanelTooltip]="true"
              [margin]="comp.action == 'FirstPage' ? '0 1px 0 3px' : (comp.action == 'Zoom' ? '0 3px 0 1px' : '0px 1px 0 1px')"
              [selected]="selected(comp)"
              (action)="controller.action({ name: comp.action })">
            </sti-button>

            <div *ngIf="comp.type=='separator'"
              [style.height]="model.options.isMobileDevice ? '0.5in' : '35px'"
              [style.margin]="'0px 1px 0 1px'"
              class="stiJsViewerNavigatePanelSeparator"></div>

            <sti-page-control *ngIf="comp.type=='pageControl'"
              [enabled]="!(model.reportParams.pagesCount <= 1 || disableNaviButtons())"
              [count]="model.reportParams.pagesCount"
              [margin]="'0px 1px 0 1px'"
              [textBoxBorder]="'0px'"></sti-page-control>
          </td>
        </tr>
      </tbody>
    </table>
    <div class="stiJsViewerNavigatePanelDisabledPanel" [style.display]="this.model.controls.navigatePanel.enabled ? 'none' : 'block'"></div>
  </div>
  `
})

export class NavigatePanelComponent implements OnInit, AfterViewInit {

  @ViewChild('element') element: ElementRef;

  public comps: ComponentDescription[] = [];

  constructor(public model: ModelService, public controller: ControllerService, public helper: HelperService,
    public pageService: PageService) { }

  ngOnInit() {
    if (this.model.options.toolbar.displayMode === 'Separated') {
      this.initButtons();
    }
    this.controller.getMessage().subscribe((message: Message) => {
      if (message.action !== 'viewer_loaded') {
        this.updateButtons();
      }
    });
    this.model.controls.navigatePanel.visible = this.model.options.toolbar.displayMode === 'Separated';
  }

  ngAfterViewInit(): void {
    this.model.controls.navigatePanel.el = this.element;
  }

  updateButtons() {
    this.comps.filter(i => i.action === 'ViewMode').forEach(m => m.caption = this.model.loc(this.model.reportParams.viewMode));
    this.comps.filter(i => i.action === 'Zoom').forEach((m) => {
      m.menuItems.forEach(n => n.selected = n.name === ('Zoom' + this.model.reportParams.zoom));
      m.caption = Math.round(this.model.reportParams.zoom) + '%';
    });

    this.comps.filter(i => i.action === 'ShowFind').forEach((m) => {
      m.selected = this.model.controls.findPanel.visible;
    });
  }

  initButtons() {
    const comps: ComponentDescription[] = [];
    if (this.model.options.toolbar.showFirstPageButton) {
      comps.push({ type: 'button', action: 'FirstPage', img: 'PageFirst20.png', tooltip: true });
    }
    if (this.model.options.toolbar.showPreviousPageButton) {
      comps.push({ type: 'button', action: 'PrevPage', img: 'PagePrevious20.png', tooltip: true });
    }

    if (this.model.options.toolbar.showCurrentPageControl) {
      comps.push({ type: 'separator' });
      comps.push({ type: 'pageControl' });
      comps.push({ type: 'separator' });
    }

    if (this.model.options.toolbar.showNextPageButton) {
      comps.push({ type: 'button', action: 'NextPage', img: 'PageNext20.png', tooltip: true });
    }

    if (this.model.options.toolbar.showLastPageButton) {
      comps.push({ type: 'button', action: 'LastPage', img: 'PageLast20.png', tooltip: true });
    }

    comps.push({ type: 'space' });
    comps.push({ type: 'button', action: 'ZoomPageWidth', img: 'ZoomPageWidth20.png', tooltip: true });
    comps.push({ type: 'button', action: 'ZoomOnePage', img: 'ZoomOnePage20.png', tooltip: true });

    if (this.model.options.toolbar.showZoomButton) {
      comps.push({ type: 'separator' });
      comps.push({
        type: 'button', action: 'Zoom', caption: '100%', tooltip: true, arrow: 'Arrows.SmallArrowUpWhite.png',
        menuItems: this.helper.getZoomMenuItems()
      });
    }

    this.comps = comps;
  }

  enabled(desc: ComponentDescription): boolean {
    const disableNaviButtons = this.disableNaviButtons();
    switch (desc.action) {
      case 'FirstPage':
      case 'PrevPage':
        return this.model.reportParams.pageNumber > 0 && !disableNaviButtons;
      case 'NextPage':
      case 'LastPage':
        return this.model.reportParams.pageNumber < this.model.reportParams.pagesCount - 1 && !disableNaviButtons;
    }
    return true;
  }

  selected(desc: ComponentDescription): boolean {
    switch (desc.action) {
      case 'ZoomPageWidth':
        return Math.round(this.model.reportParams.zoom) === Math.round(this.pageService.getZoomByPageWidth());
      case 'ZoomOnePage':
        return Math.round(this.model.reportParams.zoom) === Math.round(this.pageService.getZoomByPageHeight());
    }
    return false;
  }

  disableNaviButtons(): boolean {
    return this.model.reportParams.viewMode === 'MultiplePages' || this.model.reportParams.viewMode === 'WholeReport' ||
      (this.model.reportParams.viewMode === 'Continuous' && !this.model.options.appearance.scrollbarsMode && !this.model.options.appearance.fullScreenMode);
  }
}

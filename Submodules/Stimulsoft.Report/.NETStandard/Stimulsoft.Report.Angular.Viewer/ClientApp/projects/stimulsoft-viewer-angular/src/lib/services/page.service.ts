import { Injectable } from '@angular/core';
import { ModelService } from './model.service';
import { HelperService } from './helper.service';
import { Rectangle } from './objects';

@Injectable()
export class PageService {

  constructor(private model: ModelService, private helper: HelperService) {
    Object.keys(model.controls).forEach(k => model.controls[k].pageService = this);
  }

  public calculateLayout() {
    setTimeout(() => {
      this.calculateLayoutInner();
    });
  }

  public calculateLayoutInner() {
    const reportLayout: Rectangle = new Rectangle();
    const paramsLayout: Rectangle = new Rectangle();

    if (!this.model.controls.reportPanel.el) {
      return reportLayout;
    }

    if (this.model.controls.dashboardsPanel.el) {
      reportLayout.top += this.model.controls.dashboardsPanel.offsetHeight;
    }

    if (this.model.reportParams.type === 'Report') {
      if (this.model.controls.toolbar && this.model.controls.toolbar.visible && !(this.model.options.isMobileDevice && this.model.options.toolbar.autoHide)) {
        reportLayout.top += this.model.controls.toolbar.offsetHeight;
      }

      if (this.model.controls.drillDownPanel && this.model.controls.drillDownPanel.visible) {
        reportLayout.top += this.model.controls.drillDownPanel.offsetHeight;
      }
      if (this.model.controls.findPanel && this.model.controls.findPanel.visible) {
        reportLayout.top += this.model.controls.findPanel.offsetHeight;
      }
      if (this.model.controls.resourcesPanel && this.model.controls.resourcesPanel.visible) {
        reportLayout.top += this.model.controls.resourcesPanel.offsetHeight;
      }

      if (this.model.controls.bookmarksPanel && this.model.controls.bookmarksPanel.visible) {
        reportLayout.left += this.model.options.appearance.bookmarksTreeWidth;
        if (this.model.options.toolbar.displayMode === 'Simple') {
          reportLayout.left += 2;
        }
      }

      if (this.model.controls.navigatePanel && this.model.controls.navigatePanel.visible && !(this.model.options.isMobileDevice && this.model.options.toolbar.autoHide)) {
        reportLayout.bottom = this.model.controls.navigatePanel.offsetHeight;
      }
    }

    if (this.model.controls.parametersPanel && this.model.controls.parametersPanel.visible) {
      this.model.controls.parametersPanel.layout = paramsLayout;
      paramsLayout.top = reportLayout.top;

      if (this.model.options.appearance.parametersPanelPosition === 'Left') {
        paramsLayout.left = reportLayout.left;
        paramsLayout.width = this.model.controls.parametersPanel.el.nativeElement.firstChild.offsetWidth;
        reportLayout.left += paramsLayout.width;
        if (this.model.options.toolbar.displayMode === 'Simple') {
          reportLayout.left += 2;
        }
      }

      if (this.model.options.appearance.parametersPanelPosition === 'Top') {
        paramsLayout.height = this.model.controls.parametersPanel.offsetHeight;
        reportLayout.top += paramsLayout.height;
      }
    }

    if (this.model.controls.bookmarksPanel) {
      // this.model.controls.bookmarksPanel.layout = new Rectangle();
      let styleTop = this.model.options.toolbar.visible ? this.model.controls.toolbar.offsetHeight : 0;
      if (this.model.options.isMobileDevice && this.model.options.toolbar.autoHide) {
        styleTop = 0;
      }
      styleTop += this.model.controls.parametersPanel.exists && this.model.options.appearance.parametersPanelPosition === 'Top' ? this.model.controls.parametersPanel.offsetHeight : 0;
      styleTop += this.model.controls.findPanel.exists ? this.model.controls.findPanel.offsetHeight : 0;
      styleTop += this.model.controls.drillDownPanel.exists ? this.model.controls.drillDownPanel.offsetHeight : 0;
      styleTop += this.model.controls.resourcesPanel.exists ? this.model.controls.resourcesPanel.offsetHeight : 0;

      this.model.controls.bookmarksPanel.layout.top = styleTop; // reportLayout.top;
    }

    if (this.model.options.toolbar.displayMode === 'Simple' && reportLayout.top > 0) {
      reportLayout.top += 2;
    }

    if (this.model.controls.reportPanel.el?.nativeElement.style.position === 'relative') {
      reportLayout.top = paramsLayout.height;
    }

    const reportMargins = {
      top: parseInt(this.model.controls.reportPanel.el.nativeElement.style.marginTop ? this.model.controls.reportPanel.el.nativeElement.style.marginTop : 0),
      right: parseInt(this.model.controls.reportPanel.el.nativeElement.style.marginRight ? this.model.controls.reportPanel.el.nativeElement.style.marginRight : 0),
      bottom: parseInt(this.model.controls.reportPanel.el.nativeElement.style.marginBottom ? this.model.controls.reportPanel.el.nativeElement.style.marginBottom : 0),
      left: parseInt(this.model.controls.reportPanel.el.nativeElement.style.marginLeft ? this.model.controls.reportPanel.el.nativeElement.style.marginLeft : 0)
    };

    reportLayout.width = this.model.controls.reportPanel.offsetWidth - reportLayout.left - reportLayout.right + reportMargins.left + reportMargins.right;
    reportLayout.height = this.model.controls.reportPanel.el.nativeElement.style.position === 'absolute'
      ? this.model.controls.reportPanel.offsetHeight - reportLayout.top - reportLayout.bottom + reportMargins.top + reportMargins.bottom
      : Math.round(reportLayout.width * 0.56); // use 16:9 aspect ratio for automatic height

    this.model.controls.reportPanel.layout = reportLayout;
  }

  public getZoomByPageWidth(): number {
    const pageNumber = this.model.reportParams.viewMode === 'SinglePage' ? 0 : (this.model.reportParams.pageNumber || 0);
    return (this.model.controls.reportPanel.layout.width - 40) * this.getZoom() / (this.model.pages[pageNumber]?.page?.offsetWidth || 1);
  }

  public getZoomByPageHeight(): number {
    const pageNumber = this.model.reportParams.viewMode === 'SinglePage' ? 0 : (this.model.reportParams.pageNumber || 0);
    return (this.model.controls.reportPanel.layout.height - 40) * this.getZoom() / (this.model.pages[pageNumber]?.page?.offsetHeight || 1);
  }

  private getZoom(): number {
    return this.model.reportParams.zoom < 0 ? 100 : this.model.reportParams.zoom;
  }

}

import { Component, OnInit, AfterViewInit, ElementRef, ViewChild } from '@angular/core';
import { ModelService } from '../services/model.service';
import { HelperService } from '../services/helper.service';
import { DashboardService } from '../services/dashboard.service';

@Component({
  selector: 'sti-dashboards-panel',
  template: `
    <div #element
      [class]="'stiJsViewerToolBar' + (model.options.toolbar.displayMode == 'Separated' ? ' stiJsViewerToolBarSeparated' : '')"
      [style.fontFamily]="model.options.toolbar.fontFamily"
      [style.fontColor]="helper.val(model.options.toolbar.fontColor)"
      [style.display]="model.controls.dashboardsPanel.visible ? '' : 'none'"
      [style.background]="dbsMode ? 'transparent' : ''"
      [style.borderColor]="dbsMode ? 'transparent' : ''">
      <div [style.paddingTop]="model.options.toolbar.displayMode == 'Simple' ? '2px' : ''">
        <table class="stiJsViewerToolBarTable" cellpadding="0" cellspacing="0" style="margin: 0px; box-sizing: border-box;"
          [style.border]="model.options.toolbar.displayMode == 'Separated' ? '0px' : ''"
          [style.background]="dbsMode ? 'transparent' : ''"
          [style.borderColor]="dbsMode ? 'transparent' : ''">
          <tbody>
            <tr class="stiJsViewerClearAllStyles">
              <td class="stiJsViewerClearAllStyles" style="width: 100%;">
                <table class="stiJsViewerToolBarTable" cellpadding="0" cellspacing="0"
                  [style.align]="model.options.appearance.rightToLeft ? 'right' : (model.options.toolbar.alignment == 'default' ? 'left' : model.options.toolbar.alignment)">
                  <tbody>
                      <tr>
                         <td *ngFor="let info of model.reportParams.dashboards; let i=index">
                            <sti-dashboard-button
                              [info]="info"
                              [dbsMode]="dbsMode"
                              [display]="dashboardsCount == 1 && reportsCount == 0 ? 'none': null">
                            </sti-dashboard-button>
                        </td>
                      </tr>
                  </tbody>
                </table>
              </td>

              <!-- actions table -->
              <td class="stiJsViewerClearAllStyles">
                <table class="stiJsViewerToolBarTable" cellpadding="0" cellspacing="0" style="margin-right:2px">
                  <tbody>
                      <tr>
                        <td *ngIf="model.options.toolbar.showRefreshButton && model.options.toolbar.visible">
                          <sti-button [imageName]="imagesPath + 'Refresh.png'"
                            [tooltip]="model.loc('Refresh')"
                            [helpLink]="helper.helpLinks['DashboardToolbar']"
                            [margin]="'2px 0 2px 2px'"
                            [actionName]="'Refresh'"
                            [height]="model.options.toolbar.displayMode == 'Separated' ? (dbsMode ? (model.options.isTouchDevice ? '28px' : '23px') : '28px') : null"
                            [width]="model.options.toolbar.displayMode == 'Separated' ? (dbsMode ? (model.options.isTouchDevice ? '28px' : '23px') : '28px') : null"
                            [innerTableWidth]="model.options.toolbar.displayMode == 'Separated' ? '100%': null"
                            [imageCellTextAlign]="model.options.toolbar.displayMode == 'Separated' ? 'center': null"
                            [display]="previewSettings.dashboardToolBar && previewSettings.dashboardRefreshButton ? '' : 'none'">
                          </sti-button>
                        </td>

                        <td *ngIf="model.options.toolbar.showParametersButton && model.options.toolbar.visible">
                          <sti-button [imageName]="imagesPath + 'Parameters.png'"
                            [tooltip]="model.loc('Parameters')"
                            [helpLink]="helper.helpLinks['DashboardToolbar']"
                            [margin]="'2px 0 2px 2px'"
                            [actionName]="'Parameters'"
                            [height]="model.options.toolbar.displayMode == 'Separated' ? (dbsMode ? (model.options.isTouchDevice ? '28px' : '23px') : '28px') : null"
                            [width]="model.options.toolbar.displayMode == 'Separated' ? (dbsMode ? (model.options.isTouchDevice ? '28px' : '23px') : '28px') : null"
                            [innerTableWidth]="model.options.toolbar.displayMode == 'Separated' ? '100%': null"
                            [imageCellTextAlign]="model.options.toolbar.displayMode == 'Separated' ? 'center': null">
                          </sti-button>
                        </td>

                        <td *ngIf="model.options.toolbar.showOpenButton && model.options.toolbar.visible">
                          <sti-button [imageName]="imagesPath + 'Open.png'"
                            [tooltip]="model.loc('Open')"
                            [helpLink]="helper.helpLinks['DashboardToolbar']"
                            [margin]="'2px 0 2px 2px'"
                            [actionName]="'OpenDashboard'"
                            [height]="model.options.toolbar.displayMode == 'Separated' ? (dbsMode ? (model.options.isTouchDevice ? '28px' : '23px') : '28px') : null"
                            [width]="model.options.toolbar.displayMode == 'Separated' ? (dbsMode ? (model.options.isTouchDevice ? '28px' : '23px') : '28px') : null"
                            [innerTableWidth]="model.options.toolbar.displayMode == 'Separated' ? '100%': null"
                            [imageCellTextAlign]="model.options.toolbar.displayMode == 'Separated' ? 'center': null"
                            [display]="previewSettings.dashboardToolBar && previewSettings.dashboardOpenButton ? '' : 'none'">
                          </sti-button>
                        </td>

                        <td *ngIf="model.options.toolbar.showDesignButton && model.options.toolbar.visible">
                          <sti-button [imageName]="imagesPath + 'Edit.png'"
                            [tooltip]="model.loc('Edit')"
                            [helpLink]="helper.helpLinks['DashboardToolbar']"
                            [margin]="'2px 0 2px 2px'"
                            [actionName]="'postDesign'"
                            [height]="model.options.toolbar.displayMode == 'Separated' ? (dbsMode ? (model.options.isTouchDevice ? '28px' : '23px') : '28px') : null"
                            [width]="model.options.toolbar.displayMode == 'Separated' ? (dbsMode ? (model.options.isTouchDevice ? '28px' : '23px') : '28px') : null"
                            [innerTableWidth]="model.options.toolbar.displayMode == 'Separated' ? '100%': null"
                            [imageCellTextAlign]="model.options.toolbar.displayMode == 'Separated' ? 'center': null"
                            [display]="previewSettings.dashboardToolBar && previewSettings.dashboardEditButton ? '' : 'none'">
                          </sti-button>
                        </td>

                        <td *ngIf="model.options.toolbar.showFullScreenButton && model.options.toolbar.visible">
                          <sti-button [imageName]="imagesPath + 'CloseFullScreen.png'"
                            [caption]="model.loc('Close')"
                            [tooltip]="model.loc('FullScreen')"
                            [helpLink]="helper.helpLinks['DashboardToolbar']"
                            [margin]="'2px 0 2px 2px'"
                            [actionName]="'postFullScreen'"
                            [height]="model.options.toolbar.displayMode == 'Separated' ? (dbsMode ? (model.options.isTouchDevice ? '28px' : '23px') : '28px') : null"
                            [innerTableWidth]="model.options.toolbar.displayMode == 'Separated' ? '100%': null"
                            [imageCellTextAlign]="model.options.toolbar.displayMode == 'Separated' ? 'center': null"
                            [display]="previewSettings.dashboardToolBar && previewSettings.dashboardFullScreenButton ? '' : 'none'">
                          </sti-button>
                        </td>

                        <td *ngIf="model.options.toolbar.showSaveButton && model.options.toolbar.visible">
                          <sti-button [imageName]="imagesPath + 'Menu.png'"
                            [tooltip]="model.loc('Save')"
                            [helpLink]="helper.helpLinks['DashboardToolbar']"
                            [margin]="'2px 0 2px 2px'"
                            [actionName]="'ExportDashboard'"
                            [height]="model.options.toolbar.displayMode == 'Separated' ? (dbsMode ? (model.options.isTouchDevice ? '28px' : '23px') : '28px') : null"
                            [width]="model.options.toolbar.displayMode == 'Separated' ? (dbsMode ? (model.options.isTouchDevice ? '28px' : '23px') : '28px') : null"
                            [innerTableWidth]="model.options.toolbar.displayMode == 'Separated' ? '100%': null"
                            [imageCellTextAlign]="model.options.toolbar.displayMode == 'Separated' ? 'center': null"
                            [display]="previewSettings.dashboardToolBar && previewSettings.dashboardMenuButton && ((previewSettings.dashboardShowReportSnapshots && !model.options.jsMode) || previewSettings.dashboardShowExports) ? '' :'none'">
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

export class DashboardsPanelComponent implements OnInit, AfterViewInit {

  @ViewChild('element') element: ElementRef;

  public imagesPath = 'Dashboards.Actions.Light.';

  constructor(public model: ModelService, public helper: HelperService, public dashboardService: DashboardService) { }

  ngOnInit() { }

  ngAfterViewInit(): void {
    this.model.controls.navigatePanel.el = this.element;
  }


  get dbsMode(): boolean {
    return this.dashboardsCount > 0 && this.reportsCount === 0;
  }

  get dashboardsCount(): number {
    let count = 0;
    if (this.model?.reportParams?.dashboards) {
      (this.model.reportParams.dashboards as []).forEach(i => {
        if (i['type'] === 'Dashboard') {
          count++;
        }
      });
    }
    return count;
  }

  get reportsCount(): number {
    if (this.model?.reportParams?.dashboards) {
      return (this.model.reportParams.dashboards as []).length - this.dashboardsCount;
    }
    return 0;
  }

  get previewSettings(): any {
    return this.model.reportParams.previewSettings || {};
  }
}

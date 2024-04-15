import { Injectable } from '@angular/core';
import { DrillDown, Message } from './objects';
import { ControllerService } from './controller.service';
import { ModelService } from './model.service';
import { HelperService } from './helper.service';

@Injectable()
export class DrillDownService {

  constructor(public controller: ControllerService, public model: ModelService, public helper: HelperService) {
    controller.getMessage().subscribe((message: Message) => {
      if (message.action !== 'InitVars' && message.action !== 'viewer_loaded') {
        this.model.options.drillDownInProgress = false;
        const parameters = message.data;

        if (this.model.drillDownButtons.length === 0) {
          this.addButton(parameters.reportFileName, {});
        }

        if (message.action === 'DrillDown') {
          const useDbsDrillDownPanel = false; // jsObject.controls.dashboardsPanel && jsObject.controls.dashboardsPanel.visible && jsObject.controls.dashboardsPanel.selectedButton;
          if (useDbsDrillDownPanel) {
            // jsObject.controls.dashboardsPanel.addDrillDownButton(null, parameters.drillDownGuid, parameters.drillDownParameters, parameters.previewSettings, parameters.reportFileName);
          }
          this.showDrillDownPage(parameters.reportFileName, parameters.drillDownGuid, parameters.drillDownParameters, useDbsDrillDownPanel);
        }
      }
    });

  }

  private showDrillDownPage(reportFileName: string, drillDownGuid: string, drillDownParameters: any, useDbsDrillDownPanel: boolean) {
    if (useDbsDrillDownPanel) { return; }

    let buttonExist = false;
    this.model.drillDownButtons.forEach((b: DrillDown) => {
      if (b.reportParams.drillDownGuid === drillDownGuid) {
        this.model.drillDownButtons.forEach(bt => bt.selected = false);
        buttonExist = b.selected = b.visible = true;
      }
    });

    if (!buttonExist) {
      this.addButton(reportFileName);
      this.model.reportParams.drillDownParameters = drillDownParameters;
      this.model.reportParams.pageNumber = 0;
      this.model.reportParams.pagesWidth = 0;
      this.model.reportParams.pagesHeight = 0;
    }
  }

  public addButton(caption: string, reportParams: any = {}) {
    this.model.drillDownButtons.forEach(b => b.selected = false);
    this.model.drillDownButtons.push({ caption, selected: true, reportParams, visible: true });
    this.updateVisibility();
  }

  public updateVisibility() {
    this.model.controls.drillDownPanel.visible = this.model.drillDownButtons.length > 1;
  }

  public saveState() {
    const sButton = this.model.drillDownButtons.find(b => b.selected);
    if (sButton) {
      sButton.reportParams = this.model.getReportParams();
    }
  }

}

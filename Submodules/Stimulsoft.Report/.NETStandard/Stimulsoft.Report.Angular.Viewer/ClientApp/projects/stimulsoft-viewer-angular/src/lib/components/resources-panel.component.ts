import { Component, OnInit, AfterViewInit, ViewChild, ElementRef } from '@angular/core';
import { ModelService } from '../services/model.service';
import { ControllerService } from '../services/controller.service';
import { Message, Resource } from '../services/objects';
import { HelperService } from '../services/helper.service';
import { MenuItem } from '../menu/meni-item.component';

@Component({
  selector: 'sti-resources-panel',
  template: `
    <div #element style="z-index: 3"
      [style.display]="this.model.controls.resourcesPanel.visible ? '' : 'none'"
      [style.fontFamily]="model.options.toolbar.fontFamily"
      [style.fontColor]="model.options.toolbar.fontColor"
      [class]="'stiJsViewerToolBar' + (model.options.toolbar.displayMode == 'Separated' ? ' stiJsViewerToolBarSeparated' : '')">
      <div [style.paddingTop]="model.options.toolbar.displayMode == 'Simple' ? '2px' : ''">
        <div class="stiJsViewerToolBarTable" style="box-sizing: border-box; display: table;"
          [style.border]="model.options.toolbar.displayMode == 'Separated' ? '0px' : ''">
          <table class="stiJsViewerClearAllStyles" cellpadding="0" cellspacing="0">
            <tbody>
              <tr class="stiJsViewerClearAllStyles">
                <td *ngFor="let resource of model.reportParams.resources" class="stiJsViewerClearAllStyles">
                  <sti-button [caption]="resource.name"
                    [caption2]="helper.getHumanFileSize(resource.size, 1)"
                    [imageName]="getResourceImage(resource.type)"
                    [styleName]="'stiJsViewerFormButton'"
                    [height]="'auto'"
                    [margin]="'3px 0 3px 3px'"
                    [innerTableWidth]="'100%'"
                    [minWidth]="'80px'"
                    [captionAlign]="'left'"
                    [captionPaddingLeft]="'3px'"
                    [captionMaxWidth]="'150px'"
                    [captionLineHeight]="'14px'"
                    [captionWhiteSpace]="'nowrap'"
                    [captionOverflow]="'hidden'"
                    [captionTextOverflow]="'ellipsis'"
                    [imageCellWidth]="'1px'"
                    [imageCellPadding]="'4px 8px 4px 4px'"
                    [menuItems]="getMenuItems(resource)"
                    [resourceButton]="true"
                    [imageSizesWidth]="32"
                    [imageSizesHeight]="32"
                    [actionName]="getActionName(resource)"
                    (action)="action(resource)">
                  </sti-button>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>
  `
})

export class ResourcesPanelComponent implements OnInit, AfterViewInit {

  @ViewChild('element') element: ElementRef;

  constructor(public model: ModelService, public controller: ControllerService, public helper: HelperService) {
    controller.getMessage().subscribe((message: Message) => {
      if (message.action === 'GetReport' || message.action === 'OpenReport') {
        this.model.controls.resourcesPanel.visible = this.model.reportParams.resources?.length > 0;
      }
    });
  }

  ngOnInit() { }

  ngAfterViewInit(): void {
    this.model.controls.resourcesPanel.el = this.element;
  }

  action(resource: Resource) {
    const resTypesAllowedViewInBrowser = ['Image', 'Pdf', 'Txt'];
    const viewType = resTypesAllowedViewInBrowser.indexOf(resource.type) >= 0 ? 'View' : 'SaveFile';
    this.controller.postReportResource(resource.name, this.model.options.jsMode ? 'SaveFile' : viewType);
  }

  getResourceImage(resourceType: string): string {
    if (this.model.img('BigResource' + resourceType + '.png') !== '') {
      return 'BigResource' + resourceType + '.png';
    } else {
      return 'BigResource.png';
    }
  }

  getMenuItems(resource: Resource): MenuItem[] {
    const items: MenuItem[] = [];
    items.push({ name: 'Resource' + 'View', caption: this.model.loc('ButtonView'), value: resource });
    items.push({ name: 'Resource' + 'SaveFile', caption: this.model.loc('SaveFile'), value: resource });
    return items;
  }

  getActionName(resource: Resource): string {
    if (!resource.id) {
      resource.id = Math.random().toString();
    }
    return 'resource' + resource.name + resource.id;
  }
}

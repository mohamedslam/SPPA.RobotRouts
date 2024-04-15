import { Component, OnInit, Input, ElementRef, ViewChild, AfterViewInit } from '@angular/core';
import { ModelService } from '../services/model.service';
import { ToolbarService } from '../services/toolbar.service';
import { ControllerService } from '../services/controller.service';
import { ComponentDescription } from '../services/objects';

@Component({
  selector: 'sti-toolbar-dop',
  template: `
    <table #mainTable
      class="stiJsViewerClearAllStyles"
      cellpadding="0"
      cellspacing="0"
      style="margin: 1px;"
      [attr.align]="align"
      [style.marginLeft]="marginLeft">
      <tbody>
        <tr class="stiJsViewerClearAllStyles">
          <td *ngFor="let comp of comps" class="stiJsViewerClearAllStyles">
            <sti-button *ngIf="comp.type=='button'"
              [caption]="model.options.toolbar.showButtonCaptions ? comp.caption : null"
              [imageName]="comp.img"
              [arrow]="!model.options.isMobileDevice ? comp.arrow : null"
              [menuItems]="comp.menuItems"
              [actionName]="comp.action"
              [tooltip]="comp.tooltip"
              [enabled]="toolbarService.enabled(comp)"
              [display]="toolbarService.display(comp)"
              [innerTableWidth]="toolbarService.getInnerTableWidth(comp)"
              [width]="toolbarService.getButonWidth(comp)"
              [height]="model.options.toolbar.displayMode == 'Separated' ? (model.options.isMobileDevice ? '0.5in' : '28px') : null"
              (action)="controller.action({ name: comp.action });"
              [selected]="toolbarService.selected(comp)"></sti-button>
            <sti-toolbar-separator *ngIf="comp.type=='separator'"></sti-toolbar-separator>
            <sti-page-control *ngIf="comp.type=='pageControl'" [enabled]="!(model.reportParams.pagesCount <= 1 || toolbarService.disableNaviButtons())" [count]="model.reportParams.pagesCount" ></sti-page-control>
        </td>
      </tr>
    </tbody>
  </table>
  `
})

export class ToolbarDopComponent implements OnInit, AfterViewInit {

  @ViewChild('mainTable') mainTable: ElementRef;

  @Input() comps: ComponentDescription[];
  @Input() align: string;

  public marginLeft = '1px';

  viewInit = false;

  constructor(public model: ModelService, public controller: ControllerService, public toolbarService: ToolbarService) { }

  ngAfterViewInit(): void {
    this.viewInit = true;

    setTimeout(() => {
      const width = this.mainTable?.nativeElement?.offsetWidth;
      this.marginLeft = this.viewInit && this.model.options.toolbar.alignment === 'Center' && width ? `calc(50% - ${Math.trunc(width / 2)}px)` : '1px';
    });

  }

  ngOnInit() { }

}

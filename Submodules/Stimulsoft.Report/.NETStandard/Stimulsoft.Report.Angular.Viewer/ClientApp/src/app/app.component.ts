import { AfterViewInit, Component, ViewChild } from '@angular/core';
import { StimulsoftViewerComponent } from 'projects/stimulsoft-viewer-angular/src/public-api';
import { StimulsoftDesignerComponent } from 'projects/stimulsoft-designer-angular/src/public-api';

@Component({
  selector: 'app-root',
  template: `
    <input type='button' (click)="showViewer = !showViewer" value="Viewer">
    <input type='button' (click)="showDesigner = !showDesigner" value="Designer">
    <input type='button' (click)="designerHeight = designerHeight + 100" value="DesignerHeight">
    <input type='button' (click)="refresh()" value="Refresf">
    <input type='button' (click)="reload()" value="Reload">
    <br>
    <stimulsoft-viewer-angular *ngIf="showViewer" #viewer [requestUrl]="isNetCore ? netCoreViewer : netViewer" [action]="'InitViewer'" [properties]="properties" [height]="height" [style]="''"
    [postParametersFunction]="getPostParameters"
    (error)="error($event)" (export)="export($event)" (email)="email($event)" (loaded)="loaded($event)" (print)="print($event)"></stimulsoft-viewer-angular>

    <!--stimulsoft-viewer-angular *ngIf="showDesigner" #viewer2 [requestUrl]="isNetCore ? netCoreViewer : netViewer" [action]="'InitViewer'" [properties]="properties2" [height]="height" [style]="''"
    (error)="error($event)" (export)="export($event)" (email)="email($event)" (loaded)="loaded($event)" (print)="print($event)"></stimulsoft-viewer-angular-->

    <stimulsoft-designer-angular *ngIf="showDesigner" #designer [requestUrl]="isNetCore ? netCoreDesigner : netDesigner" [width]="'100%'" [height]="designerHeight + 'px'" (designerLoaded)="designerLoaded()">
      Loading...
    </stimulsoft-designer-angular>
  `
})
export class AppComponent {
  netCoreViewer = 'http://localhost:60015/Viewer/{action}';
  netViewer = 'http://localhost:60015/ViewerAngular/{action}';

  netCoreDesigner = 'http://localhost:60015/api/designer';
  netDesigner = 'http://localhost:60015/DesignerAngular/Get';

  reports = ['interactions/BookmarksAndHyperlinks.mrt', 'attach.mrt', 'ListOfProducts.mrt', 'EditableReport.mrt', 'Variables.mrt', 'ParametersInvoice.mrt', 'MasterDetail.mrt', 'Report.mrt'];
  title = 'ClientApp';
  properties = { reportName: this.reports[2] };
  properties2 = { reportName: this.reports[0] };
  height = '600px';
  designerHeight = 600;
  showDesigner = false;
  showViewer = true;
  isNetCore = true;

  @ViewChild('viewer') viewer: StimulsoftViewerComponent;
  @ViewChild('designer') designer: StimulsoftDesignerComponent;

  constructor() { }

  public getPostParameters(data): any {
    return { param: "value" };
  }

  updateConfig(reportName: string) {
    this.properties = { reportName };
  }

  error(error: any) {
    console.error(error.error);
  }

  export(data: any) {
    console.log(data);
  }

  print(data: any) {
    console.log(data);
  }

  email(data: any) {
    console.log(data);
  }

  loaded(data: any) {
    console.log('loaded');
  }

  designerLoaded() {
    this.designer.designerEl.nativeElement.firstChild.jsObject.options.haveSaveAsEvent = true;
    console.log('loaded designer');
  }

  refresh() {
    this.viewer.exportService.controller.post("Refresh");
  }

  reload() {
    this.viewer.ngAfterViewInit();
  }
}

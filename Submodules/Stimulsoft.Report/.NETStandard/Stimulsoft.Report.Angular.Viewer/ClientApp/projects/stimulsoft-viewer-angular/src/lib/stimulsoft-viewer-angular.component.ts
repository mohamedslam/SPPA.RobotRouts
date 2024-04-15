import { Component, OnInit, Input, AfterViewInit, ViewChild, ElementRef, OnChanges, Output, EventEmitter } from '@angular/core';
import { ControllerService } from './services/controller.service';
import { MenuService } from './menu/menu.service';
import { ModelService } from './services/model.service';
import { Message } from './services/objects';
import { ExportService } from './services/export.service';
import { FormService } from './forms/form.service';
import { ApiService } from './services/api.service';
import { PrintService } from './services/print.service';
import { DashboardService } from './services/dashboard.service';
import { StiHttpClientService } from './services/http-client.service';
import { StylesService } from './services/styles.service';
import { HelperService } from './services/helper.service';
import { MouseService } from './services/mouse.service';
import { PageService } from './services/page.service';
import { TooltipService } from './services/tooltip.service';
import { FindService } from './services/find.service';
import { AnimationService } from './services/animation.service';
import { InteractionsService } from './services/interactions.service';
import { RadioButtonService } from './services/radio-button.service';
import { CollectionsService } from './services/collections.service';
import { MailService } from './services/mail.service';
import { EditableFieldsService } from './services/editable-fields.service';
import { DrillDownService } from './services/drill-down.service';
import { Md5Service } from './services/md5.service';
import { FullScreenService } from './services/full-screen.service';
import { ToolbarService } from './services/toolbar.service';
//1bfb351c1add27b1537ffce9c723e3cb1f5b27bb 02.03.2021 17:29:20

@Component({
  selector: 'stimulsoft-viewer-angular',
  template: `
    <span #viewer [style]="style"
      [style.top.px]="0"
      [style.right.px]="0"
      [style.bottom.px]="0"
      [style.left.px]="0"
      [style.display]="'inline-block'"
      [style.backgroundColor]="backgroundColor"
      [style.width]="this.width != null ? this.width : '100%'"
      [style.height]="this.height != null ? this.height : (model.options?.appearance.scrollbarsMode ? '650px' : '100%')">
      <div class="stiJsViewerMainPanel">
        <sti-center-text *ngIf="initialized"></sti-center-text>
        <sti-toolbar *ngIf="initialized"></sti-toolbar>
        <div *ngFor="let i of [1,2,3,4,5]" [style.zIndex]="i * 10" [style.display]="formService.form?.level == i ? '' : 'none'" class="stiJsViewerDisabledPanel"></div>
        <sti-about-panel *ngIf="initialized"></sti-about-panel>

        <sti-dashboards-panel *ngIf="initialized"></sti-dashboards-panel>
        <sti-report-panel *ngIf="initialized"></sti-report-panel>
        <sti-progress *ngIf="initialized"></sti-progress>
        <sti-find-panel *ngIf="initialized"></sti-find-panel>
        <sti-drill-down-panel *ngIf="initialized"></sti-drill-down-panel>
        <sti-resources-panel *ngIf="initialized"></sti-resources-panel>
        <sti-navigate-panel *ngIf="initialized"></sti-navigate-panel>

        <sti-export-form *ngIf="initialized" [exportFormSettings]="exportService.exportFormSettings"></sti-export-form>
        <sti-send-email-form *ngIf="initialized"></sti-send-email-form>
        <sti-password-form *ngIf="initialized"></sti-password-form>
        <sti-error-message-form *ngIf="initialized"></sti-error-message-form>
        <sti-notification-form *ngIf="initialized"></sti-notification-form>

        <sti-tooltip *ngIf="initialized"></sti-tooltip>
        <sti-menu *ngFor="let menu of menuService.menus" [menu]="menu"></sti-menu>
        <sti-bookmarks-panel *ngIf="initialized"></sti-bookmarks-panel>
        <sti-interactions-panel *ngIf="initialized"></sti-interactions-panel>

        <sti-open-dialog *ngIf="initialized" [fileMask]="model.openDialogFileMask"></sti-open-dialog>
      </div>
    </span>
  `,
  providers: [ModelService, StiHttpClientService, ControllerService, StylesService, HelperService, MenuService,
    MouseService, PageService, TooltipService, FindService, AnimationService, Md5Service, InteractionsService, FormService, RadioButtonService, ExportService,
    CollectionsService, MailService, EditableFieldsService, DrillDownService, PrintService, FullScreenService, ToolbarService, ApiService, DashboardService]
})
export class StimulsoftViewerComponent implements OnInit, AfterViewInit, OnChanges {

  /**
   * Root viewer span
   */
  @ViewChild('viewer') viewerElement: ElementRef;

  /**
   * Occurs when report/dashboard loaded
   */
  @Output() loaded: EventEmitter<any> = new EventEmitter();

  /**
   * Occurs on error, $event is ErrorMessage object contains error: string & type: any  (if present)
   */
  @Output() error: EventEmitter<any> = new EventEmitter();

  /**
   * Occurs on export, $event object contains exportFormat: string & exportSettings: {}
   */
  @Output() export: EventEmitter<any> = new EventEmitter();

  /**
   * Occurs on export & email, $event object contains exportFormat: string & exportSettings: {}
   */
  @Output() email: EventEmitter<any> = new EventEmitter();

  /**
   * Occurs on export & email, $event object contains format: string : 'PrintPdf' or 'PrintWithoutPreview' or 'PrintWithPreview'
   */
  @Output() print: EventEmitter<any> = new EventEmitter();

  /**
   * Occurs on pressing 'Design' button
   */
  @Output() design: EventEmitter<any> = new EventEmitter();

  /**
   * Url to server instance, must contains placeholder {action} that will replace with action
   * Example: http://server.url:51528/Viewer/{action}
   */
  @Input() requestUrl: string;

  /**
   * Controller action that handle viewer initial request
   */
  @Input() action: string;

    /**
     * Properties that will transfer to controller action as JSON object
     */
    @Input() properties: any;

  /**
   * Viewer width
   */
  @Input() width: string;

  /**
   * Viewer height
   */
  @Input() height: string;

  /**
   * Viewer background color
   */
  @Input() backgroundColor = 'White';

  /**
   * Style of viewer applied to main span as [style]="style"
   */
  @Input() style: string;

  @Input() postParametersFunction: any;

  public initialized = false;

  private viewInit = false;

  constructor(private controller: ControllerService, public menuService: MenuService,
    public model: ModelService, public exportService: ExportService, public formService: FormService,
    public api: ApiService, private printService: PrintService, private dashboardService: DashboardService) {

    window.onresize = () => this.resize();

    this.initEvents();
  }

  ngOnChanges(changes: import('@angular/core').SimpleChanges): void {
    this.loadViewer();
  }

  ngOnInit(): void {
    this.model.postParametersFunction = this.postParametersFunction;
    this.controller.getMessage().subscribe((message: Message) => {
      switch (message.action) {
        case 'viewer_loaded':
          this.initialized = true;
          break;
      }
    });
  }

  ngAfterViewInit(): void {
    this.viewInit = true;
    this.loadViewer();
  }

  initEvents() {
    this.controller.getMessage().subscribe((message: Message) => {
      switch (message.action) {
        case 'GetReport':
        case 'OpenReport':
          setTimeout(() => {
            this.loaded.next();
          });
          break;
      }
    });

    this.controller.getActionMessage().subscribe((message: Message) => {
      switch (message.action) {
        case 'Error':
          setTimeout(() => {
            this.error.next(this.model.errorMessage);
          });
          break;

        case 'ExportReport':
          setTimeout(() => {
            this.export.next(message.data);
          });
          break;

        case 'Email':
          setTimeout(() => {
            this.email.next(message.data);
          });
          break;

        case 'Print':
          setTimeout(() => {
            this.print.next(message.data);
          });
          break;

        case 'Design':
          setTimeout(() => {
            this.design.next();
          });
          break;
      }
    });
  }

  loadViewer() {
    if (this.viewInit) {
      this.model.requestUrl = this.requestUrl;
      this.model.action = this.action;
      this.model.properties = this.properties;
      this.controller.loadViewer();
      this.resize();
      this.model.controls.viewer.el = this.viewerElement;
    }
  }

  resize() {
    if (this.viewerElement?.nativeElement) {
      this.model.viewerSize = { width: this.viewerElement.nativeElement.offsetWidth, height: this.viewerElement.nativeElement.offsetHeight };
      this.controller.viewerResized();
    }
  }
}

import { Component, OnInit, ViewChild, ElementRef, AfterViewInit } from '@angular/core';
import { ControllerService } from '../services/controller.service';
import { ModelService } from '../services/model.service';
import { StylesService } from '../services/styles.service';
import { InteractionsService } from '../services/interactions.service';
import { Message } from '../services/objects';

@Component({
  selector: 'sti-report-panel',
  template: `
    <div #element
         [style.textAlign]="model.options.appearance.pageAlignment == 'default' ? 'center' : model.options.appearance.pageAlignment"
         [style.top]="0"
         [style.bottom]="0"
         [style.position]="position"
         [style.height]="model.options.heightType != 'Percentage' || model.options.appearance.scrollbarsMode ? 'auto' : 'calc(100% - 35px)'"
         [style.overflow] = "model.reportParams.type == 'Report' && model.options.appearance.scrollbarsMode ? 'auto' : 'hidden'"
         [style.marginTop.px]="model.controls.reportPanel.layout.top"
         [style.marginLeft.px]="model.controls.reportPanel.layout.left"
         [style.marginBottom.px]="position == 'absolute' ? model.controls.reportPanel.layout.bottom : 0"
         [style.paddingBottom.px]="position == 'relative' ? model.controls.reportPanel.layout.bottom : 0"
         [style.transition]="model.options.isMobileDevice ? 'margin 200ms ease' : null"
         (scroll)="scroll()"
         class="stiJsViewerReportPanel"
         (touchstart)="eventTouchStart($event)"
         (touchmove)="eventTouchMove($event)"
         (touchend)="eventTouchEnd($event)">
      <sti-page *ngFor="let page of model.pages" [pageAttributes]="page" [reportPanel]="this"></sti-page>
    </div>
  `
})

export class ReportPanelComponent implements OnInit, AfterViewInit {

  @ViewChild('element') element: ElementRef;

  private scrollTimeout: any;
  private touchesLength: number;
  private touchZoomFirstDistance = 0;
  private touchZoomSecondDistance = 0;
  private touchZoomValue = 0;
  private touchStartX: number;

  public maxHeights = {};

  constructor(private controller: ControllerService, public model: ModelService, private stylesService: StylesService, private interactionService: InteractionsService) {

    controller.getMessage().subscribe((message: Message) => {

      if (message.action !== 'viewer_loaded' && this.model.reportParams.pagesArray) {
        if (this.model.reportParams.repaintOnlyDashboardContent) {
          // jsObject.controls.reportPanel.repaintDashboardContent(parameters);
        } else {
          this.loadPages();
        }
      }

      if (message.action === 'GetReport' || message.action === 'OpenReport') {
        setTimeout(() => {
          if (!this.model.options.isParametersReceived && ((this.model.reportParams.type === 'Report' && this.model.options.toolbar.showParametersButton) || this.model.reportParams.type === 'Dashboard')) {
            interactionService.postInteraction({ action: 'InitVars' });
          }

          if (this.model.reportParams.autoZoom) {
            this.controller.action({ name: this.model.reportParams.autoZoom === -1 ? 'ZoomPageWidth' : 'ZoomOnePage' });
            this.model.reportParams.autoZoom = null;
          }
        });
      }

    });
  }

  ngOnInit() { }

  ngAfterViewInit(): void {
    this.model.controls.reportPanel.el = this.element;
  }

  loadPages() {
    this.maxHeights = {};
    const pagesArray: [] = this.model.reportParams.pagesArray;
    this.stylesService.setupStyle(pagesArray[pagesArray.length - 2], 'page');
    const chartScript = pagesArray[pagesArray.length - 1];
    this.model.pages = pagesArray.slice(0, pagesArray.length - 2);

    setTimeout(() => {
      this.model.pages.forEach(page => this.interactionService.initializeInteractions(page.page));
      this.interactionService.updateAllHyperLinks();
      this.stylesService.addChartScript(chartScript);
    });

  }

  get position(): string {
    return this.model.options.heightType !== 'Percentage' || this.model.options.appearance.scrollbarsMode ? 'absolute' : 'relative';
  }

  scroll() {
    if (this.model.pagesNavigationIsActive()) {
      clearTimeout(this.scrollTimeout);

      // update current page number
      this.scrollTimeout = setTimeout(() => {
        let commonPagesHeight = 0;
        let index = 0;

        for (index = 0; index < this.model.pages.length; index++) {
          commonPagesHeight += this.model.pages[index].page.offsetHeight + 10;
          if (commonPagesHeight > this.element.nativeElement.scrollTop) {
            break;
          }
        }

        if (index < this.model.reportParams.pagesCount && index >= 0 && index !== this.model.reportParams.pageNumber) {
          this.model.reportParams.pageNumber = index;
        }
      }, 300);
    }
  }

  public eventTouchStart(e: any) {
    this.touchesLength++;
    this.touchStartX = parseInt(e.changedTouches[0].clientX, 10);

    if (this.model.options.appearance.allowTouchZoom && this.touchesLength === 1) {
      this.touchZoomFirstDistance = 0;
      this.touchZoomSecondDistance = 0;
      this.touchZoomValue = 0;
    }
  }

  public eventTouchMove(e: any) {
    if (this.model.options.appearance.allowTouchZoom && e.touches.length > 1) {
      if ('preventDefault' in e) { e.preventDefault(); }

      this.touchZoomSecondDistance = Math.sqrt(Math.pow(e.touches[0].pageX - e.touches[1].pageX, 2) + Math.pow(e.touches[0].pageY - e.touches[1].pageY, 2));
      if (this.touchZoomFirstDistance === 0) {
        this.touchZoomFirstDistance = Math.sqrt(Math.pow(e.touches[0].pageX - e.touches[1].pageX, 2) + Math.pow(e.touches[0].pageY - e.touches[1].pageY, 2));
      }

      const touchZoomOffset = Math.trunc((this.touchZoomSecondDistance - this.touchZoomFirstDistance) / 2.5);
      if (Math.abs(touchZoomOffset) >= 5) {
        this.touchZoomValue = Math.trunc((this.model.reportParams.zoom + touchZoomOffset) / 5) * 5;
        this.touchZoomValue = Math.min(Math.max(this.touchZoomValue, 20), 200);
        this.controller.actionSubject.next({ action: 'centerText', data: this.touchZoomValue.toString() });
      }
    }
  }

  public eventTouchEnd(e: any) {
    if (this.touchesLength > 0) { this.touchesLength--; }

    if (this.model.options.isMobileDevice && this.model.options.toolbar.autoHide) {
      if (Math.trunc(this.touchStartX - e.changedTouches[0].clientX) !== 0) {
        this.controller.keepToolbar();
      } else {
        if (!this.model.controls.toolbar.visible) {
          this.controller.showToolbar();
        } else {
          this.controller.hideToolbar();
        }
      }
    }

    if (this.model.options.appearance.allowTouchZoom && this.touchZoomValue !== 0 && this.touchesLength === 0) {
      this.controller.actionSubject.next({ action: 'hideCenterText' });
      this.model.reportParams.zoom = this.touchZoomValue;
      this.controller.post('GetPages');
    }
  }
}

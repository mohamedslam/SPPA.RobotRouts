import { Component, OnInit, Input, ViewChild, ElementRef, AfterViewInit } from '@angular/core';
import { ModelService } from '../services/model.service';
import { ReportPanelComponent } from './report-panel.component';
import { ControllerService } from '../services/controller.service';

@Component({
  selector: 'sti-page',
  template: `
    <div #element
      style="overflow: hidden; text-align: left; vertical-align: top; color: #000000; box-sizing: content-box; display:inline-block"
      [style.margin]="model.reportParams.viewMode === 'Continuous' ? '10px auto 10px auto' : '10px'"
      [style.display]="this.model.reportParams.viewMode === 'Continuous' ? 'table' : 'inline-block'"
      [style.border]="'1px solid ' + this.model.options.appearance.pageBorderColor"
      [style.background]="pageAttributes['background'] == 'Transparent' || pageAttributes.background == 'rgba(255,255,255,0)' ? 'White' : pageAttributes['background']"
      [style.padding]="pageAttributes['margins']"
      [style.width]="!pageAttributes['content'] ? pageWidth + 'px' : ''"
      [style.height]="!pageAttributes['content'] ? pageHeight + 'px' : ''"
      [class]="model.options.appearance.showPageShadow ? 'stiJsViewerPageShadow' : 'stiJsViewerPage'"
      (touchstart)="eventTouchStart($event)"
      (touchmove)="eventTouchMove($event)"
      (touchend)="eventTouchEnd($event)">
      </div>
  `
})

export class PageComponent implements OnInit, AfterViewInit {

  @ViewChild('element') element: ElementRef;
  @Input() pageAttributes: any;
  @Input() reportPanel: ReportPanelComponent;

  private touchesLength = 0;
  private lastTouches = [{ x: 0, y: 0, time: 0 }, { x: 0, y: 0, time: 0 }];
  private touchAllowPageAction = false;
  private touchAllowScroll = false;
  private touchStartX: number;
  private touchStartScrollY: number;
  private touchPosX: number;

  constructor(public model: ModelService, public controller: ControllerService) { }

  ngOnInit() {

  }

  ngAfterViewInit() {
    const page = this.element.nativeElement;
    page.innerHTML = this.pageAttributes.content;
    this.pageAttributes.page = page;

    // Correct Watermark
    if (this.pageAttributes.existsWatermark) {
      page.style.position = 'relative';
      for (const childNode of page.childNodes) {
        if (childNode.className === 'stiWatermarkImage') {
          childNode.style.width = 'auto';
          childNode.style.height = 'auto';
          break;
        }
      }
    }

    let reportDisplayMode = this.model.options.displayModeFromReport || this.model.options.appearance.reportDisplayMode;

    if (reportDisplayMode == "Div" || reportDisplayMode == "Span") {
      const childs = page.getElementsByClassName('StiPageContainer');
      if (childs && childs.length > 0) {
        const pageContainer = childs[0];
        pageContainer.style.position = 'relative';
        if (reportDisplayMode == "Span") {
          pageContainer.style.margin = '0 1px'; // fix Chrome bug with SPAN position
        }
        page.style.width = (this.pageWidth - this.margins[1] - this.margins[3]) + 'px';
        page.style.height = (this.pageHeight - this.margins[0] - this.margins[2]) + 'px';
      }
    }

    this.element.nativeElement.pageAttributes = this.pageAttributes;

    /*const pageHeight = this.pageHeight;
    if (pageHeight !== 0) {
      // fixed bug with long time execute
      if (this.model.options.appearance.reportDisplayMode !== 'Table' && this.model.reportParams.viewMode !== 'SinglePage') {
        setTimeout(() => {
          const currentPageHeight = page.offsetHeight - this.margins[0] - this.margins[2];
          if (this.reportPanel.maxHeights[pageHeight] == null || currentPageHeight > this.reportPanel.maxHeights[pageHeight]) {
            this.reportPanel.maxHeights[pageHeight] = currentPageHeight;
          }
        });
      } else {
        const currentPageHeight = page.offsetHeight - this.margins[0] - this.margins[2];
        if (this.reportPanel.maxHeights[pageHeight] === null || currentPageHeight > this.reportPanel.maxHeights[pageHeight]) {
          this.reportPanel.maxHeights[pageHeight] = currentPageHeight;
        }
      }
    }*/
  }

  public get margins(): number[] {
    const margins = [0, 0, 0, 0];
    if (this.pageAttributes.margins) {
      const marginsPx = this.pageAttributes.margins.split(' ');
      for (let i = 0; i < marginsPx.length; i++) {
        margins[i] = parseInt(marginsPx[i].replace('px', ''), 10);
      }
    }
    return margins;
  }

  public get pageWidth(): number {
    const pageSizes: string[] = this.pageAttributes.sizes ? this.pageAttributes.sizes.split(';') : null;
    if (pageSizes) {
      return parseInt(pageSizes[0], 10);
    }
    return 0;
  }

  public get pageHeight(): number {
    const pageSizes: string[] = this.pageAttributes.sizes ? this.pageAttributes.sizes.split(';') : null;
    if (pageSizes) {
      return parseInt(pageSizes[1], 10);
    }
    return 0;
  }

  public eventTouchStart(e: any) {
    const reportPanel = this.model.controls.reportPanel.el.nativeElement;
    this.touchAllowPageAction = this.touchesLength === 0 && Math.abs(reportPanel.offsetWidth - reportPanel.scrollWidth) <= 10;
    this.touchAllowScroll = reportPanel.offsetWidth === reportPanel.scrollWidth;
    this.touchesLength++;

    if (this.touchAllowPageAction) {
      this.touchStartX = parseInt(e.changedTouches[0].clientX, 10);
      this.touchStartScrollY = reportPanel.scrollTop;
    }
  }

  public eventTouchMove(e: any) {
    const reportPanel = this.model.controls.reportPanel.el.nativeElement;
    if (this.touchAllowPageAction) {
      this.lastTouches.shift();
      this.lastTouches.push({
        x: e.changedTouches[0].clientX,
        y: e.changedTouches[0].clientY,
        time: new Date().getTime()
      });
      if (this.touchAllowScroll && this.touchStartScrollY === reportPanel.scrollTop) {
        this.touchPosX = Math.trunc(this.lastTouches[1].x - this.touchStartX);
        if (scrollX === 0) {
          this.element.nativeElement.style.transform = `translateX(${this.touchPosX}px)`;
        }
      }
    }
  }

  public eventTouchEnd(e: any) {
    const reportPanel = this.model.controls.reportPanel.el.nativeElement;
    if (this.touchesLength > 0) { this.touchesLength--; }
    if (this.touchAllowPageAction && this.touchesLength === 0) {
      const dX = this.lastTouches[1].x - this.lastTouches[0].x;
      const dT = new Date().getTime() - this.lastTouches[1].time;

      if (this.touchStartScrollY !== reportPanel.scrollTop ||
        (dX <= 0 && this.model.reportParams.pageNumber >= this.model.reportParams.pagesCount - 1) ||
        (dX >= 0 && this.model.reportParams.pageNumber <= 0)) {
        this.translateX(0);
      } else if ((dX < -5 && dT <= 14 && this.lastTouches[1].x < this.touchStartX) ||
        (dX < 0 && this.touchPosX < -this.pageWidth / 3)) {
        this.controller.action({ name: 'NextPage' });
        this.translateX(-this.pageWidth);
      } else if ((dX > 5 && dT <= 14 && this.lastTouches[1].x > this.touchStartX) ||
        (dX > 0 && this.touchPosX > this.pageWidth / 3)) {
        this.controller.action({ name: 'PrevPage' });
        this.translateX(this.pageWidth);
      } else {
        this.translateX(0);
      }
    }
  }

  public translateX(value: number) {
    this.element.nativeElement.style.transitionDuration = '300ms';
    this.element.nativeElement.style.transform = value === 0 ? '' : `translateX(${value}px)`;
    setTimeout(() => {
      this.element.nativeElement.style.transitionDuration = '';
    }, 300);
  }
}

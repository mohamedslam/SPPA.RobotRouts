import { Component, OnInit, Input, ElementRef, ViewChild, OnChanges, Output, EventEmitter } from '@angular/core';

declare var Stimulsoft: any;

@Component({
  selector: 'stimulsoft-designer-angular',
  template: `
    <div #designer [attr.id]="id">
      <ng-content></ng-content>
    </div>
  `,
  styles: []
})
export class StimulsoftDesignerComponent implements OnInit, OnChanges {

  @ViewChild('designer') designerEl: ElementRef;

  /**
   * Occurs when designer loaded
   */
  @Output() designerLoaded: EventEmitter<any> = new EventEmitter();

  /**
   * Url to server instance (designer controller)
   * Example: http://server.url:51528/api/designer
   */
  @Input() requestUrl: string;

  /**
   * The width of Designer
   */
  @Input() width: string;

  /**
   * The height of Designer
   */
  @Input() height: string;

  id = 'aDesigner' + (Math.random() * 10000000).toString();
  designer: any;
  chartScripts: any;

  private loaded = false;

  constructor() { }

  ngOnInit(): void {
    this.addScript();
  }

  ngOnChanges(changes: import('@angular/core').SimpleChanges): void {
    this.addScript();
  }

  addScript() {
    if (this.chartScripts) {
      this.head.removeChild(this.chartScripts);
    }
    this.chartScripts = document.createElement('Script') as HTMLScriptElement;
    this.chartScripts.setAttribute('type', 'text/javascript');
    this.chartScripts.src = this.requestUrl;
    this.chartScripts.addEventListener('load', () => {
      if (!this.loaded) {
        try {
          const options = new Stimulsoft.Designer.StiDesignerOptions();
          this.loaded = true;
          setTimeout(() => {
            this.designerLoaded.next();
          });
          this.showDesigner();
        } catch { }
      }
    });
    this.head.appendChild(this.chartScripts);
  }

  showDesigner() {
    const options = new Stimulsoft.Designer.StiDesignerOptions();
    if (this.width) {
      options.width = this.width;
    }
    if (this.height) {
      options.height = this.height;
    }

    this.designer = new Stimulsoft.Designer.StiDesigner(options, 'StiDesigner', false);
    this.designer.renderHtml(this.designerEl.nativeElement);
  }

  public get head(): HTMLHeadElement {
    return document.getElementsByTagName('head')[0];
  }

}

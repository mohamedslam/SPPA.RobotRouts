import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { ControllerService } from '../services/controller.service';
import { Message } from '../services/objects';
import { HelperService } from '../services/helper.service';
import { FormService } from '../forms/form.service';
import { ModelService } from '../services/model.service';

@Component({
  selector: 'sti-about-panel',
  template: `
    <div #element class="stiJsViewerAboutPanel" style="background-size: contain;"
      [style.display]="formService.form?.name == 'about' ? '' : 'none'"
      [style.left.px]="left"
      [style.top.px]="top"
      [style.opacity]="opacity"
      (click)="close()" >
      <div class="stiJsViewerAboutPanelHeader">
        Stimulsoft Reports
      </div>
      <img src="{{model.img('About.png')}}" style="margin-top: 30px; height: 94px; width: 94px;">
      <div class="stiJsViewerAboutPanelCopyright">
        {{'Copyright 2003-' + year + ' Stimulsoft'}}
      </div>
      <div class="stiJsViewerAboutPanelVersion">
        {{model.options.productVersion?.trim()}}, {{model.options?.frameworkType}}, Angular
      </div>
      <div class="stiJsViewerAboutPanelVersion">
        All rights reserved
      </div>
      <div class="stiJsViewerAboutPanelVersion" style="margin-top: 20px;font-weight: bold; "
        [style.color]="isLicensed() ? 'red' : '#444444'"
        [style.display]="isLicensed() || userLabel ? '' : 'none'">
        {{userLabel}}
      </div>
      <div class="stiJsViewerFormSeparator" style="margin-top: 20px;">
      </div>
      <div class="stiJsViewerAboutPanelStiLink" (click)='click($event)'>
        www.stimulsoft.com
      </div>
    </div>
  `
})

export class AboutPanelComponent implements OnInit {

  @ViewChild('element') element: ElementRef;

  year: number = new Date().getFullYear();
  jsHelper: any;
  top = 0;
  left = 0;
  opacity = 0;

  constructor(public model: ModelService, public controller: ControllerService, public helper: HelperService,
    public formService: FormService) {
    controller.getActionMessage().subscribe((message: Message) => {
      if (message.action === 'About') {
        this.formService.form = { name: 'about', left: 0, top: 0, isMooving: false, level: 2 };
        this.opacity = 0;
        setTimeout(() => {
          this.top = this.model.viewerSize.height / 2 - this.element.nativeElement.offsetHeight / 2;
          this.left = this.model.viewerSize.width / 2 - this.element.nativeElement.offsetWidth / 2;
          this.opacity = 1;
        });
      }
    });
  }

  ngOnInit() { }

  close() {
    this.formService.closeForm('about');
  }

  click(event: any) {
    if (event) {
      event.stopPropagation();
      event.preventDefault();
    }
    this.helper.openNewWindow('https://www.stimulsoft.com');
  }

  isLicensed(): boolean {
    return !this.model.options.cloudMode && !this.model.options.serverMode && !this.model.options.standaloneJsMode &&
      !this.model.options.reportDesignerMode && !this.model.options.licenseIsValid;
  }

  get userLabel(): string {
    let userName = this.model.options.licenseUserName || '';
    if (this.isLicensed()) {
      if (userName) {
        userName += ', ';
      }
      return userName + this.helper.getBackText(true) + ' Version';
    } else {
      return userName;
    }
  }
}

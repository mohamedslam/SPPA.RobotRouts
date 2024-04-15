import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { ModelService } from '../services/model.service';
import { ControllerService } from '../services/controller.service';
import { Message } from '../services/objects';
import { HelperService } from '../services/helper.service';

@Component({
  selector: 'sti-center-text',
  template: `
    <div #element
      style="position: absolute; z-index: 1000; transition-property: opacity; transition-duration: 300ms; text-shadow: rgb(0, 0, 0) -1px -1px 0px, rgb(0, 0, 0) 1px -1px 0px, rgb(0, 0, 0) -1px 1px 0px, rgb(0, 0, 0) 1px 1px 0px; font-size: 100px;"
      [style.fontFamily]="model.options.toolbar.fontFamily"
      [style.color]="model.options.toolbar.fontColor"
      [style.opacity]="opacity"
      [style.display]="display">
      <div>
        {{text}}
      </div>
    </div>
  `
})

export class CenterTextComponent implements OnInit {

  @ViewChild('element') element: ElementRef;

  public text = '';
  public opacity = 0;
  public display = 'none';

  private hideTimer: any;

  constructor(public model: ModelService, public controller: ControllerService, public helper: HelperService) {
    controller.getActionMessage().subscribe((message: Message) => {
      switch (message.action) {
        case 'centerText':
          this.show(message.data);
          break;

        case 'hideCenterText':
          this.hide();
          break;
      }
    });
  }

  ngOnInit() { }

  public show(text: string) {
    this.display = '';
    this.opacity = 0;
    this.text = text;
    setTimeout(() => {
      this.helper.setObjectToCenter(this.element.nativeElement);
      this.opacity = 1;
    });

    if (this.hideTimer) { clearTimeout(this.hideTimer); }
    this.hideTimer = setTimeout(() => {
      this.hide();
    }, 2000);
  }

  public hide() {
    this.opacity = 0;
    if (this.hideTimer) { clearTimeout(this.hideTimer); }
    this.hideTimer = setTimeout(() => {
      this.display = 'none';
    }, 300);
  }
}

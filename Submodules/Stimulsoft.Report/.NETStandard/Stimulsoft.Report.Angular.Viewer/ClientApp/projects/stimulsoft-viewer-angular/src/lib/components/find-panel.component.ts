import { Component, OnInit, AfterViewInit, ViewChild, ElementRef } from '@angular/core';
import { ModelService } from '../services/model.service';
import { ComponentDescription, Message } from '../services/objects';
import { FindService } from '../services/find.service';
import { TextBoxComponent } from '../controls/text-box.componet';
import { ControllerService } from '../services/controller.service';

@Component({
  selector: 'sti-find-panel',
  template: `
    <div #element [style.display]="model.controls.findPanel.visible ? '' : 'none'"
    [class]="'stiJsViewerToolBar' + (model.options.toolbar.displayMode == 'Separated' ? ' stiJsViewerToolBarSeparated' : '')">
      <div [style.paddingTop]="model.options.toolbar.displayMode == 'Simple' ? '2px' : ''">
        <div class="stiJsViewerToolBarTable" [style.border]="model.options.toolbar.displayMode == 'Separated' ? '0px' : ''" style="box-sizing: border-box">
          <table class="stiJsViewerClearAllStyles" cellpadding="0" cellspacing="0" style="margin: 0">
            <tbody>
                <tr class="stiJsViewerClearAllStyles">
                   <td *ngFor="let item of items"  class="stiJsViewerClearAllStyles">
                      <sti-button *ngIf="item.type=='button'"
                        [actionName]="item.action"
                        [imageName]="item.img"
                        [caption]="item.caption"
                        [margin]="item.margin"
                        [height]="model.options.toolbar.displayMode == 'Separated' ? '28px' : ''"
                        (action)="action(item.action)"
                        [selected]="selected(item)"
                        ></sti-button>
                      <sti-text-box #textBox *ngIf="item.type=='textBox'" [width]="170"
                        [margin]="item.margin"
                        [focusOnCreate]="true"
                        (action)="textBoxAction($event)"
                        [value]="findService.text"></sti-text-box>
                      <sti-text-block *ngIf="item.type=='textBlock'" [margin]="item.margin" [text]="item.caption"></sti-text-block>
                  </td>
                </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>
  `
})

export class FindPanelComponent implements OnInit, AfterViewInit {

  @ViewChild('element') element: ElementRef;
  @ViewChild('textBox') textBox: TextBoxComponent;

  items: ComponentDescription[];
  private changeFind = false;

  constructor(public model: ModelService, public findService: FindService, public controller: ControllerService) {
    this.initItems();

    controller.getMessage().subscribe((message: Message) => {
      switch (message.action) {
        case 'GetReport':
        case 'GetPages':
        case 'OpenReport':
        case 'Variables':
          if (model.controls.findPanel.visible) {
            setTimeout(() => {
              this.findService.hideFindLabels();
              this.find('Next');
            });
          }
          break;
      }
    });
  }

  ngAfterViewInit(): void {
    this.model.controls.findPanel.el = this.element;

    this.model.controls.findPanel.getVisibility().subscribe(() => {
      if (this.model.controls.findPanel.visible) {
        this.findService.text = '';
        setTimeout(() => {
          this.textBox.element.nativeElement.focus();
        });
      }
    });
  }

  ngOnInit() { }

  selected(item: ComponentDescription): boolean {
    switch (item.action) {
      case 'MatchCase':
        return this.findService.matchCase;
      case 'MatchWholeWord':
        return this.findService.matchWholeWord;
    }
    return false;
  }

  textBoxAction(target: any) {
    this.findService.text = target.value;
    this.find('Next');
  }

  find(direction: string) {
    if (this.findService.text === '') {
      this.findService.hideFindLabels();
      return;
    }
    if (this.findService.lastFindText !== this.findService.text || this.changeFind) {
      this.changeFind = false;
      this.findService.showFindLabels();
    } else {
      this.findService.selectFindLabel(direction);
    }
  }

  initItems() {
    this.items = [
      { type: 'button', action: 'close', img: 'CloseForm.png', margin: '2px' },
      { type: 'textBlock', action: 'text', caption: this.model.loc('FindWhat').replace(":", ""), margin: '2px' },
      { type: 'textBox', action: 'findTextBox', margin: '2px' },
      { type: 'button', action: 'FindPreviows', caption: this.model.loc('FindPrevious'), img: 'Arrows.ArrowUpBlue.png', margin: '2px' },
      { type: 'button', action: 'FindNext', caption: this.model.loc('FindNext'), img: 'Arrows.ArrowDownBlue.png', margin: '2px' },
      { type: 'button', action: 'MatchCase', caption: this.model.loc('MatchCase').replace('&', ''), margin: '2px' },
      { type: 'button', action: 'MatchWholeWord', caption: this.model.loc('MatchWholeWord').replace('&', ''), margin: '2px' }
    ];
  }

  action(action: string) {
    switch (action) {
      case 'close':
        this.model.controls.findPanel.visible = false;
        break;
      case 'MatchCase':
        this.findService.matchCase = !this.findService.matchCase;
        this.changeFind = true;
        break;
      case 'MatchWholeWord':
        this.findService.matchWholeWord = !this.findService.matchWholeWord;
        this.changeFind = true;
        break;
      case 'FindPreviows':
        this.findService.text = this.textBox.element.nativeElement.value;
        this.find('Previous');
        break;
      case 'FindNext':
        this.findService.text = this.textBox.element.nativeElement.value;
        this.find('Next');
        break;
    }
  }
}

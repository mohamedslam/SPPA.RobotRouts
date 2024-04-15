import { Component, OnInit, Input, EventEmitter, Output, ViewChild, ElementRef } from '@angular/core';
import { ModelService } from '../services/model.service';
import { HelperService } from '../services/helper.service';
import { trigger, state, transition, style, animate, AnimationEvent } from '@angular/animations';
import { FormService } from './form.service';

@Component({
  selector: 'sti-base-form',
  template: `
    <div #element class="stiJsViewerForm"
      [style.zIndex]="level * 10 + 1"
      [@.disabled]="!model.options.toolbar.menuAnimation"
      [style.fontFamily]="fontFamily"
      [style.color]="color"
      [style.fontSize]="fontSize"
      [style.left.px]="formService.form?.left"
      [style.top.px]="formService.form?.top"
      [style.display]="display"
      [@showForm]="formService.form?.name == name ? 'visible' : 'hidden'"
      (@showForm.start)="startAnimation($event)"
      (@showForm.done)="doneAnimation($event)">
      <div class="stiJsViewerFormHeader"
        (mousedown)="formService.startMove(name, $event)"
        (touchstart)="formService.startMove(name, null, $event)"
        (touchmove)="formService.move($event)"
        (touchend)="formService.stopMove()">
        <table class="stiJsViewerClearAllStyles" cellpadding="0" cellspacing="0" style="width: 100%;">
          <tbody>
            <tr class="stiJsViewerClearAllStyles">
              <td class="stiJsViewerClearAllStyles" style="text-align: left; padding: 5px 10px 8px 15px;">
                {{caption}}
              </td>
              <td *ngIf="helpUrl != null && model.options.appearance.showDialogsHelp" class="stiJsViewerClearAllStyles" style="width: 20px; text-align: right; padding: 2px 0px 1px 0px; vertical-align: top;">
                <sti-button [imageMargin]="'0 2px 0 2px'" [display]="'inline-block'" (action)="helper.showHelpWindow(helpUrl)">
                </sti-button>
              </td>
              <td class="stiJsViewerClearAllStyles" style="vertical-align: top; width: 30px; text-align: right; padding: 2px 1px 1px;">
                <sti-button [imageMargin]="'0 2px 0 2px'" [display]="'inline-block'" [imageName]="'CloseForm.png'" (action)="formService.closeForm(name)">
                </sti-button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>

      <div class="stiJsViewerFormContainer" [style.padding]="containerPadding">
        <ng-content>
        </ng-content>
      </div>

      <div *ngIf="showSeparator" class="stiJsViewerFormSeparator"></div>

      <div *ngIf="showButtons" class="stiJsViewerFormButtonsPanel">
        <table class="stiJsViewerClearAllStyles" cellpadding="0" cellspacing="0">
          <tbody>
            <tr class="stiJsViewerClearAllStyles">
              <td class="stiJsViewerClearAllStyles" style="padding: 8px;">
                <sti-form-button [caption]="model.loc('ButtonOk')" (action)="action.emit()">
                </sti-form-button>
              </td>
              <td *ngIf="showCancel" class="stiJsViewerClearAllStyles" style="padding: 8px 8px 8px 0px;">
                <sti-form-button [caption]="model.loc('ButtonCancel')" (action)="formService.closeForm(name)">
                </sti-form-button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>

    </div>
  `,
  animations: [
    trigger('showForm', [
      state('hidden', style({ opacity: '0' })),
      state('visible', style({ opacity: '1' })),
      transition('hidden => visible', [
        animate('200ms ease-in-out')])
    ])
  ]
})

export class BaseFormComponent implements OnInit {

  @ViewChild('element') element: ElementRef;

  @Output() changeVisibility: EventEmitter<any> = new EventEmitter();
  @Output() action: EventEmitter<any> = new EventEmitter();

  @Input() level = 1;
  @Input() caption: string;
  @Input() helpUrl: string;
  @Input() fontFamily: string;
  @Input() color: string;
  @Input() fontSize: string;
  @Input() containerPadding: string;
  @Input() name: string;
  @Input() defaultTop: number;
  @Input() showCancel = true;
  @Input() showButtons = true;
  @Input() showSeparator = true;

  display = 'none';

  constructor(public model: ModelService, public helper: HelperService, public formService: FormService) { }

  ngOnInit() { }

  startAnimation(event: AnimationEvent) {
    if (event.toState === 'visible') {
      this.display = '';
      if (this.defaultTop != null) {
        this.formService.centerForm(this, this.defaultTop);
      }
    }
  }

  doneAnimation(event: AnimationEvent) {
    if (event.toState === 'hidden') {
      this.display = 'none';
    }
    this.changeVisibility.emit(event.toState);
  }
}

import { Component, OnInit } from '@angular/core';
import { ModelService } from '../services/model.service';
import { MailService } from '../services/mail.service';
import { ExportService } from '../services/export.service';

@Component({
  selector: 'sti-send-email-form',
  template: `
    <sti-base-form #baseForm [fontFamily]="model.options.toolbar.fontFamily"
      [color]="model.options.toolbar.fontColor"
      [fontSize]="'12px'"
      [name]="'sendEmailForm'"
      [caption]="model.loc('EmailOptions')"
      [level]="1"
      [defaultTop]="150"
      (action)="this.mailService.sendMail()">
      <table class="stiJsViewerClearAllStyles" cellpadding="0" cellspacing="0" style="width: 100%;">
        <tbody>
            <tr class="stiJsViewerClearAllStyles">
              <td class="stiJsViewerCaptionControls" [style.padding]="model.options.isMobileDevice ? '0 25px 0 4px' : ''">
                {{model.loc('Email')}}
              </td>
              <td *ngIf="!model.options.isMobileDevice">
                <sti-text-box [margin]="model.options.isMobileDevice ? '4px 4px 12px 4px' : '4px'"
                  [value]="exportService.exportSettings.Email"
                  [width]="model.options.isMobileDevice ? 200 : 280"
                  (onchange)="exportService.exportSettings.Email = $event.value">
                </sti-text-box>
              </td>
            </tr>
            <tr class="stiJsViewerClearAllStyles" *ngIf="model.options.isMobileDevice">
              <td class="stiJsViewerClearAllStyles">
                <sti-text-box [margin]="model.options.isMobileDevice ? '4px 4px 12px 4px' : '4px'"
                  [value]="exportService.exportSettings.Email"
                  [width]="model.options.isMobileDevice ? 200 : 280"
                  (onchange)="exportService.exportSettings.Email = $event.value">
                </sti-text-box>
              </td>
            </tr>

            <tr class="stiJsViewerClearAllStyles">
              <td class="stiJsViewerCaptionControls" [style.padding]="model.options.isMobileDevice ? '0 25px 0 4px' : ''">
                {{model.loc('Subject')}}
              </td>
              <td *ngIf="!model.options.isMobileDevice">
                <sti-text-box [margin]="model.options.isMobileDevice ? '4px 4px 12px 4px' : '4px'"
                  [value]="exportService.exportSettings.Subject"
                  [width]="model.options.isMobileDevice ? 200 : 280"
                  (onchange)="exportService.exportSettings.Subject = $event.value">
                </sti-text-box>
              </td>
            </tr>
            <tr class="stiJsViewerClearAllStyles" *ngIf="model.options.isMobileDevice">
              <td class="stiJsViewerClearAllStyles">
                <sti-text-box [margin]="model.options.isMobileDevice ? '4px 4px 12px 4px' : '4px'"
                  [value]="exportService.exportSettings.Subject"
                  [width]="model.options.isMobileDevice ? 200 : 280"
                  (onchange)="exportService.exportSettings.Subject = $event.value">
                </sti-text-box>
              </td>
            </tr>

            <tr class="stiJsViewerClearAllStyles">
              <td class="stiJsViewerCaptionControls" [style.padding]="model.options.isMobileDevice ? '0 25px 0 4px' : ''">
                {{model.loc('Message')}}
              </td>
              <td *ngIf="!model.options.isMobileDevice">
                <sti-text-area [margin]="model.options.isMobileDevice ? '4px 4px 12px 4px' : '4px'"
                  [value]="exportService.exportSettings.Message"
                  [height]="70"
                  [width]="model.options.isMobileDevice ? 200 : 280"
                  (onchange)="exportService.exportSettings.Message = $event.value">
                </sti-text-area>
              </td>
            </tr>
            <tr class="stiJsViewerClearAllStyles" *ngIf="model.options.isMobileDevice">
              <td class="stiJsViewerClearAllStyles">
                <sti-text-area [margin]="model.options.isMobileDevice ? '4px 4px 12px 4px' : '4px'"
                  [value]="exportService.exportSettings.Message"
                  [height]="70"
                  [width]="model.options.isMobileDevice ? 200 : 280"
                  (onchange)="exportService.exportSettings.Message = $event.value">
                </sti-text-area>
              </td>
            </tr>

            <tr class="stiJsViewerClearAllStyles">
              <td class="stiJsViewerCaptionControls" [style.padding]="model.options.isMobileDevice ? '0 25px 0 4px' : ''">
                {{model.loc('Attachment')}}
              </td>
              <td *ngIf="!model.options.isMobileDevice">
                <div style="margin: 4px">
                  {{exportService.exportSettings.Attachment}}
                </div>
              </td>
            </tr>
            <tr class="stiJsViewerClearAllStyles" *ngIf="model.options.isMobileDevice">
              <td class="stiJsViewerClearAllStyles">
                <div style="margin: 4px">
                  {{exportService.exportSettings.Attachment}}
                </div>
              </td>
            </tr>
        </tbody>
      </table>
    </sti-base-form>
  `
})

export class SendEmailFormComponent implements OnInit {

  constructor(public model: ModelService, public mailService: MailService, public exportService: ExportService) { }

  ngOnInit() { }

}

import { Injectable } from '@angular/core';
import { ModelService } from './model.service';
import { ExportService } from './export.service';
import { FormService } from '../forms/form.service';
import { ControllerService } from './controller.service';

@Injectable()
export class MailService {

  constructor(public model: ModelService, public exportService: ExportService, public formService: FormService, public controller: ControllerService) {
    exportService.getMessage().subscribe(event => {
      if (event === 'postMail') {
        this.postMail();
      }
    });
  }

  public postMail() {
    this.fillDefaults();

    if (this.model.options.email.showEmailDialog) {
      this.formService.showForm('sendEmailForm');
    } else {
      this.sendMail();
    }
  }

  public sendMail() {
    const data = {
      exportFormat: this.exportService.format,
      exportSettings: this.exportService.exportSettings
    };

    this.formService.closeForm('sendEmailForm');
    this.controller.post('EmailReport', this.model.options.actions.emailReport, data);
    this.controller.actionSubject.next({ action: 'Email', data });
  }

  public fillDefaults() {
    this.exportService.exportSettings.Email = this.model.options.email.defaultEmailAddress;
    this.exportService.exportSettings.Message = this.model.options.email.defaultEmailMessage;
    this.exportService.exportSettings.Subject = this.model.options.email.defaultEmailSubject;

    let ext = this.exportService.format.toLowerCase().replace('image', '');
    switch (ext) {
      case 'excel': ext = 'xls'; break;
      case 'excel2007': ext = 'xlsx'; break;
      case 'excelxml': ext = 'xls'; break;
      case 'html5': ext = 'html'; break;
      case 'jpeg': ext = 'jpg'; break;
      case 'ppt2007': ext = 'ppt'; break;
      case 'text': ext = 'txt'; break;
      case 'word2007': ext = 'docx'; break;
    }

    this.exportService.exportSettings.Attachment = this.model.reportParams.reportFileName + '.' + ext;
  }

}

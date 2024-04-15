import { Injectable } from '@angular/core';
import { ModelService } from './model.service';
import { ExportService } from './export.service';
import { HelperService } from './helper.service';
import { StiHttpClientService } from './http-client.service';
import { ControllerService } from './controller.service';
import { Message } from './objects';

@Injectable()
export class PrintService {

  constructor(public model: ModelService, public exportService: ExportService, public helper: HelperService,
    public httpClient: StiHttpClientService, public controller: ControllerService) {

    controller.getActionMessage().subscribe((message: Message) => {
      if (message.action === 'Print') {
        this.postPrint(message.data.format);
      }
    });

    controller.getMessage().subscribe((message: Message) => {
      if (message.action === 'PrintReport') {
        switch (message.subAction) {
          case 'PrintWithPreview':
            this.printAsPopup(message.data);
            break;

          case 'PrintWithoutPreview':
            this.printAsHtml(message.data);
            break;
        }
      }
    });
  }

  postPrint(printAction: string) {
    if (printAction === 'PrintPdf' && /iPhone|iPad|iPod|Macintosh/i.test(navigator.userAgent) && this.helper.isTouchDevice()) {
      const settings = this.exportService.getDefaultExportSettings('Pdf');
      settings.OpenAfterExport = true;
      this.postExport('Pdf', settings);
      return;
    }

    const data = {
      action: 'PrintReport',
      printAction,
      bookmarksPrint: this.model.options.appearance.bookmarksPrint
    };

    const url = ''; // this.model.options.requestUrl.replace('{action}', this.model.options.actions.printReport);
    switch (printAction) {
      case 'PrintPdf':
        if (this.model.options.appearance.printToPdfMode === 'Popup' || this.helper.getNavigatorName() == 'Safari' || this.helper.getNavigatorName() == 'iPad') {
          this.printAsPdfPopup(data);
        } else {
          this.printAsPdf(url, data);
        }
        break;

      default:
        this.controller.post('PrintReport', this.model.options.actions.printReport, data, 'text', printAction);
        break;
    }
  }

  public postExport(format: string, settings: any, elementName?: string, isDashboardExport?: boolean) {
    const data = {
      action: isDashboardExport ? 'ExportDashboard' : 'ExportReport',
      exportFormat: format,
      exportSettings: settings,
      elementName
    };

    const doc = settings && settings.OpenAfterExport && this.model.options.appearance.openExportedReportWindow === '_blank' ? this.helper.openNewWindow('about:blank', '_blank').document : null;
    const url = this.model.requestUrl;
    this.httpClient.postForm(url.replace('{action}', this.model.options.actions.exportReport), data, doc);
  }

  public printAsPdfPopup(data: any) {
    const url = this.model.requestUrl.replace('{action}', this.model.options.actions.printReport);
    const win = this.helper.openNewWindow('about:blank', '_blank');
    if (win != null) {
      this.httpClient.postForm(url, data, win.document);
    }
  }

  public printAsPdf(url: string, data: any) {
    data.responseType = 'blob';
    const viewer = this.model.controls.viewer.el.nativeElement;
    const printFrameId = this.model.options.viewerId + '_PdfPrintFrame';
    let printFrame: any = document.getElementById(printFrameId);
    if (printFrame) { viewer.removeChild(printFrame); }

    printFrame = document.createElement('iframe');
    printFrame.id = printFrameId;
    printFrame.name = printFrameId;
    printFrame.width = '0';
    printFrame.height = '0';
    printFrame.style.position = 'absolute';
    printFrame.style.border = 'none';

    // Firefox does not load the invisible content of the iframe
    if (this.helper.getNavigatorName() === 'Mozilla') {
      printFrame.width = '100px';
      printFrame.height = '100px';
      printFrame.style.visibility = 'hidden';
      printFrame.style.zIndex = '-100';
      printFrame.style.pointerEvents = 'none';
    }

    viewer.insertBefore(printFrame, viewer.firstChild);

    // Manual printing in browsers that do not support automatic PDF printing
    if (this.helper.getNavigatorName() !== 'Mozilla') {
      printFrame.onload = () => {
        printFrame.contentWindow.focus();
        printFrame.contentWindow.print();
      };
    }

    const form: any = document.createElement('FORM');
    form.setAttribute('id', 'printForm');
    form.setAttribute('method', 'POST');
    form.setAttribute('action', this.model.requestUrl.replace('{action}', this.model.options.actions.printReport));
    form.setAttribute('target', this.model.options.viewerId + '_PdfPrintFrame');

    const params = this.model.createPostParameters(data, true);
    Object.keys(params).forEach(key => {
      const paramsField = document.createElement('INPUT');
      paramsField.setAttribute('type', 'hidden');
      paramsField.setAttribute('name', key);
      paramsField.setAttribute('value', params[key]);
      form.appendChild(paramsField);
    });

    document.body.appendChild(form);
    form.submit();
    document.body.removeChild(form);
  }

  public printAsPopup(text: string) {
    const width = this.model.reportParams.pagesWidth || 790;
    const win = this.helper.openNewWindow('about:blank', 'PrintReport', 'height=900,width=' + width + ',toolbar=no,menubar=yes,scrollbars=yes,resizable=yes,location=no,directories=no,status=no');
    if (win != null) {
      win.document.open();
      win.document.write(text);
      win.document.close();
    }
  }

  public printAsHtml(text: string) {
    if (this.controller.showError(text)) { return; }

    const viewer = this.model.controls.viewer.el.nativeElement;
    // Remove '_PdfPrintFrame', this should fix IE strange error
    let printFrameId = this.model.options.viewerId + '_PdfPrintFrame';
    let printFrame: any = document.getElementById(printFrameId);
    if (printFrame) { viewer.removeChild(printFrame); }

    printFrameId = this.model.options.viewerId + '_HtmlPrintFrame';
    printFrame = document.getElementById(printFrameId);
    if (printFrame) { viewer.removeChild(printFrame); }

    printFrame = document.createElement('iframe');
    printFrame.id = printFrameId;
    printFrame.name = printFrameId;
    printFrame.width = '0';
    printFrame.height = '0';
    printFrame.style.position = 'absolute';
    printFrame.style.border = 'none';
    viewer.insertBefore(printFrame, viewer.firstChild);

    printFrame.contentWindow.document.open();
    printFrame.contentWindow.document.write(text);
    printFrame.contentWindow.document.close();
    setTimeout(() => {
      printFrame.contentWindow.focus();
      printFrame.contentWindow.print();
    });
  }
}
